using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;           // TextMeshProUGUI ����� ���� using
using UnityEngine.UI;  // UI ����� ���� using
using Photon.Pun;      // ���� ���̺귯���� ����Ƽ ������Ʈ�� ����� �� �ְ� �ϴ� ���̺귯��  
using Photon.Realtime; // ������ �ǽð� ��Ʈ��ũ ���� ���߿� C# ���̺귯��
using HashTable = ExitGames.Client.Photon.Hashtable;
// ExitGames �� ������ ���� ȸ���ε� ���⿡�� ���� Hashtable�� ����Ϸ��µ�
// ����Ƽ���� �⺻������ �����ϴ� HashTable�� �̸��� ��ġ�Ƿ� ������ HashTable�� �Ҵ��Ͽ� �̸� ���

public class UIManager : MonoBehaviourPunCallbacks
{
    [Header("** �α��� UI **")]
    [SerializeField] TMP_InputField InputField_NickName = null;  // �г����� �Է¹��� InputField
    [SerializeField] Button         Button_JoinLobby    = null;  // �κ� ���� ��ư

    [Header("** �κ� UI **")]
    [SerializeField] GameObject     Panel_Login         = null;  // �α��� �г� -> ���߿� ���ϼ��������� ���ܵ�
    [SerializeField] GameObject     Panel_Lobby             = null;  // �κ� �г�
    [SerializeField] GameObject     CreateRoomPanel         = null;  // ���� �����ϴµ� ���� �г�
    [SerializeField] Button         CreateRoomPanelButton   = null;  // ����� �ǳ��� ����� ��ư
    [SerializeField] GameObject     room                    = null;  // �������� ���� ������� �� ������
    [SerializeField] Transform      Content_Room            = null;  // �� ������ ��ũ�Ѻ信 �־��� ���� ��ġ(Vertical Layout Group ������� �˸°� ���� �Ұ���)
    [SerializeField] Button         CreateRoomButton        = null;  // ���� �����ϴ� ��ư
    [SerializeField] TMP_InputField InputField_RoomName     = null;  // �� �̸��� ���� ��ǲ�ʵ�

    [Header("** �� UI **")]
    [SerializeField] TextMeshProUGUI   _roomNameText    = null;    // �� �̸�
    [SerializeField] GameObject        Panel_Room       = null;    // ���� ��ü���� �г�
    [SerializeField] TextMeshProUGUI[] PlayerNickNames  = null;    // �÷��̾���� �г����� ���� �迭
    [SerializeField] GameObject        Button_StartGame = null;    // ���� ���� ��ư

    // ------------------------- �ӽ� ���� ����
    [SerializeField] GameObject Button_ImsiStartGame = null;    // �ӽ� ���� ���� ��ư

    [SerializeField] TextMeshProUGUI Text_ConnectionInfo = null;  // ���� ��Ʈ��ũ ���� �޼����� ��Ÿ�� TextMeshPro

    string ninkNameInputField;
    // �� Ÿ��Ʋ�� ���� string
    private string roomNameText;

    // �� ������ ���� ����Ʈ
    List<RoomInfo> _roomList = new List<RoomInfo>();

    // ���� �ڽ��� �����ִ� ��
    private Room curRoom;

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // ��ư�� ��ȣ�ۿ��� ���Ƴ��´�
        Button_JoinLobby.interactable = false;

