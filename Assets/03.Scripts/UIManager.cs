using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;           // TextMeshProUGUI 사용을 위해 using
using UnityEngine.UI;  // UI 사용을 위해 using
using Photon.Pun;      // 포톤 라이브러리를 유니티 컴포넌트로 사용할 수 있게 하는 라이브러리  
using Photon.Realtime; // 포톤의 실시간 네트워크 게임 개발용 C# 라이브러리
using Hashtable = ExitGames.Client.Photon.Hashtable;
// ExitGames 가 포톤을 만든 회사인데 여기에서 만든 Hashtable을 사용하려는데
// 유니티에서 기본적으로 제공하는 Hashtable이 아닌 포톤에서 제공하는 Hashtable을 사용한다

public class UIManager : MonoBehaviourPunCallbacks
{
    #region UI참조들
    [SerializeField] TextMeshProUGUI Text_ConnectionInfo = null;  // 현재 네트워크 상태 메세지를 나타낼 TextMeshPro
    [SerializeField] GameObject      Panel_Notice        = null;  // 현재 알림 상태를 띄울 패널

    [Header("** 로그인 UI **")]
    [SerializeField] TMP_InputField InputField_NickName = null;  // 닉네임을 입력받을 InputField
    [SerializeField] Button         Button_JoinLobby    = null;  // 로비 접속 버튼

    [Header("** 로비 UI **")]
    [SerializeField] GameObject      Panel_Login             = null;  // 로그인 패널
    [SerializeField] GameObject      Panel_Lobby             = null;  // 로비 패널
    [SerializeField] GameObject      Panel_CreateRoom        = null;  // 방을 생성하는데 쓰일 패널
    [SerializeField] Button          Button_CreateRoomPanel  = null;  // 방생성 판넬을 띄워줄 버튼
    [SerializeField] Transform       Tr_Content_Room         = null;  // 방 생성시 스크롤뷰에 넣어줄 방의 위치(Vertical Layout Group 사용으로 알맞게 들어가게 할것임)
    [SerializeField] Button          Button_CreateRoom       = null;  // 방을 생성하는 버튼
    [SerializeField] TMP_InputField  InputField_RoomName     = null;  // 방 이름을 받을 인풋필드
    [SerializeField] GameObject      room                    = null;  // 방정보에 따라 만들어줄 방 프리팹
    [SerializeField] Toggle[]        togglesForMaxPlayer     = null;  // 최대 플레이어를 정해줄 토글들
    [SerializeField] TextMeshProUGUI Text_MyCost             = null;  // 나의 코스트

    [Header("** 방 UI **")]
    [SerializeField] TextMeshProUGUI   Text_roomName           = null;    // 방 이름
    [SerializeField] TextMeshProUGUI   Text_RoomCost           = null;    // 방 코스트
    [SerializeField] TextMeshProUGUI   Text_MyCostInRoom       = null;    // 방 안에서의 나의 코스트
    [SerializeField] GameObject        Panel_Room              = null;    // 방의 전체적인 패널
    [SerializeField] GameObject        Button_StartGame        = null;    // 게임 시작 버튼
    [SerializeField] GameObject        Button_Ready            = null;    // 게임 레디 버튼
    [SerializeField] GameObject[]      Panel_PlayerSlot        = null;    // 플레이어들이 들어올수 있는 슬롯


    [SerializeField] GameObject        Panel_ChangeUIEffect    = null;    // 방 입장,퇴장시 효과를 나타낼 패널
    #endregion

    #region 방에서 쓰일 변수들
    // 방 정보를 담을 리스트
    List<RoomInfo> _roomList = new List<RoomInfo>();

    // 방 입장과 게임에 쓰일 코스트
    private int myCost = 0;

    // 방 타이틀에 쓰일 string
    private string roomNameText = "";

    // RoomOption의 maxPlayer가 byte타입이라서 byte타입
    private byte myRoomMaxPlayer = 0;

