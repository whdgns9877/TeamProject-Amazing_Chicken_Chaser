using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;           // TextMeshProUGUI ����� ���� using
using UnityEngine.UI;  // UI ����� ���� using
using Photon.Pun;      // ���� ���̺귯���� ����Ƽ ������Ʈ�� ����� �� �ְ� �ϴ� ���̺귯��  
using Photon.Realtime; // ������ �ǽð� ��Ʈ��ũ ���� ���߿� C# ���̺귯��

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI Text_ConnectionInfo = null;  // ���� �޼���
    [SerializeField] Button          Button_JoinLobby    = null;  // �κ� ���� ��ư
    //[SerializeField] GameObject      Panel_Login         = null;  // �α��� �г� -> ���߿� ���ϼ��������� ���ܵ�
    [SerializeField] GameObject      Panel_Lobby         = null;  // �κ� �г�
    [SerializeField] TMP_InputField  InputField_NickName = null;  // �κ� �г�

    string imsiString;

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
        imsiString = inStr;
    }

    // �κ� ���� ��ư�� ���� �Լ�
    public void OnClick_JoinLobby()
    {
        // ���� ��Ʈ��ũ�� ������ ������ ����� ���°� �ƴ϶�� ó������ �ʰ�
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer || string.IsNullOrEmpty(imsiString))
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
        if (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
        {
            InputField_NickName.interactable = false;
            Button_JoinLobby.interactable = false;
        }
        else
        {
            InputField_NickName.interactable = true;
            Button_JoinLobby.interactable = true;
        }
    }
}