        // �����ͼ��� ���� ��û
        PhotonNetwork.ConnectUsingSettings();
    }

    // ������ ���� ���� �����ÿ� ȣ��
    public override void OnConnectedToMaster()
    {
        Text_ConnectionInfo.text = "������ ������ ���� �Ϸ�!";
    }

    // ������ ������ ������ �������� �� ȣ��
    public override void OnDisconnected(DisconnectCause cause)
    {
        Text_ConnectionInfo.text = "������ �������� ������...";
        Panel_Lobby.SetActive(false);
        InputField_NickName.interactable = true;
        Button_JoinLobby.interactable = true;

        // ���Ƿ� ������ ������ ���� ���� �Ҿ������� �����
        // ���������� �ڵ����� ������ ������ �ٽ� �����ϰ� �õ�
        PhotonNetwork.ConnectUsingSettings();
    }

    // �̸� �Է� ��Ʈ��(Input Field)
    public void OnValueChanged(string inStr)
    {
        if (string.IsNullOrEmpty(inStr))
            Button_JoinLobby.interactable = false;
        else
            Button_JoinLobby.interactable = true;

        PhotonNetwork.LocalPlayer.NickName = inStr;
        ninkNameInputField = inStr;
    }

    // �� �����Ҷ� �Է� ��Ʈ��
    public void OnvalueChangedCreateRoom(string inStr)
    {
        if (string.IsNullOrEmpty(inStr))
            CreateRoomButton.interactable = false;
        else
            CreateRoomButton.interactable = true;

        roomNameText = inStr;
    }

    // �κ� ���� ��ư�� ���� �Լ�
    public void OnClick_JoinLobby()
    {
        // ���� ��Ʈ��ũ�� ������ ������ ����� ���°� �ƴ϶�� ó������ �ʰ�
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer || string.IsNullOrEmpty(ninkNameInputField))
            return;

        Text_ConnectionInfo.text = "���� �õ� �� ...";
        InputField_NickName.interactable = false;
        Button_JoinLobby.interactable = false;
        PhotonNetwork.JoinLobby();
    }

    // �κ� ���� �Ϸ�� ȣ��Ǵ� �Լ�
    public override void OnJoinedLobby()
    {
        Text_ConnectionInfo.text = "�κ� ���� �Ϸ�!";
        // �κ� ���� �Ϸ�� �α��� �г��� ��Ȱ��ȭ �κ� �г��� Ȱ��ȭ ���ش�
        Panel_Lobby.SetActive(true);
        InputField_NickName.interactable = false;
        // �κ� ���ӽ� �켱 �� ������ ������
        _roomList.Clear();
    }

    // ���� ���� ��ư�� ���� �Լ�
    public void OnClick_DisConnect()
    {
        Text_ConnectionInfo.text = "���� ������ ...";
        PhotonNetwork.Disconnect();
    }

    // �� ������ �ٲ� �ڵ����� ȣ��Ǵ� �Լ�
    // �κ� ���� ��
    // ���ο� ���� ������� ���
    // ���� �����Ǵ� ���
    // ���� IsOpen ���� ��ȭ�� ���
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // ���� ������ �ٲ�� �� �ݹ��Լ��� ����Ǹ�
        // ���� �ִ� ����� ���� �����ְ�
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOM"))
        {
            Destroy(obj);
        }

        // roomList ����Ʈ�� �������� Ȯ���Ѵ�
        foreach (RoomInfo roomInfo in roomList)
        {
            // �� ������ isVisible�� false �̰ų� ����Ʈ���� ���ŵ� ����(�÷��̾ �ƹ��� ���) ���
            if (!roomInfo.IsVisible || roomInfo.RemovedFromList)
            {
                // �� ��Ͽ��� �����Ѵ�
                if (_roomList.IndexOf(roomInfo) != -1)
                    _roomList.RemoveAt(_roomList.IndexOf(roomInfo));
            }
            else
            {
                // ���� ��Ȳ�� �ƴϸ� �渮��Ʈ�� ���� �־��ش�
                if (!_roomList.Contains(roomInfo)) _roomList.Add(roomInfo);
                else _roomList[_roomList.IndexOf(roomInfo)] = roomInfo;
            }
        }

        // ���� ������ ���� ����Ʈ�� �ִ� ����� ������ش�
        foreach (RoomInfo roomInfo in _roomList)
        {
            GameObject _room = Instantiate(room, Content_Room);
            RoomData roomData = _room.GetComponent<RoomData>();
            roomData.roomName = roomInfo.Name;
            roomData.maxPlayer = roomInfo.MaxPlayers;
            roomData.playerCount = roomInfo.PlayerCount;
            roomData.isOpen = roomInfo.IsOpen;
            roomData.UpdateInfo();

            // �ش� ���� �ο��� ���������� ��ưŬ���� ���� �����Ҽ� �����Ѵ�
            if (roomData.playerCount == roomData.maxPlayer)
                _room.GetComponent<Button>().interactable = false;

            // ���� ���������� �������� ��ư Ŭ���� ����
            if (roomData.isOpen == false)
                _room.GetComponent<Button>().interactable = false;
            else
            {
                // ���� ����������
                // delegate�� ������ �����Ͽ� Ŭ�������� �濡 ������ �� �ֵ��� ó��
                roomData.GetComponent<Button>().onClick.AddListener
                (
                    delegate
                    {
                        roomNameText = roomData.roomName;
                        // �� �κ��� ������ �濡 �����ϴ� �κ�
                        PhotonNetwork.JoinRoom(roomData.roomName, null);
                    }
                );
            }
        }
    }

    // �� �����г� ��ư�� �޾��� �Լ�
    public void OnCreateRoomInfoButtonClicked()
    {
        // �� ������ �Է¹��� �ǳ��� Ȱ��ȭ ��Ű��
        CreateRoomPanel.SetActive(true);
        // �ߺ��ؼ� ��� ��ư�� ������ �ʰ� ���� ��ư�� Ŭ���� ���´�
        CreateRoomButton.GetComponent<Button>().interactable = false;
    }

    // �� ���� ��ư�� �� �Լ�
    public void OnCreateRoomButtonClicked()
    {
        // ���� ��������� ������ư�� �ٽ� ����Ҽ� �ְ� Ŭ���� Ǯ���ְ�
        CreateRoomPanelButton.GetComponent<Button>().interactable = true;

        // ���� ������ ����ִ� Ŭ����
        RoomOptions ro = new RoomOptions();
        ro.IsVisible = true;                        // ���� ���̰�
        ro.IsOpen = true;                           // ���� ����
        ro.MaxPlayers = 8;                          // �ִ� �ο����� 8
        ro.CleanupCacheOnLeave = true;

        PhotonNetwork.CreateRoom(roomNameText, ro); // ������ ���� ����� �Լ�
    }

    // �濡 �����ϸ� �ڵ������� ȣ��Ǵ� �ݹ��Լ�
    public override void OnJoinedRoom()
    {
        curRoom = PhotonNetwork.CurrentRoom;

        _roomNameText.text = "�� : " + curRoom.Name;

        Panel_Login.SetActive(false);
        Panel_Lobby.SetActive(false);
        Panel_Room.SetActive(true);

        if(PhotonNetwork.IsMasterClient)
        {

        }
    }

    private void Update()
    {
        // ��Ʈ��ũ ���°� ������ ������ ����Ǿ����� �ʴٸ�
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
        {
            // �� UI�� ��ȣ�ۿ��� ���´�
            InputField_NickName.interactable = false;
            Button_JoinLobby.interactable = false;
        }
        // ��Ʈ��ũ ���°� ������ ������ ����� ���¶��
        else
        {
            // InputField�� �Է��� �ؽ�Ʈ�� �����������
            if(string.IsNullOrEmpty(ninkNameInputField))
                // ���� Button�� ��ȣ�ۿ��� ���´�
                Button_JoinLobby.interactable = false;
            // InputField�� �Է��� �ؽ�Ʈ�� �����ΰ� �����Ѵٸ�(�Է��Ѱ� �ִٸ�)
            else
                // ���� Button�� ��ȣ�ۿ��� Ȱ��ȭ ��Ų��
                Button_JoinLobby.interactable = true;

            // InputField�� ��ȣ�ۿ� �����ϰ�
            InputField_NickName.interactable = true;
        }

        if (!CreateRoomPanel.activeInHierarchy)
            InputField_RoomName.text = null;
    }


    //------------------------------- �ӽ÷� ���� ���� �Լ�---------------------------------------------------------
    public void ImsiGameStart()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}