    // 방정보를 갱신하는 시간을 0.2초로
    private WaitForSeconds delayUpdateTime = new WaitForSeconds(0.2f);

    // 장면 전화 효과에 쓰일 시간
    private WaitForSeconds uiUpdateTime = new WaitForSeconds(0.05f);
    // 자기 자신의 레디상태
    private bool ready = false;

    private bool leftRoomDone = true;

    private bool enterRoomDone = true;

    private bool createRoomDone = true;

    private string myRoomName = null;

    RoomOptions myRo = null;

    // 현재 자신이 속해있는 방
    private Room curRoom = null;
    #endregion

    #region 입장 정보
    // 닉네임에 쓰일 InputField
    private string nickNameInputField = "";

    // 로그인 상태 변수
    private bool isLogin = false;
    #endregion

    private void Awake()
    {
        // 초기 화면 세팅
        Screen.SetResolution(960, 540, false);

        // 전송률 설정
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        Invoke("DelaySync", 2f);
        // DAPPX를 위한 Cost
        myCost = 10000;
        // 시작 BGM 재생
        SoundManager.Inst.InGameBGM.Stop();
        SoundManager.Inst.StartBGM.Play();
    }

    private void DelaySync()
    {
        // 방장(마스터 클라이언트)가 게임씬으로 이동할때 클라이언트들도 같이 이동
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        // 버튼의 상호작용을 막아놓는다
        Button_JoinLobby.interactable = false;

        // 마스터서버 접속 요청
        PhotonNetwork.ConnectUsingSettings();
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
            if (string.IsNullOrEmpty(nickNameInputField))
                // 접속 Button의 상호작용을 막는다
                Button_JoinLobby.interactable = false;
            // InputField에 입력한 텍스트가 무엇인가 존재한다면(입력한게 있다면)
            else
                // 접속 Button의 상호작용을 활성화 시킨다
                Button_JoinLobby.interactable = true;

            // InputField는 상호작용 가능하게
            InputField_NickName.interactable = true;
        }

