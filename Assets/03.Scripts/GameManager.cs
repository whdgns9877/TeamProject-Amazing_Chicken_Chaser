using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    void Awake()
    {
        // 서버에 잘 연결된 상태로 게임 씬에 들어오면 실행
        if (PhotonNetwork.IsConnected)
        {
            // 방장의 시작위치와 클라이언트의 시작위치를 임의로 나눠놓음
            if(PhotonNetwork.IsMasterClient)
                PhotonNetwork.Instantiate("Player Car", new Vector3(-3,0,0), Quaternion.identity);
            else
                PhotonNetwork.Instantiate("Player Car", new Vector3(3,0,-3), Quaternion.identity);
        }
    }

    //void Update()
    //{
        
    //}
}
