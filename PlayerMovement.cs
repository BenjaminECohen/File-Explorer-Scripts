using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float gravity = 9.8f;
    public float moveSpeed = 5f;
    public float jumpPower = 5f;
    public float pickupRaycastLength = 1f;

    Rigidbody _rb;
    Animator _anim;
    public Animator Animator { get { return _anim; } }

    private float horizontal;
    private float depth;


    [Header("Jump Detection")]
    public float jumpCooldown = 0.5f;
    public float castDepth = 0.1f;
    public Vector3 vectorOffset = Vector3.zero;


    [Header("Private var, but exposed")]
    [SerializeField] bool canJump = true;

    [Header("For File Pickup")]
    public Vector3 OffsetPickupRayCast = Vector3.zero;
    public float TossPower = 10f;
    float lastHorizontalMagnitude = 1f;
    [SerializeField] bool holdingItem = false;
    public bool IsHolding { get { return holdingItem; } }
    public Transform holdPos;
    public GameObject heldItem;
    Transform itemLastParent;
    [SerializeField] int playerDataSize = 0;  
    public int PlayerDataSize { get { return playerDataSize; } }

    ParticleSystem _wandSparkles;
    public ParticleSystem WandSparkles { get { return _wandSparkles; } }
    


    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
        _wandSparkles = GetComponentInChildren<ParticleSystem>();

        // let's just turn gravity off here incase I forget to set it up :(
        _rb.useGravity = false;
    }



    private void FixedUpdate()
    {
        //transform.lossyScale gives the global scale of an object
        _rb.velocity = (Vector3.right * moveSpeed * transform.lossyScale.y * horizontal) + new Vector3(0, _rb.velocity.y, 0);

        // custom gravity, since unity doesn't support individual gravity scale
        // REMEMBER TO TURN OFF USE GRAVITY ON PLAYER RB
        _rb.AddForce(gravity * Vector3.down * transform.lossyScale.y);
    }


    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        depth = Input.GetAxis("Vertical");

        if (canJump && !holdingItem && Physics.Raycast(vectorOffset + transform.position, Vector3.down, castDepth * transform.lossyScale.y))
        {
            if (Input.GetButtonDown("Jump"))
            {
                _rb.AddForce(Vector3.up * jumpPower * transform.lossyScale.y, ForceMode.VelocityChange);
                canJump = false;
                _anim.SetTrigger("IsJumping");
                StartCoroutine(JumpCooldown());
            }
            
        }
        
        if (horizontal > 0)
        {

            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontal < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        }

        if (horizontal != 0) //Get last horizontal value (normalized)
        {
            lastHorizontalMagnitude = horizontal / Mathf.Abs(horizontal);
            _anim.SetBool("IsWalking", true);
            _anim.SetFloat("HoldandWalk", 0.5f);
            _anim.SetBool("IsIdle", false);
        }
        else
        {
            _anim.SetBool("IsIdle", true);
            _anim.SetBool("IsWalking", false);
            _anim.SetFloat("HoldandWalk", 0.5f);
        }

        //PICKUP PORTION OF SCRIPT

        RaycastHit hit;
        if (holdingItem && Input.GetKeyDown(KeyCode.F)) //Put down
        {

            holdingItem = false;
            _anim.SetBool("IsHolding", false);
            heldItem.GetComponent<Rigidbody>().useGravity = true;

            heldItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            heldItem.GetComponent<Rigidbody>().constraints -= (int)RigidbodyConstraints.FreezePositionY + (int)RigidbodyConstraints.FreezePositionX;
            heldItem.GetComponent<Rigidbody>().AddForce(Vector3.right * lastHorizontalMagnitude * TossPower * Mathf.Abs(transform.lossyScale.x), ForceMode.VelocityChange);

            heldItem.transform.parent = itemLastParent;

            playerDataSize -= heldItem.GetComponent<FileBehavior>().FileSI.dataSizeKB;

            heldItem = null;

            //Player Layer
            GetComponent<Collider>().excludeLayers = LayerMask.GetMask("HoldingBlocker");

        }
        else if (Physics.Raycast(transform.position + OffsetPickupRayCast, Vector3.right * Mathf.Abs(transform.lossyScale.x) * lastHorizontalMagnitude * pickupRaycastLength, out hit, 1f))
        {
            if (hit.collider.tag == "Pickup")
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (!holdingItem) //Pickup
                    {
                        holdingItem = true;
                        _anim.SetBool("IsHolding", true);
                        _anim.SetTrigger("IsGrabbing");
                        heldItem = hit.collider.gameObject;
                        itemLastParent = heldItem.transform.parent;
                        heldItem.transform.parent = transform;
                        heldItem.transform.position = holdPos.position + new Vector3(0, heldItem.GetComponent<Collider>().bounds.center.y - heldItem.GetComponent<Collider>().bounds.min.y, 0);
                        heldItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        heldItem.GetComponent<Rigidbody>().useGravity = false;

                        //Set player Layer
                        GetComponent<Collider>().excludeLayers = 0;
                        playerDataSize += heldItem.GetComponent<FileBehavior>().FileSI.dataSizeKB;


                    }
                }
            }

        }
        


    }

    



    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }



    


    private void OnDrawGizmos()
    {
        if (canJump)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(vectorOffset + transform.position, Vector3.down * castDepth * transform.lossyScale.y);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(vectorOffset + transform.position, Vector3.down * castDepth * transform.lossyScale.y);
        }

        //Raycast for pickup
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + OffsetPickupRayCast, Vector3.right * Mathf.Abs(transform.lossyScale.x) * lastHorizontalMagnitude * pickupRaycastLength);

    }

}
