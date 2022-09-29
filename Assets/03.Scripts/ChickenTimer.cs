using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;           // TextMeshProUGUI 사용을 위해 using
using UnityEngine.UI;  // UI 사용을 위해 using
using Photon.Pun;      // 포톤 라이브러리를 유니티 컴포넌트로 사용할 수 있게 하는 라이브러리  
using Photon.Realtime; // 포톤의 실시간 네트워크 게임 개발용 C# 라이브러리
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.UIElements;

// ExitGames 가 포톤을 만든 회사인데 여기에서 만든 Hashtable을 사용하려는데
// 유니티에서 기본적으로 제공하는 Hashtable이 아닌 포톤에서 제공하는 Hashtable을 사용한다


public class ChickenTimer : MonoBehaviourPunCallbacks
{
    //=========================================================
    // Singleton
    #region Singeton
    static ChickenTimer instance = null;

    public static ChickenTimer Inst
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ChickenTimer>();
                if (instance == null)
                { instance = new GameObject("ChickenTimer").AddComponent<ChickenTimer>(); }
            }
            return instance;
        }
    }
    #endregion
    //=========================================================


    [SerializeField] TextMeshProUGUI timer = null;      // text for timer 

    double startTime;            // start time
    double timepassed;

    bool gameStart = false;
    public bool GameStart { get { return gameStart; } }

    bool isGameOver = true;
    public bool IsGameOver { get { return isGameOver; } }   



    // 현재 자신이 속해있는 방
    private Room curRoom = null;

    private void Awake()
    {
        curRoom = PhotonNetwork.CurrentRoom;
    }


    void Start()
    {
        StartCoroutine(CountDonw());
    }



    void setTime()
    {
        // set startTime of clients as hashtable value 
        // convert hashtable value => string => double 
        startTime = (double)(curRoom.CustomProperties["StartTime"]);
    }


    IEnumerator CountDonw()
    {
        timer.text = "Player Ready!!";
        yield return new WaitForSeconds(2f);

        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable CustomValue = new Hashtable();
            startTime = PhotonNetwork.Time;             // current game start time
            CustomValue.Add("StartTime", startTime);    // set hashtable as current game start time
            curRoom.SetCustomProperties(CustomValue); // set custom properties
        }

        timer.text = "Start!!";
        Invoke("setTime", 1f);
        yield return new WaitForSeconds(1f);
        
        // is game over? 
        while (isGameOver)
        {
            // is game started? 
            gameStart = true;

            double timeLimit = 32f;

            // curret time - game start time 
            timepassed = PhotonNetwork.Time - startTime;

            timeLimit -= timepassed;

            timer.text = $" Chicken Time : {(int)timeLimit}";

            if (timeLimit <= 0)
            {
                isGameOver = false;
                gameStart = false;

            }
            yield return null;
        }
    }
}
