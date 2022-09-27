using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using ExitGames.Client.Photon;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviourPun
{

    void Awake()
    {
        // ������ �� ����� ���·� ���� ���� ������ ����
        if (PhotonNetwork.IsConnected)
        {
            // ������ ������ġ�� Ŭ���̾�Ʈ�� ������ġ�� ���Ƿ� ��������
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Instantiate("Player Car", new Vector3(-3, 0, 0), Quaternion.identity);
            else
                PhotonNetwork.Instantiate("Player Car", new Vector3(3, 0, -3), Quaternion.identity);
        }

    }

}
