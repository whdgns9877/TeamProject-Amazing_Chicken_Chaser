using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Banana : MonoBehaviourPun
{

    private void OnTriggerEnter(Collider other)
    {
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "Me")
            {
                PhotonNetwork.Destroy(gameObject);
            }
    }

}
