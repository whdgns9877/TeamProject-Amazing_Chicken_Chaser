using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MissilePool : MonoBehaviourPun
{

    //�̱��� �װ��� ���⼭ �ʿ������� �𸣰����� �ϴ� ����� ����.
    #region �̱���
    static MissilePool instance = null;
    public static MissilePool Inst
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<MissilePool>();
                if (instance == null)
                    instance = new GameObject("MissilePool").AddComponent<MissilePool>();
            }
            return instance; 
        }
    }
    #endregion
    //========================================================

    Missile missilePrefab = null;
    public PhotonView PV = null;
    Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        missilePrefab = Resources.Load<Missile>("Missile");
        PV = photonView;
    }

    public void OrderInst(Transform playerTrans, int ID)
    {
        Vector3 playerPos = playerTrans.position;
        Vector3 playerForward = playerTrans.forward;
        PV.RPC("CreateMissile", RpcTarget.AllViaServer, playerPos, playerForward, ID); //RPC�� ���ڰ��� Ʈ�������� ���� �� ����.
    }

    //�ܺηκ����� ���� ��û 
    [PunRPC]
    public GameObject CreateMissile(Vector3 playerPos, Vector3 playerForward, int ID)
    {
        Debug.Log("RPCȣ��");
        GameObject instMissile = null;
        //ó������ �ƹ��͵� ������ ������ ����
        if(pool.Count == 0)
        {
            instMissile = PhotonNetwork.Instantiate("Missile", playerPos + new Vector3(0, 0.4f, 0f), Quaternion.LookRotation(playerForward)); 
            return instMissile;
        }
        //instMissile.transform.parent = playerTrans; //�θ���
        instMissile = pool.Dequeue(); 
        instMissile.SetActive(true);
        instMissile.transform.position = playerPos + new Vector3(0, 0.4f, 0f);
        instMissile.transform.rotation = Quaternion.LookRotation(playerForward);
        Debug.Log("�̻��� ID ���� : " + ID);
        //instMissile.transform.forward = dir;

        return instMissile;

    }

    //��Ȱ��ȭ
    //[PunRPC]
    public void DestroyMissile(GameObject missile)
    {
        
        missile.SetActive(false);
        pool.Enqueue(missile);


    }
}
