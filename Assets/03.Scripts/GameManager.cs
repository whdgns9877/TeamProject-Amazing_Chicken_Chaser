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
            // 게임씬에서는 만약에 마스터가 치킨을 획득하지 못해 방을 떠나야 할 경우
            // 모든 클라이언트들이 같이 이동하기때문에 해당 옵션을 취소한다
            PhotonNetwork.AutomaticallySyncScene = false;
            curRoom = PhotonNetwork.CurrentRoom;

            // 플레이어마다 자신의 슬롯번호와 액터넘버를 비교하여 해당 위치에 본인의 차를 생성한다
            for(int i = 0; i < curRoom.Players.Count; i++)
            {
                if ((int)curRoom.CustomProperties[i.ToString()] == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    PhotonNetwork.Instantiate("Player Car", playerSpawnPosArr[i].transform.position, Quaternion.identity);
                    SoundManager.Inst.StartBGM.Stop();
                    SoundManager.Inst.InGameBGM.Play();
                    break;
                }
            }
        }
    }
}
