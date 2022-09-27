using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
            Debug.Log("만들어라 치킨");
        }
    }


    public void Destroy(GameObject gameObject)
    {
        Debug.Log("## Destroy!");

        gameObject.SetActive(false);
        ChickenList.Enqueue(gameObject);
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        Debug.Log("## instantiate!");

        if (ChickenList.Count > 0)
        {
            GameObject chicken = ChickenList.Dequeue();
            chicken.transform.position = position;
            chicken.transform.rotation = rotation;
            chicken.SetActive(true);
            return chicken;
        }

        return Instantiate(prefabId, position, rotation);
    }

}
