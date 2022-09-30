using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Missile : MonoBehaviourPun
{
    GameObject target = null;
    Collider[] colliders = null;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] GameObject hitEffect;
    [SerializeField] float aliveTime;

    MissilePool missilePool;
    [SerializeField] bool Me = false;

    PhotonView PV;

    void Awake()
    {
        PV = photonView;
        playerLayer = LayerMask.NameToLayer("Player");
        hitEffect = Resources.Load<GameObject>("hitEffect");
    }

    [PunRPC]
    private void OnEnable()
    {
        if (PV.IsMine)
            Me = true;

        aliveTime = 5f; //유지시간
        //transform.SetParent(null);
       
       //colliders = Physics.OverlapSphere(transform.position, 40, playerLayer); //생성되면 주변 범위 내에 플레이어가 있는지를 감지
       // if(colliders != null)
       // {
       //     for(int i = 0; i < colliders.Length; i++)
       //     {
       //         if (colliders[i].GetComponent<PhotonView>().IsMine == true)
       //         {
       //             continue;
       //         }
       //         else
       //         target = colliders[i].gameObject; // 감지된 플레이어 타겟 설정 ON
       //     }
            
       //      //배열에 제일 첫번째로 걸린 녀석을 타겟으로 설정한다
       //     Debug.Log(target.name + " : 타겟");
       // }
    }


    private void OnDisable()
    {
        aliveTime = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        aliveTime -= Time.deltaTime;
        if (aliveTime <= 0)
            Destroy(gameObject);

            transform.Translate(Vector3.forward, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Me" || other.gameObject.tag == "Chicken" || other.gameObject.tag == "Missile" || other.gameObject.tag == "Bomb" || other.gameObject.tag == "Untagged")
        {
            return;
        }
        else
        {
            PhotonNetwork.Instantiate("hitEffect", transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }

    }
}
