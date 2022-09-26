using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawn_JSP : MonoBehaviour
{
    public GameObject PrefabChicken = null;

    void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            PhotonNetwork.Instantiate("Chicken", new Vector3(i + 2, 0, 0) , Quaternion.identity);
        }
    }
}
