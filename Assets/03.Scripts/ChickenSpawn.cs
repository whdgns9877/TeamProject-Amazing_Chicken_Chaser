using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Runtime.InteropServices.WindowsRuntime;

public class ChickenSpawn : MonoBehaviourPun, IPunPrefabPool
{

    //=========================================================
    // Singleton
    #region Singeton
    static ChickenSpawn instance = null;

    public static ChickenSpawn Inst
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ChickenSpawn>();
                if (instance == null)
                { instance = new GameObject("ChickenSpawn").AddComponent<ChickenSpawn>(); }
            }
            return instance;
        }
    }
    #endregion
    //=========================================================





    //=========================================================
    //Chicken pooling에 관한 변수

    Queue<GameObject> ChickenList;

    //=========================================================
    void Awake()
    {
        ChickenList = new Queue<GameObject>();
    }


    void Start()
    {
        // only master clinent instantiate chickens 
        if (!PhotonNetwork.IsMasterClient)
            return;

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount - 1; i++)
        {
            PhotonNetwork.Instantiate("Chicken", new Vector3(i + 2, 0, 0), Quaternion.identity);
        }
    }


    public void Destroy(GameObject Chicken)
    {
        Chicken.SetActive(false);
        ChickenList.Enqueue(Chicken);
    }


    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (ChickenList.Count > 0)
        {
            GameObject chicken = ChickenList.Dequeue();
            chicken.transform.position = position + (Vector3.up * 4f);
            chicken.transform.rotation = rotation;
            chicken.SetActive(true);

            return chicken;
        }

        return null;
    }
}
