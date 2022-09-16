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
    //[SerializeField] GameObject      Panel_Login         = null;  // �α��� �г� -> ���߿� ���ϼ��������� ���ܵ�
    [SerializeField] GameObject Panel_Lobby  = null;  // �κ� �г�
    [SerializeField] GameObject Content_Room = null;  // �� ������ ��ũ�Ѻ信 �־��� ���� ��ġ(Vertical Layout Group ������� �˸°� ���� �Ұ���)

    [Header("** �� UI **")]
    [SerializeField] GameObject        Panel_Room      = null;    // ���� ��ü���� �г�
    [SerializeField] TextMeshProUGUI[] PlayerNickNames = null;    // �÷��̾���� �г����� ���� �迭

    [SerializeField] TextMeshProUGUI Text_ConnectionInfo = null;  // ���� ��Ʈ��ũ ���� �޼����� ��Ÿ�� TextMeshPro

    string ninkNameInputField;

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
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
    }

    // ���� ���� ��ư�� ���� �Լ�
    public void OnClick_DisConnect()
    {
        Text_ConnectionInfo.text = "���� ������ ...";
        PhotonNetwork.Disconnect();
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
    }
}
