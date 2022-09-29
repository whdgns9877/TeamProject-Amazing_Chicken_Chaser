using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using ExitGames.Client.Photon;
using UnityEngine.UIElements;
using System;

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

    void Awake()
    {
        // 서버에 잘 연결된 상태로 게임 씬에 들어오면 실행
        if (PhotonNetwork.IsConnected)
        {
            curRoom = PhotonNetwork.CurrentRoom;
            //// 방장의 시작위치와 클라이언트의 시작위치를 임의로 나눠놓음
            //if (PhotonNetwork.IsMasterClient)
            //    PhotonNetwork.Instantiate("Player Car", new Vector3(-3, 0, 0), Quaternion.identity);
            //else
            //    PhotonNetwork.Instantiate("Player Car", new Vector3(3, 0, -3), Quaternion.identity);

            for(int i = 0; i < curRoom.Players.Count; i++)
            {
                if ((int)curRoom.CustomProperties[i.ToString()] == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    PhotonNetwork.Instantiate("Player Car", playerSpawnPosArr[i].transform.position, Quaternion.identity);
                    break;
                }
            }
        }
    }




}
