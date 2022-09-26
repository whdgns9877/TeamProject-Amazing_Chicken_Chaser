using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;           // TextMeshProUGUI ����� ���� using
using UnityEngine.UI;  // UI ����� ���� using
using Photon.Pun;      // ���� ���̺귯���� ����Ƽ ������Ʈ�� ����� �� �ְ� �ϴ� ���̺귯��  
using Photon.Realtime; // ������ �ǽð� ��Ʈ��ũ ���� ���߿� C# ���̺귯��
using Hashtable = ExitGames.Client.Photon.Hashtable;

// ExitGames �� ������ ���� ȸ���ε� ���⿡�� ���� Hashtable�� ����Ϸ��µ�
// ����Ƽ���� �⺻������ �����ϴ� Hashtable�� �ƴ� ���濡�� �����ϴ� Hashtable�� ����Ѵ�

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI Text_ConnectionInfo = null;  // ���� ��Ʈ��ũ ���� �޼����� ��Ÿ�� TextMeshPro

    [Header("** �α��� UI **")]
    [SerializeField] TMP_InputField InputField_NickName = null;  // �г����� �Է¹��� InputField
    [SerializeField] Button         Button_JoinLobby    = null;  // �κ� ���� ��ư

    [Header("** �κ� UI **")]
    [SerializeField] GameObject     Panel_Login             = null;  // �α��� �г�
    [SerializeField] GameObject     Panel_Lobby             = null;  // �κ� �г�
    [SerializeField] GameObject     Panel_CreateRoom        = null;  // ���� �����ϴµ� ���� �г�
    [SerializeField] Button         Button_CreateRoomPanel  = null;  // ����� �ǳ��� ����� ��ư
    [SerializeField] Transform      Tr_Content_Room         = null;  // �� ������ ��ũ�Ѻ信 �־��� ���� ��ġ(Vertical Layout Group ������� �˸°� ���� �Ұ���)
    [SerializeField] Button         Button_CreateRoom       = null;  // ���� �����ϴ� ��ư
    [SerializeField] TMP_InputField InputField_RoomName     = null;  // �� �̸��� ���� ��ǲ�ʵ�
    [SerializeField] GameObject     room                    = null;  // �������� ���� ������� �� ������
    [SerializeField] Toggle[]       togglesForMaxPlayer     = null;  // �ִ� �÷��̾ ������ ��۵�

    [Header("** �� UI **")]
    [SerializeField] TextMeshProUGUI   Text_roomName    = null;    // �� �̸�
    [SerializeField] GameObject        Panel_Room       = null;    // ���� ��ü���� �г�
    //[SerializeField] TextMeshProUGUI[] PlayerNickNames  = null;    // �÷��̾���� �г����� ���� �迭
    [SerializeField] GameObject        Button_StartGame = null;    // ���� ���� ��ư
    [SerializeField] GameObject        Button_Ready     = null;    // ���� ���� ��ư
    [SerializeField] GameObject[]      Panel_PlayerSlot = null;    // �÷��̾���� ���ü� �ִ� ����

    // �г��ӿ� ���� InputField
    private string nickNameInputField = "";
    // �� Ÿ��Ʋ�� ���� string
    private string roomNameText = "";

    // �� ������ ���� ����Ʈ
    List<RoomInfo> _roomList = new List<RoomInfo>();

    // ���� �ڽ��� �����ִ� ��
    private Room curRoom = null;

    private bool isLogin = false;

    // RoomOption�� maxPlayer�� byteŸ���̶� byteŸ��
    private byte myRoomMaxPlayer = 0;

    // �������� �����ϴ� �ð��� 0.2�ʷ�
    private WaitForSeconds delayUpdateTime = new WaitForSeconds(0.2f);

    // �ڱ� �ڽ��� �������
    private bool ready = false;

    private void Awake()
    {
        // �ʱ� ȭ�� ����
        Screen.SetResolution(960, 540, false);
        // ����(������ Ŭ���̾�Ʈ)�� ���Ӿ����� �̵��Ҷ� Ŭ���̾�Ʈ�鵵 ���� �̵�
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
        // �α��� �Ǿ��ִ� ���·� ������ ������ ����Ǹ� �ڵ������� �κ� ����
        if (isLogin) PhotonNetwork.JoinLobby();
    }

    // ������ ������ ������ �������� �� ȣ��
    public override void OnDisconnected(DisconnectCause cause)
    {
        Text_ConnectionInfo.text = "������ �������� ������...";
        // ������ ������ ���¿����� �κ� �г��� ��Ȱ��ȭ�ϰ� 
        Panel_Lobby.SetActive(false);
        // �г����� �Է¹޴� InputField�� ���� Button�� Ȱ��ȭ �����ش�
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
        nickNameInputField = inStr;
    }

    // �� �����Ҷ� �Է� ��Ʈ��
    public void OnvalueChangedCreateRoom(string inStr)
    {
        if (string.IsNullOrEmpty(inStr))
            Button_CreateRoom.interactable = false;
        else
            Button_CreateRoom.interactable = true;

        roomNameText = inStr;
    }

    // �κ� ���� ��ư�� ���� �Լ�
    public void OnClick_JoinLobby()
    {
        // ���� ��Ʈ��ũ�� ������ ������ ����� ���°� �ƴ϶�� ó������ �ʰ�
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer || string.IsNullOrEmpty(nickNameInputField))
            return;

        Text_ConnectionInfo.text = "���� �õ� �� ...";
        InputField_NickName.interactable = false;
        Button_JoinLobby.interactable = false;
        PhotonNetwork.JoinLobby();
        isLogin = true;
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
        isLogin = false;
    }

    // �� ������ �ٲ� �ڵ����� ȣ��Ǵ� �Լ�
    // �κ� ���� ��(������ ���� -> �κ�)
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
            GameObject _room = Instantiate(room, Tr_Content_Room);
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
        Panel_CreateRoom.SetActive(true);
        // �ߺ��ؼ� ��� ��ư�� ������ �ʰ� ���� ��ư�� Ŭ���� ���´�
        Button_CreateRoom.GetComponent<Button>().interactable = false;
    }

    // ���� �����Ҷ� �ִ� �÷��̾���� �������ִ� �Լ�
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
    // �� ���� ��ư�� �� �Լ�
    public void OnCreateRoomButtonClicked()
    {
        // ���� ��������� ������ư�� �ٽ� ����Ҽ� �ְ� Ŭ���� Ǯ���ְ�
        Button_CreateRoomPanel.GetComponent<Button>().interactable = true;

        // ���� ������ ����ִ� Ŭ����
        RoomOptions ro = new RoomOptions();
        ro.IsVisible = true;                        // ���� ���̰�
        ro.IsOpen = true;                           // ���� ����
        CheckToggleValue();                         // ���� ��� ����� ���õǾ��ִ��� Ȯ���ϰ�
        ro.MaxPlayers = myRoomMaxPlayer;            // �ִ� �ο����� üũ�� ��۰��� ���� ���� �ִ´�
        // Ŭ���̾�Ʈ���� ���� ������
        // �ش� Ŭ���̾�Ʈ�� Properties�� ����ش�
        ro.CleanupCacheOnLeave = true;

        PhotonNetwork.CreateRoom(roomNameText, ro); // ������ ���� ����� �Լ�
    }

    // �濡 �����ϸ� �ڵ������� ȣ��Ǵ� �ݹ��Լ�
    public override void OnJoinedRoom()
    {
        // ���� �濡 �޷��ִ� �±׸� Hashtable ������ curRoomProperties ��� ������ �־��ش�
        curRoom = PhotonNetwork.CurrentRoom;
        // ���� �ִ� ���� UI �� Text�� ���� ���� text�� �־��ش�
        Text_roomName.text = "�� : " + curRoom.Name;

        if (PhotonNetwork.IsMasterClient)
        {
            // �ε����� 0 ���� �����ϹǷ� -1
            int max = curRoom.MaxPlayers - 1;
            //// ������ ó���� ���� �İԵǸ� ���� �ʱ� ���Ե��� ������ ���ش�
            curRoom.SetCustomProperties(new Hashtable
            {
                // ����(ȣ��Ʈ)�� 0�� ���� ��ȣ, �������� ������0, �����Ұ��� ������ -1
                // �ش� ������ �ε����� ��ɼ��� MaxPlayer�� ���Ͽ� MaxPlayer�� �Ѿ�� ������ �ݾ��ش�
                {"0", PhotonNetwork.LocalPlayer.ActorNumber }, {"1", 0 },
                {"2", 2 <= max ? 0 : -1 }, {"3", 3 <= max ? 0 : -1 }, {"4", 4 <= max ? 0 : -1 },
                {"5", 5 <= max ? 0 : -1 }, {"6", 6 <= max ? 0 : -1 }, {"7", 7 <= max ? 0 : -1 } 
            });
        }
        else
        {
            // �濡 ������ ����� �������� �����̶�� �ڱ��ȣ��
            for (int i = 0; i < 8; i++)
            {
                if (GetRoomTag(i) == 0)
                {
                    SetRoomTag(i, PhotonNetwork.LocalPlayer.ActorNumber);
                    break;
                }
            }
        }

        // �濡 �����ϸ� �غ���¸� false��
        SetLocalTag("IsReady", false);

        // �гε��� UI���� �°� ó�����ش�
        Panel_Login.SetActive(false);
        Panel_Lobby.SetActive(false);
        Panel_Room.SetActive(true);

        // ���� ���� �±� ������ �������ش�
        StartCoroutine(RoomUpdate());
    }

    IEnumerator RoomUpdate()
    {
        // �濡 ������ ���¶�� ��� üũ�Ѵ�
        while (PhotonNetwork.InRoom)
        {
            yield return delayUpdateTime;
            // ������Ʈ ���� �濡�� �����ԵǸ� ������ ������ �����
            if(!PhotonNetwork.InRoom) yield break;

            // ������ �ٲ������ �ٲ��÷��̾ �����̸� �ش� �÷��̾��� ���ӽ��۹�ư�� Ȱ��ȭ �ȴ�
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


            // �÷��̾�� ���� ���� ����
            for (int i = 0; i < 8; i++)
            {
                // ����(������Ŭ���̾�Ʈ)�� �漳���� ���ش�
                if (PhotonNetwork.IsMasterClient)
                {
                    // ������ �����ִµ� ����� ��������� �ش� ���Կ� 0�� ����
                    if (GetPlayer(i) == null && GetRoomTag(i) > 0) SetRoomTag(i, 0);
                }

                // �� �±׸� �ٶ� ������ ������ 0, -1�� �±װ��� �����Ƿ� �̸� ���� �Ǵ�
                if (GetRoomTag(i) == -1)
                {
                    // ������ �����ٴ� UI �̹����� ����ش�
                    Panel_PlayerSlot[i].transform.GetChild(4).gameObject.SetActive(true);
                }
                else if (GetRoomTag(i) > 0)
                {
                    Panel_PlayerSlot[i].transform.GetChild(4).gameObject.SetActive(false);
                }

                // ������ �����ִ� �����ε� �÷��̾ ���ٸ� ���� ó��
                if (GetPlayer(i) == null)
                {
                    Panel_PlayerSlot[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
                    Panel_PlayerSlot[i].transform.GetChild(1).gameObject.SetActive(false);
                    Panel_PlayerSlot[i].transform.GetChild(2).gameObject.SetActive(false);
                    Panel_PlayerSlot[i].transform.GetChild(3).gameObject.SetActive(false);
                    Panel_PlayerSlot[i].transform.GetChild(5).gameObject.SetActive(false);
                }
                // �ݴ� ����� �ش� �÷��̾� ���� �־��ִ� ó��
                else
                {
                    Panel_PlayerSlot[i].GetComponentInChildren<TextMeshProUGUI>().text = GetPlayer(i).NickName; ;
                    Panel_PlayerSlot[i].transform.GetChild(1).gameObject.SetActive(true);
                    Panel_PlayerSlot[i].transform.GetChild(3).gameObject.SetActive(true);

                    // ������ ������ ����ǥ�ø� ����
                    if (GetPlayer(i).IsMasterClient)
                    {
                        Panel_PlayerSlot[i].transform.GetChild(2).gameObject.SetActive(false);
                        Panel_PlayerSlot[i].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "�� ��";
                        Panel_PlayerSlot[i].transform.GetChild(5).gameObject.SetActive(true);
                    }
                    else
                    {
                        // Ŭ���̾�Ʈ���� ������ ������¿� ���� �����̹����� ���ų� ������ ����ǥ�ô� ������
                        Panel_PlayerSlot[i].transform.GetChild(2).gameObject.SetActive((bool)GetPlayer(i).CustomProperties["IsReady"]);
                        Panel_PlayerSlot[i].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "READY";
                        Panel_PlayerSlot[i].transform.GetChild(5).gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void OnLeaveRoomButtonClicked() => PhotonNetwork.LeaveRoom();

    // �÷��̾ ���� �������� �ش� �ݹ��Լ��� ����
    public override void OnLeftRoom()
    {
        ResetMyRoom();
        // UI���� ��Ȳ�� �°� ó��
        Panel_Room.SetActive(false);
        Panel_Login.SetActive(true);
        Panel_Lobby.SetActive(true);
    }

    private void ResetMyRoom()
    {
        // ���� ������ �����濡 ������ UIó������ �ʱ�ȭ�Ͽ��ش�
        for (int i = 0; i < 8; i++)
        {
            for (int j = 1; j <= 5; j++)
            {
                Panel_PlayerSlot[i].transform.GetChild(j).gameObject.SetActive(false);
                Panel_PlayerSlot[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }
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
            if(string.IsNullOrEmpty(nickNameInputField))
                // ���� Button�� ��ȣ�ۿ��� ���´�
                Button_JoinLobby.interactable = false;
            // InputField�� �Է��� �ؽ�Ʈ�� �����ΰ� �����Ѵٸ�(�Է��Ѱ� �ִٸ�)
            else
                // ���� Button�� ��ȣ�ۿ��� Ȱ��ȭ ��Ų��
                Button_JoinLobby.interactable = true;

            // InputField�� ��ȣ�ۿ� �����ϰ�
            InputField_NickName.interactable = true;
        }

        if (!Panel_CreateRoom.activeInHierarchy)
            InputField_RoomName.text = null;
    }

    private void SetRoomTag(int slotIndex, int value) => curRoom.SetCustomProperties(new Hashtable { { slotIndex.ToString(), value } });


    private int GetRoomTag(int slotIndex) => (int)curRoom.CustomProperties[slotIndex.ToString()];
    

    private Player GetPlayer(int slotIndex)
    {
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].ActorNumber == GetRoomTag(slotIndex))
                return PhotonNetwork.PlayerList[i];
        }
        return null;
    }

    private void SetLocalTag(string key, bool value) => PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { key, value } });


    //private object GetLocalTag(string key) => (bool)PhotonNetwork.LocalPlayer.CustomProperties[key];
    

    public void OnClick_ReadyButton()
    {
        if (ready == true)
            ready = false;
        else
            ready = true;
        SetLocalTag("IsReady", ready);
        Debug.Log("���� ���� ���� ���´� " + ready);
    }

    public void OnClick_StartGame()
    {
        if (CheckPlayersReady())
        {
            curRoom.IsOpen = false;
            PhotonNetwork.LoadLevel("GameScene");
        }
        else
            Debug.Log("���� ��� �÷��̾ ������°� �ƴմϴ�");
    }

    public bool CheckPlayersReady()
    {
        int readyCnt = 0;
        for(int i = 0; i < curRoom.PlayerCount; i++)
        {
            if ((bool)GetPlayer(i).CustomProperties["IsReady"])
                readyCnt++;
        }

        if (readyCnt == curRoom.PlayerCount - 1)
            return true;
        else
            return false;
    }
}
