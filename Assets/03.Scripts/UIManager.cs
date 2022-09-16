using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;           // TextMeshProUGUI 사용을 위해 using
using UnityEngine.UI;  // UI 사용을 위해 using
using Photon.Pun;      // 포톤 라이브러리를 유니티 컴포넌트로 사용할 수 있게 하는 라이브러리  
using Photon.Realtime; // 포톤의 실시간 네트워크 게임 개발용 C# 라이브러리
using HashTable = ExitGames.Client.Photon.Hashtable;
// ExitGames 가 포톤을 만든 회사인데 여기에서 만든 Hashtable을 사용하려는데
// 유니티에서 기본적으로 제공하는 HashTable과 이름이 겹치므로 포톤의 HashTable을 할당하여 이를 사용

public class UIManager : MonoBehaviourPunCallbacks
{
    [Header("** 로그인 UI **")]
    [SerializeField] TMP_InputField InputField_NickName = null;  // 닉네임을 입력받을 InputField
    [SerializeField] Button         Button_JoinLobby    = null;  // 로비 접속 버튼

    [Header("** 로비 UI **")]
    //[SerializeField] GameObject      Panel_Login         = null;  // 로그인 패널 -> 나중에 쓰일수도있으니 남겨둠
    [SerializeField] GameObject Panel_Lobby  = null;  // 로비 패널
    [SerializeField] GameObject Content_Room = null;  // 방 생성시 스크롤뷰에 넣어줄 방의 위치(Vertical Layout Group 사용으로 알맞게 들어가게 할것임)

    [Header("** 방 UI **")]
    [SerializeField] GameObject        Panel_Room      = null;    // 방의 전체적인 패널
    [SerializeField] TextMeshProUGUI[] PlayerNickNames = null;    // 플레이어들의 닉네임을 담을 배열

    [SerializeField] TextMeshProUGUI Text_ConnectionInfo = null;  // 현재 네트워크 상태 메세지를 나타낼 TextMeshPro

    string ninkNameInputField;

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
    }

    // Start is called before the first frame update
    void Start()
    {
        // 버튼의 상호작용을 막아놓는다
        Button_JoinLobby.interactable = false;

        // 마스터서버 접속 요청
        PhotonNetwork.ConnectUsingSettings();
    }

    // 마스터 서버 접속 성공시에 호출
    public override void OnConnectedToMaster()
    {
        Text_ConnectionInfo.text = "마스터 서버에 연결 완료!";
    }

    // 마스터 서버와 연결이 끊어졌을 때 호출
    public override void OnDisconnected(DisconnectCause cause)
    {
        Text_ConnectionInfo.text = "마스터 서버에서 끊어짐...";
        Panel_Lobby.SetActive(false);
        InputField_NickName.interactable = true;
        Button_JoinLobby.interactable = true;

        // 임의로 본인이 접속을 끊든 서버 불안정으로 끊기든
        // 끊어졌을때 자동으로 마스터 서버로 다시 접속하게 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 이름 입력 컨트롤(Input Field)
    public void OnValueChanged(string inStr)
    {
        if (string.IsNullOrEmpty(inStr))
            Button_JoinLobby.interactable = false;
        else
            Button_JoinLobby.interactable = true;

        PhotonNetwork.LocalPlayer.NickName = inStr;
        ninkNameInputField = inStr;
    }

    // 로비 연결 버튼에 쓰일 함수
    public void OnClick_JoinLobby()
    {
        // 아직 네트워크가 마스터 서버에 연결된 상태가 아니라면 처리하지 않게
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer || string.IsNullOrEmpty(ninkNameInputField))
            return;

        Text_ConnectionInfo.text = "접속 시도 중 ...";
        InputField_NickName.interactable = false;
        Button_JoinLobby.interactable = false;
        PhotonNetwork.JoinLobby();
    }

    // 로비에 연결 완료시 호출되는 함수
    public override void OnJoinedLobby()
    {
        Text_ConnectionInfo.text = "로비 접속 완료!";
        // 로비에 접속 완료시 로그인 패널은 비활성화 로비 패널은 활성화 해준다
        Panel_Lobby.SetActive(true);
        InputField_NickName.interactable = false;
    }

    // 접속 끊기 버튼에 쓰일 함수
    public void OnClick_DisConnect()
    {
        Text_ConnectionInfo.text = "연결 끊어짐 ...";
        PhotonNetwork.Disconnect();
    }

    private void Update()
    {
        // 네트워크 상태가 마스터 서버에 연결되어있지 않다면
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
        {
            // 두 UI의 상호작용을 막는다
            InputField_NickName.interactable = false;
            Button_JoinLobby.interactable = false;
        }
        // 네트워크 상태가 마스터 서버에 연결된 상태라면
        else
        {
            // InputField에 입력한 텍스트가 비워져있으면
            if(string.IsNullOrEmpty(ninkNameInputField))
                // 접속 Button의 상호작용을 막는다
                Button_JoinLobby.interactable = false;
            // InputField에 입력한 텍스트가 무엇인가 존재한다면(입력한게 있다면)
            else
                // 접속 Button의 상호작용을 활성화 시킨다
                Button_JoinLobby.interactable = true;

            // InputField는 상호작용 가능하게
            InputField_NickName.interactable = true;
        }
    }
}
