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
    [SerializeField] GameObject     Panel_Login         = null;  // 로그인 패널 -> 나중에 쓰일수도있으니 남겨둠
    [SerializeField] GameObject     Panel_Lobby             = null;  // 로비 패널
    [SerializeField] GameObject     CreateRoomPanel         = null;  // 방을 생성하는데 쓰일 패널
    [SerializeField] Button         CreateRoomPanelButton   = null;  // 방생성 판넬을 띄워줄 버튼
    [SerializeField] GameObject     room                    = null;  // 방정보에 따라 만들어줄 방 프리팹
    [SerializeField] Transform      Content_Room            = null;  // 방 생성시 스크롤뷰에 넣어줄 방의 위치(Vertical Layout Group 사용으로 알맞게 들어가게 할것임)
    [SerializeField] Button         CreateRoomButton        = null;  // 방을 생성하는 버튼
    [SerializeField] TMP_InputField InputField_RoomName     = null;  // 방 이름을 받을 인풋필드

    [Header("** 방 UI **")]
    [SerializeField] TextMeshProUGUI   _roomNameText    = null;    // 방 이름
    [SerializeField] GameObject        Panel_Room       = null;    // 방의 전체적인 패널
    [SerializeField] TextMeshProUGUI[] PlayerNickNames  = null;    // 플레이어들의 닉네임을 담을 배열
    [SerializeField] GameObject        Button_StartGame = null;    // 게임 시작 버튼

    // ------------------------- 임시 참조 변수
    [SerializeField] GameObject Button_ImsiStartGame = null;    // 임시 게임 시작 버튼

    [SerializeField] TextMeshProUGUI Text_ConnectionInfo = null;  // 현재 네트워크 상태 메세지를 나타낼 TextMeshPro

    string ninkNameInputField;
    // 방 타이틀에 쓰일 string
    private string roomNameText;

    // 방 정보를 담을 리스트
    List<RoomInfo> _roomList = new List<RoomInfo>();

    // 현재 자신이 속해있는 방
    private Room curRoom;

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.AutomaticallySyncScene = true;
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

    // 방 생성할때 입력 컨트롤
    public void OnvalueChangedCreateRoom(string inStr)
    {
        if (string.IsNullOrEmpty(inStr))
            CreateRoomButton.interactable = false;
        else
            CreateRoomButton.interactable = true;

        roomNameText = inStr;
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
        // 로비 접속시 우선 방 정보를 지워줌
        _roomList.Clear();
    }

    // 접속 끊기 버튼에 쓰일 함수
    public void OnClick_DisConnect()
    {
        Text_ConnectionInfo.text = "연결 끊어짐 ...";
        PhotonNetwork.Disconnect();
    }

    // 방 정보가 바뀔때 자동으로 호출되는 함수
    // 로비에 접속 시
    // 새로운 룸이 만들어질 경우
    // 룸이 삭제되는 경우
    // 룸의 IsOpen 값이 변화할 경우
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 방의 정보가 바뀌어 이 콜백함수가 실행되면
        // 원래 있던 방들을 전부 없애주고
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOM"))
        {
            Destroy(obj);
        }

        // roomList 리스트의 정보들을 확인한다
        foreach (RoomInfo roomInfo in roomList)
        {
            // 방 정보의 isVisible이 false 이거나 리스트에서 제거된 정보(플레이어가 아무도 없어서) 라면
            if (!roomInfo.IsVisible || roomInfo.RemovedFromList)
            {
                // 방 목록에서 제거한다
                if (_roomList.IndexOf(roomInfo) != -1)
                    _roomList.RemoveAt(_roomList.IndexOf(roomInfo));
            }
            else
            {
                // 위의 상황이 아니면 방리스트에 새로 넣어준다
                if (!_roomList.Contains(roomInfo)) _roomList.Add(roomInfo);
                else _roomList[_roomList.IndexOf(roomInfo)] = roomInfo;
            }
        }

        // 위의 조건을 돌고 리스트에 있는 방들을 만들어준다
        foreach (RoomInfo roomInfo in _roomList)
        {
            GameObject _room = Instantiate(room, Content_Room);
            RoomData roomData = _room.GetComponent<RoomData>();
            roomData.roomName = roomInfo.Name;
            roomData.maxPlayer = roomInfo.MaxPlayers;
            roomData.playerCount = roomInfo.PlayerCount;
            roomData.isOpen = roomInfo.IsOpen;
            roomData.UpdateInfo();

            // 해당 방의 인원이 꽉차있으면 버튼클릭을 막아 접속할수 없게한다
            if (roomData.playerCount == roomData.maxPlayer)
                _room.GetComponent<Button>().interactable = false;

            // 방이 닫혀있으면 못들어오게 버튼 클릭을 막고
            if (roomData.isOpen == false)
                _room.GetComponent<Button>().interactable = false;
            else
            {
                // 방이 열려있으면
                // delegate로 내용을 참조하여 클릭했을때 방에 참가할 수 있도록 처리
                roomData.GetComponent<Button>().onClick.AddListener
                (
                    delegate
                    {
                        roomNameText = roomData.roomName;
                        // 이 부분이 실제로 방에 참가하는 부분
                        PhotonNetwork.JoinRoom(roomData.roomName, null);
                    }
                );
            }
        }
    }

    // 방 생성패널 버튼에 달아줄 함수
    public void OnCreateRoomInfoButtonClicked()
    {
        // 방 정보를 입력받을 판넬을 활성화 시키고
        CreateRoomPanel.SetActive(true);
        // 중복해서 계속 버튼이 눌리지 않게 생성 버튼은 클릭을 막는다
        CreateRoomButton.GetComponent<Button>().interactable = false;
    }

    // 방 생성 버튼에 달 함수
    public void OnCreateRoomButtonClicked()
    {
        // 방을 만들었으니 생성버튼을 다시 사용할수 있게 클릭을 풀어주고
        CreateRoomPanelButton.GetComponent<Button>().interactable = true;

        // 방의 정보를 담고있는 클래스
        RoomOptions ro = new RoomOptions();
        ro.IsVisible = true;                        // 방이 보이게
        ro.IsOpen = true;                           // 방을 열고
        ro.MaxPlayers = 8;                          // 최대 인원수는 8
        ro.CleanupCacheOnLeave = true;

        PhotonNetwork.CreateRoom(roomNameText, ro); // 실제로 방을 만드는 함수
    }

    // 방에 참가하면 자동적으로 호출되는 콜백함수
    public override void OnJoinedRoom()
    {
        curRoom = PhotonNetwork.CurrentRoom;

        _roomNameText.text = "방 : " + curRoom.Name;

        Panel_Login.SetActive(false);
        Panel_Lobby.SetActive(false);
        Panel_Room.SetActive(true);

        if(PhotonNetwork.IsMasterClient)
        {

        }
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

        if (!CreateRoomPanel.activeInHierarchy)
            InputField_RoomName.text = null;
    }


    //------------------------------- 임시로 만든 입장 함수---------------------------------------------------------
    public void ImsiGameStart()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}
