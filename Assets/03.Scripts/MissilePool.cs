using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MissilePool : MonoBehaviourPun
{

    //싱글톤 그것이 여기서 필요한지는 모르겠으나 일단 만들고 본다.
    #region 싱글톤
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
        PV.RPC("CreateMissile", RpcTarget.AllViaServer, playerPos, playerForward, ID); //RPC의 인자값에 트랜스폼을 넣을 수 없다.
    }

    //외부로부터의 생성 요청 
    [PunRPC]
    public GameObject CreateMissile(Vector3 playerPos, Vector3 playerForward, int ID)
    {
        Debug.Log("RPC호출");
        GameObject instMissile = null;
        //처음에는 아무것도 없으니 무적권 생성
        if(pool.Count == 0)
        {
            instMissile = PhotonNetwork.Instantiate("Missile", playerPos + new Vector3(0, 0.4f, 0f), Quaternion.LookRotation(playerForward)); 
            return instMissile;
        }
        //instMissile.transform.parent = playerTrans; //부모설정
        instMissile = pool.Dequeue(); 
        instMissile.SetActive(true);
        instMissile.transform.position = playerPos + new Vector3(0, 0.4f, 0f);
        instMissile.transform.rotation = Quaternion.LookRotation(playerForward);
        Debug.Log("미사일 ID 변경 : " + ID);
        //instMissile.transform.forward = dir;

        return instMissile;

    }

    //비활성화
    //[PunRPC]
    public void DestroyMissile(GameObject missile)
    {
        
        missile.SetActive(false);
        pool.Enqueue(missile);


    }
}
