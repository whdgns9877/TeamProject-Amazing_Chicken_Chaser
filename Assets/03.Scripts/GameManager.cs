using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using ExitGames.Client.Photon;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviourPun, IPunObservable, IPunPrefabPool
{

    //=========================================================
    //singleton
    #region Singleton_Not used
    static GameManager instance = null;

    public static GameManager Inst
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    instance = new GameObject("GameManager").AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }
    #endregion 
    //=========================================================






    //=========================================================
    // Chicken pooling에 관한 변수

    public GameObject ChickenPrefab;

    GameObject Chicken = null;
    Queue<GameObject> ChickenList = new Queue<GameObject>();
    Animator ChickenAni = new Animator();


    //=========================================================

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
        
        }

        else
        {
            
        }
    }



    void Awake()
    {
        // 서버에 잘 연결된 상태로 게임 씬에 들어오면 실행
        if (PhotonNetwork.IsConnected)
        {
            // 방장의 시작위치와 클라이언트의 시작위치를 임의로 나눠놓음
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Instantiate("Player Car", new Vector3(-3, 0, 0), Quaternion.identity);
            else
                PhotonNetwork.Instantiate("Player Car", new Vector3(3, 0, -3), Quaternion.identity);
        }
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        for (int i = 0; i < 4; i++)
        {
            Chicken = PhotonNetwork.Instantiate("Chicken", new Vector3(i + 2, 0, 0), Quaternion.identity);
            ChickenList.Enqueue(Chicken);
        }
    }

    //========================================================================================
    // IPunPrefabPool
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

        if (!PhotonNetwork.IsMasterClient)
            return null; 

        return Instantiate(ChickenPrefab, position, rotation);
    }

    public void Destroy(GameObject gameObject)
    {
        Debug.Log("## Destroy!");

        gameObject.SetActive(false);
        ChickenList.Enqueue(gameObject);
    }
    //========================================================================================


}
