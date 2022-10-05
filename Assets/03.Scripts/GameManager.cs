using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using ExitGames.Client.Photon;
using UnityEngine.UIElements;
using System;
using Photon.Pun.Demo.Cockpit;

public class GameManager : MonoBehaviourPun
{
    [SerializeField] Transform[] playerSpawnPosArr;
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


    public float GameTime;

    private Room curRoom;


    bool doAgain = false;
    bool checkChicken = false;
    public bool CheckChicken { get { return checkChicken; } }

    public bool DoAgain { get { return doAgain; } }


    void Awake()
    {
        // 서버에 잘 연결된 상태로 게임 씬에 들어오면 실행
        if (PhotonNetwork.IsConnected)
        {
            // 게임씬에서는 만약에 마스터가 치킨을 획득하지 못해 방을 떠나야 할 경우
            // 모든 클라이언트들이 같이 이동하기때문에 해당 옵션을 취소한다
            PhotonNetwork.AutomaticallySyncScene = false;
            curRoom = PhotonNetwork.CurrentRoom;

            // 플레이어마다 자신의 슬롯번호와 액터넘버를 비교하여 해당 위치에 본인의 차를 생성한다
            // 현재 방의 플레이어 수 만큼 반복 
            for (int i = 0; i < curRoom.PlayerCount; i++)
            {
                // 현재 룸에 있는 플레이어의 액터넘버와 로컬 플레이어의 액터 넘버가 같을 경우 플레이어 생성 
                if (PhotonNetwork.PlayerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    PhotonNetwork.Instantiate("Player Car", playerSpawnPosArr[i].transform.position, Quaternion.identity);
                    break;
                }
            }
        }
    }




    private void Update()
    {
        //if game is over 
        if (!ChickenTimer.Inst.IsGameOver && !checkChicken)
        {
            // count chicken in Queue, 
            if (ChickenSpawn.Inst.MyChickenLiet.Count == 0)
            {
                Debug.Log("No one has chicek!");
                doAgain = true;
            }

            
            checkChicken = true;
        }

    }
}
