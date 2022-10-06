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
        if (PhotonNetwork.IsMasterClient)
        {
            // ���Ӿ� �̵��� �����Ͱ� �������Ѵ�
            ZeraAPIHandler.Inst.BettingZera();
        }
        else
        {
            // ������ Ŭ���̾�Ʈ���� 2�ʵڿ� ���þ��̵� �޾ƿ´�
            Invoke(nameof(GetGameBetID), 2f);
        }

        // ������ �� ����� ���·� ���� ���� ������ ����
        if (PhotonNetwork.IsConnected)
        {
            // ���Ӿ������� ���࿡ �����Ͱ� ġŲ�� ȹ������ ���� ���� ������ �� ���
            // ��� Ŭ���̾�Ʈ���� ���� �̵��ϱ⶧���� �ش� �ɼ��� ����Ѵ�
            PhotonNetwork.AutomaticallySyncScene = false;
            curRoom = PhotonNetwork.CurrentRoom;

            // �÷��̾�� �ڽ��� ���Թ�ȣ�� ���ͳѹ��� ���Ͽ� �ش� ��ġ�� ������ ���� �����Ѵ�
            // ���� ���� �÷��̾� �� ��ŭ �ݺ� 
            for (int i = 0; i < curRoom.PlayerCount; i++)
            {
                // ���� �뿡 �ִ� �÷��̾��� ���ͳѹ��� ���� �÷��̾��� ���� �ѹ��� ���� ��� �÷��̾� ���� 
                if (PhotonNetwork.PlayerList[i].ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    PhotonNetwork.Instantiate($"PlayerCar_{i}", playerSpawnPosArr[i].transform.position, Quaternion.identity);
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
    
    void GetGameBetID()
    {
        Debug.Log("���� �޾ƿ� �� �� ���̵�� : " + (string)curRoom.CustomProperties["gameBetID"]);
        ZeraAPIHandler.Inst.gameBetID = (string)curRoom.CustomProperties["gameBetID"];
    }
}
