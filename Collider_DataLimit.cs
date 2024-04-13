using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider_DataLimit : MonoBehaviour
{

    public int dataLimit = 100;
    PlayerMovement _playerMovement;
    [SerializeField] LayerMask _layerMask;


    // Start is called before the first frame update
    void Start()
    {
        _playerMovement = FindObjectOfType<PlayerMovement>();
        Debug.Log($"{gameObject.layer}");
    }



    private void OnCollisionEnter(Collision collision)
    {
        Collider target = collision.gameObject.GetComponent<Collider>();


        if (collision.gameObject.tag == "Player")
        {
            if (_playerMovement.PlayerDataSize > dataLimit)
            {
                target.excludeLayers = 0;
            }
            else
            {
                target.excludeLayers = _layerMask;
            }
        }
            
        if (collision.gameObject.tag == "Pickup")
        {
            if (collision.gameObject.GetComponent<FileBehavior>().FileSI.dataSizeKB > dataLimit)
            {
                target.excludeLayers = 0;
            }
            else
            {
                target.excludeLayers = _layerMask;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Collider target = collision.gameObject.GetComponent<Collider>();


        if (!_playerMovement.IsHolding)
        {
            if (collision.gameObject.tag == "Player")
            {

                target.excludeLayers = _layerMask;

            }
        }
        
        
        
        if (collision.gameObject.tag == "Pickup")
        {


            target.excludeLayers = 0;
        }
    }

}