        if (!Panel_CreateRoom.activeInHierarchy)
            InputField_RoomName.text = null;
    }
  
    #region 콜백 함수들
    // 마스터 서버 접속 성공시에 호출
    public override void OnConnectedToMaster()
    {
        Text_ConnectionInfo.text = "마스터 서버에 연결 완료!";
        // 로그인 되어있는 상태로 마스터 서버에 연결되면 자동적으로 로비에 입장
        if (isLogin) PhotonNetwork.JoinLobby();
    }

    // 마스터 서버와 연결이 끊어졌을 때 호출
    public override void OnDisconnected(DisconnectCause cause)
    {
        Text_ConnectionInfo.text = "마스터 서버에서 끊어짐...";
        // 접속이 끊어진 상태에서는 로비 패널을 비활성화하고
        if (Panel_Lobby.activeInHierarchy)
            Panel_Lobby.SetActive(false);
        // 닉네임을 입력받는 InputField와 입장 Button을 활성화 시켜준다
        InputField_NickName.interactable = true;
        Button_JoinLobby.interactable = true;

        // 임의로 본인이 접속을 끊든 서버 불안정으로 끊기든
        // 끊어졌을때 자동으로 마스터 서버로 다시 접속하게 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 로비에 연결 완료시 호출되는 함수
    public override void OnJoinedLobby()
    {
        // 로비에서 표시할 텍스트 들을 띄워줌
        Text_ConnectionInfo.text = "로비 접속 완료!";
        Text_MyCost.text = "MyCost : " + myCost.ToString();
        // 로비에 접속 완료시 로그인 패널은 비활성화 로비 패널은 활성화 해준다
        Panel_Lobby.SetActive(true);
        InputField_NickName.interactable = false;
        // 로비 접속시 우선 방 정보를 지워줌
        _roomList.Clear();
    }

    // 방 정보가 바뀔때 자동으로 호출되는 함수
    // 로비에 접속 시(마스터 서버 -> 로비)
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
            GameObject _room = Instantiate(room, Tr_Content_Room);
            RoomData roomData = _room.GetComponent<RoomData>();
            roomData.roomName = roomInfo.Name;
            roomData.maxPlayer = roomInfo.MaxPlayers;
            roomData.playerCount = roomInfo.PlayerCount;
            roomData.isOpen = roomInfo.IsOpen;
            roomData.roomCost = roomInfo.MaxPlayers * 1000;
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
                        enterRoomDone = false;
                        StartCoroutine(ChangeUIProcess());
                        myRoomName = roomData.roomName;
                    }
                );
            }
        }
    }

    // 플레이어가 방을 나갔을때 해당 콜백함수가 실행
    public override void OnLeftRoom()
    {
        leftRoomDone = true;
        ResetMyRoom();
        //// UI들을 상황에 맞게 처리
        //Panel_Room.SetActive(false);
        //Panel_Login.SetActive(true);
        //Panel_Lobby.SetActive(true);
    }

    // 방에 참가하면 자동적으로 호출되는 콜백함수
    public override void OnJoinedRoom()
    {
        myRoomName = null;
        myRo = null;

        if(enterRoomDone == false)
            enterRoomDone = true;

        if (createRoomDone == false)
            createRoomDone = true;

        // 현재 방에 달려있는 태그를 Hashtable 형식인 curRoomProperties 라는 변수에 넣어준다
        curRoom = PhotonNetwork.CurrentRoom;
        // 현재 있는 방의 UI 의 Text에 현재 방의 text를 넣어준다
        Text_roomName.text = "방 : " + curRoom.Name;

        if (PhotonNetwork.IsMasterClient)
        {
            // 인덱스가 0 부터 시작하므로 -1
            int max = curRoom.MaxPlayers - 1;
            //// 방장이 처음에 방을 파게되면 방의 초기 슬롯들의 설정을 해준다
            curRoom.SetCustomProperties(new Hashtable
            {
                // 방장(호스트)는 0에 본인 번호, 참여가능 슬롯은0, 참여불가능 슬롯은 -1
                // 해당 슬롯의 인덱스와 방옵션의 MaxPlayer를 비교하여 MaxPlayer가 넘어가는 슬롯은 닫아준다
                {"0", PhotonNetwork.LocalPlayer.ActorNumber }, {"1", 0 },
                {"2", 2 <= max ? 0 : -1 }, {"3", 3 <= max ? 0 : -1 }, {"4", 4 <= max ? 0 : -1 },
                {"5", 5 <= max ? 0 : -1 }, {"RoomCost", curRoom.MaxPlayers*1000}
            });
        }
        else
        {
            // 방에 참가한 사람은 참여가능 슬롯이라면 자기번호를
            for (int i = 0; i < 6; i++)
            {
                if (GetRoomTag(i) == 0)
                {
                    SetRoomTag(i, PhotonNetwork.LocalPlayer.ActorNumber);
                    break;
                }
            }
        }

        // 방에 참가하면 준비상태를 false로
        SetLocalTag("IsReady", false);

        myCost -= 1000;
        Text_MyCostInRoom.text = "MyCost : " + myCost.ToString();

        //// 패널들의 UI들을 맞게 처리해준다
        //Panel_Login.SetActive(false);
        //Panel_Lobby.SetActive(false);
        //Panel_Room.SetActive(true);

        // 현재 방의 태그 값들을 갱신해준다
        StartCoroutine(RoomUpdate());
    }

    #endregion

    #region UI에 등록하는 함수들
    // 이름 입력 컨트롤(Input Field)
    public void OnValueChanged(string inStr)
    {
        if (string.IsNullOrEmpty(inStr))
            Button_JoinLobby.interactable = false;
        else
            Button_JoinLobby.interactable = true;

        PhotonNetwork.LocalPlayer.NickName = inStr;
        nickNameInputField = inStr;
    }

    // 방 생성할때 입력 컨트롤
    public void OnvalueChangedCreateRoom(string inStr)
    {
        if (string.IsNullOrEmpty(inStr))
            Button_CreateRoom.interactable = false;
        else
            Button_CreateRoom.interactable = true;

        roomNameText = inStr;
    }

    // 로비 연결 버튼에 쓰일 함수
    public void OnClick_JoinLobby()
    {
        SoundManager.Inst.ClickSound.Play();
        // 아직 네트워크가 마스터 서버에 연결된 상태가 아니라면 처리하지 않게
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer || string.IsNullOrEmpty(nickNameInputField))
            return;

        Text_ConnectionInfo.text = "접속 시도 중 ...";
        InputField_NickName.interactable = false;
        Button_JoinLobby.interactable = false;
        PhotonNetwork.JoinLobby();
        isLogin = true;
    }

    // 접속 끊기 버튼에 쓰일 함수
    public void OnClick_DisConnect()
    {
        SoundManager.Inst.ClickSound.Play();
        Text_ConnectionInfo.text = "연결 끊어짐 ...";
        PhotonNetwork.Disconnect();
        isLogin = false;
    }

    // 방 생성패널 버튼에 달아줄 함수
    public void OnCreateRoomInfoButtonClicked()
    {
        SoundManager.Inst.ClickSound.Play();
        // 방 정보를 입력받을 판넬을 활성화 시키고
        Panel_CreateRoom.SetActive(true);
        // 중복해서 계속 버튼이 눌리지 않게 생성 버튼은 클릭을 막는다
        Button_CreateRoom.GetComponent<Button>().interactable = false;
    }

    // 방을 생성할때 최대 플레이어수를 결정해주는 함수
    public void CheckToggleValue()
    {
        for (int i = 0; i < togglesForMaxPlayer.Length; i++)
        {
            if (togglesForMaxPlayer[i].isOn == true)
            {
                myRoomMaxPlayer = (byte)(i + 2);
                break;
            }
        }
    }
    // 방 생성 버튼에 달 함수
    public void OnCreateRoomButtonClicked()
    {
        SoundManager.Inst.ClickSound.Play();
        // 방을 만들었으니 생성버튼을 다시 사용할수 있게 클릭을 풀어주고
        Button_CreateRoomPanel.GetComponent<Button>().interactable = true;

        // 방의 정보를 담고있는 클래스
        RoomOptions ro = new RoomOptions();
        ro.IsVisible = true;                        // 방이 보이게
        ro.IsOpen = true;                           // 방을 열고
        CheckToggleValue();                         // 현재 어느 토글이 선택되어있는지 확인하고
        ro.MaxPlayers = myRoomMaxPlayer;            // 최대 인원수를 체크한 토글값에 의한 값을 넣는다
        // 클라이언트들이 방을 나갈때
        // 해당 클라이언트의 Properties를 비워준다
        ro.CleanupCacheOnLeave = true;

        createRoomDone = false;
        myRoomName = roomNameText;
        myRo = ro;
        StartCoroutine(ChangeUIProcess());
        //PhotonNetwork.CreateRoom(roomNameText, ro); // 실제로 방을 만드는 함수
    }

    public void OnLeaveRoomButtonClicked()
    {
        SoundManager.Inst.ClickSound.Play();
        leftRoomDone = false;
        StartCoroutine(ChangeUIProcess());
        myCost += 1000;
        Text_MyCost.text = myCost.ToString();
    }

    // 준비 버튼을 눌렀을때 실행되는 함수
    public void OnClick_ReadyButton()
    {
        SoundManager.Inst.ClickSound.Play();
        if (ready == true)
            ready = false;
        else
            ready = true;
        SetLocalTag("IsReady", ready);
    }

    // 시작 버튼을 눌렀을때 실행되는 함수
    public void OnClick_StartGame()
    {
        if (CheckPlayersReady())
        {
            SoundManager.Inst.ClickSound.Play();
            curRoom.IsOpen = false;
            PhotonNetwork.LoadLevel("GameScene");
        }
        else
        {
            SoundManager.Inst.ClickNagative.Play();
            Panel_Notice.GetComponentInChildren<TextMeshProUGUI>().text = "아직 모든 플레이어가 준비 되지 않았습니다";
            Panel_Notice.SetActive(true);
        }
    }

    #endregion

    #region 코루틴 함수들

    // 방안의 정보를 갱신하는 코루틴
    IEnumerator RoomUpdate()
    {
        // 방에 입장한 상태라면 계속 체크한다
        while (PhotonNetwork.InRoom)
        {
            yield return delayUpdateTime;
            // 업데이트 도중 방에서 나가게되면 방정보 갱신을 멈춘다
            if (!PhotonNetwork.InRoom) yield break;

            Text_RoomCost.text = "방 베팅 금액 " + ((int)curRoom.CustomProperties["RoomCost"]).ToString();

            // 방장이 바뀌었을때 바뀐플레이어가 방장이면 해당 플레이어의 게임시작버튼이 활성화 된다
            if (PhotonNetwork.IsMasterClient)
            {
                Button_Ready.SetActive(false);
                Button_StartGame.SetActive(true);
                SetLocalTag("IsReady", false);
            }
            else
            {
                Button_Ready.SetActive(true);
                Button_StartGame.SetActive(false);
            }

            // 플레이어들 레디 조건 설정
            for (int i = 0; i < 6; i++)
            {
                // 방장(마스터클라이언트)이 방설정을 해준다
                if (PhotonNetwork.IsMasterClient)
                {
                    // 슬롯이 열려있는데 사람은 비어있으면 해당 슬롯에 0을 대입
                    if (GetPlayer(i) == null && GetRoomTag(i) > 0) SetRoomTag(i, 0);
                }

                // 방 태그를 줄때 열리고 닫히고를 0, -1로 태그값을 줬으므로 이를 통해 판단
                if (GetRoomTag(i) == -1)
                {
                    // 슬롯이 닫혔다는 UI 이미지를 띄워준다
                    Panel_PlayerSlot[i].transform.GetChild(4).gameObject.SetActive(true);
                }
                else if (GetRoomTag(i) > 0)
                {
                    // 반대경우에는 닫힌이미지를 비활성해 열려있는것처럼 보이게 처리
                    Panel_PlayerSlot[i].transform.GetChild(4).gameObject.SetActive(false);
                }

                // 슬롯이 열려있는 상태인데 플레이어가 없다면 비우는 처리
                if (GetPlayer(i) == null)
                {
                    Panel_PlayerSlot[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
                    Panel_PlayerSlot[i].transform.GetChild(1).gameObject.SetActive(false);
                    Panel_PlayerSlot[i].transform.GetChild(2).gameObject.SetActive(false);
                    Panel_PlayerSlot[i].transform.GetChild(3).gameObject.SetActive(false);
                    Panel_PlayerSlot[i].transform.GetChild(5).gameObject.SetActive(false);
                }
                // 반대 경우라면 해당 플레이어 정보 넣어주는 처리
                else
                {
                    Panel_PlayerSlot[i].GetComponentInChildren<TextMeshProUGUI>().text = GetPlayer(i).NickName; ;
                    Panel_PlayerSlot[i].transform.GetChild(1).gameObject.SetActive(true);
                    Panel_PlayerSlot[i].transform.GetChild(3).gameObject.SetActive(true);

                    // 방장은 본인의 방장표시를 켜줌
                    if (GetPlayer(i).IsMasterClient)
                    {
                        Panel_PlayerSlot[i].transform.GetChild(2).gameObject.SetActive(false);
                        Panel_PlayerSlot[i].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "방 장";
                        Panel_PlayerSlot[i].transform.GetChild(5).gameObject.SetActive(true);
                    }
                    else
                    {
                        // 클라이언트들은 본인의 레디상태에 따라 레디이미지를 띄우거나 내리고 방장표시는 내린다
                        Panel_PlayerSlot[i].transform.GetChild(2).gameObject.SetActive((bool)GetPlayer(i).CustomProperties["IsReady"]);
                        Panel_PlayerSlot[i].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "READY";
                        Panel_PlayerSlot[i].transform.GetChild(5).gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    // 방에 들어가고 나갈때 연출할 로직
    IEnumerator ChangeUIProcess()
    {
        float fadeCount = 0; // 처음 알파값(투명)
        Panel_ChangeUIEffect.SetActive(true);
        while (fadeCount < 1.0f) // 알파값이 1 검정색이 될때까지 반복
        {
            fadeCount += 0.05f;
            yield return uiUpdateTime;
            Panel_ChangeUIEffect.GetComponent<Image>().color = new Color(0, 0, 0, fadeCount);
        }

        if (leftRoomDone == false)
        {
            PhotonNetwork.LeaveRoom();
            yield return leftRoomDone = true;
            // UI들을 상황에 맞게 처리
            Panel_Room.SetActive(false);
            Panel_Login.SetActive(true);
            Panel_Lobby.SetActive(true);
        }

        if (enterRoomDone == false)
        {
            PhotonNetwork.JoinRoom(myRoomName, null);
            yield return enterRoomDone = true;
            // 패널들의 UI들을 맞게 처리해준다
            Panel_Login.SetActive(false);
            Panel_Lobby.SetActive(false);
            Panel_Room.SetActive(true);
        }

        if(createRoomDone == false)
        {
            PhotonNetwork.CreateRoom(myRoomName, myRo); // 실제로 방을 만드는 함수
            yield return createRoomDone = true;
            // 패널들의 UI들을 맞게 처리해준다
            Panel_Login.SetActive(false);
            Panel_Lobby.SetActive(false);
            Panel_Room.SetActive(true);
        }

        while (fadeCount > 0) // 다시 알파값이 0 투명해질때 까지 반복
        {
            fadeCount -= 0.05f;
            yield return uiUpdateTime;
            Panel_ChangeUIEffect.GetComponent<Image>().color = new Color(0, 0, 0, fadeCount);
        }
        Panel_ChangeUIEffect.SetActive(false);
    }

    #endregion

    #region 방 관련 함수

    private void ResetMyRoom()
    {
        // 방을 나가면 다음방에 들어가기전 UI처리들을 초기화하여준다
        for (int i = 0; i < 6; i++)
        {
            for (int j = 1; j <= 5; j++)
            {
                Panel_PlayerSlot[i].transform.GetChild(j).gameObject.SetActive(false);
                Panel_PlayerSlot[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }

    // 방에 태그를 달아주는 함수
    private void SetRoomTag(int slotIndex, int value) => curRoom.SetCustomProperties(new Hashtable { { slotIndex.ToString(), value } });

    // 방의 태그를 얻어오는 함수
    private int GetRoomTag(int slotIndex) => (int)curRoom.CustomProperties[slotIndex.ToString()];

    // 플레이어를 뱉어주는 함수
    private Player GetPlayer(int slotIndex)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].ActorNumber == GetRoomTag(slotIndex))
                return PhotonNetwork.PlayerList[i];
        }
        return null;
    }

    // 자기자신(로컬플레이어)의 태그를 달아주는 함수
    private void SetLocalTag(string key, bool value) => PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { key, value } });


    //private object GetLocalTag(string key) => (bool)PhotonNetwork.LocalPlayer.CustomProperties[key];

    // 플레이어들의 레디상태들을 체크
    public bool CheckPlayersReady()
    {
        int readyCnt = 0;
        // 방안의 플레이어들의 수만큼 반복문을 돌아
        for (int i = 0; i < curRoom.PlayerCount; i++)
        {
            // 해당 플레이어의 준비상태 태그 IsReady가 true라면 카운트를 올려
            if ((bool)GetPlayer(i).CustomProperties["IsReady"])
                readyCnt++;
        }

        // 이 카운트와 방장을 제외한 플레이어의 수가 같으면 true
        if (readyCnt == curRoom.MaxPlayers - 1)
            return true;
        else // 같지 않으면 false
            return false;
    }

    #endregion
}
