using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_FreezeXZ_OnGround : MonoBehaviour
{
    PlayerMovement _playerMov;

    private void Start()
    {
        _playerMov = FindObjectOfType<PlayerMovement>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_playerMov.heldItem != null && _playerMov.heldItem != gameObject)
        { 

            if (collision.gameObject.tag != "Player")
            {
                if (collision.contacts[0].normal == Vector3.up)
                {
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    GetComponent<Rigidbody>().constraints -= (int)RigidbodyConstraints.FreezePositionY;
                }
            }
            
        }
        else
        {
            if (collision.gameObject.tag != "Player")
            {
                if (collision.contacts[0].normal == Vector3.up)
                {
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    GetComponent<Rigidbody>().constraints -= (int)RigidbodyConstraints.FreezePositionY;
                }
            }
        }
        
        
        
    }
}
