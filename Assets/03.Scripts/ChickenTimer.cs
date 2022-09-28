using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;           // TextMeshProUGUI ����� ���� using
using UnityEngine.UI;  // UI ����� ���� using
using Photon.Pun;      // ���� ���̺귯���� ����Ƽ ������Ʈ�� ����� �� �ְ� �ϴ� ���̺귯��  
using Photon.Realtime; // ������ �ǽð� ��Ʈ��ũ ���� ���߿� C# ���̺귯��
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.UIElements;

// ExitGames �� ������ ���� ȸ���ε� ���⿡�� ���� Hashtable�� ����Ϸ��µ�
// ����Ƽ���� �⺻������ �����ϴ� Hashtable�� �ƴ� ���濡�� �����ϴ� Hashtable�� ����Ѵ�


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



    // ���� �ڽ��� �����ִ� ��
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
