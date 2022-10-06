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
        // ������ �� ����� ���·� ���� ���� ������ ����
        if (PhotonNetwork.IsConnected)
        {
            // ���Ӿ������� ���࿡ �����Ͱ� ġŲ�� ȹ������ ���� ���� ������ �� ���
            // ��� Ŭ���̾�Ʈ���� ���� �̵��ϱ⶧���� �ش� �ɼ��� ����Ѵ�
            PhotonNetwork.AutomaticallySyncScene = false;
            curRoom = PhotonNetwork.CurrentRoom;

            // �÷��̾�� �ڽ��� ���Թ�ȣ�� ���ͳѹ��� ���Ͽ� �ش� ��ġ�� ������ ���� �����Ѵ�
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
