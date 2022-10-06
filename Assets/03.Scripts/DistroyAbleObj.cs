using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistroyAbleObj : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Me" || collision.gameObject.tag == "Missile")
        {
            if(GetComponent<Rigidbody>() == null)
            gameObject.AddComponent<Rigidbody>();
        }
    }
}
