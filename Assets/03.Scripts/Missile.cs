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

    Vector3 dir;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        aliveTime = 5f; //유지시간
        dir = transform.parent.forward;
        transform.LookAt(dir);
        transform.SetParent(null);
        
       colliders = Physics.OverlapSphere(transform.position, 40, playerLayer); //생성되면 주변 범위 내에 플레이어가 있는지를 감지
        if(colliders != null)
        {
            for(int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject == transform.parent.transform.Find("Collider").gameObject) // 만약 감지된 플레이어가 자신이라면
                {
                    ++i; //다음 순번으로 넘어가라
                }
                else
                {
                    target = colliders[i].gameObject; // 감지된 플레이어 타겟 설정 ON
                }
            }
            
             //배열에 제일 첫번째로 걸린 녀석을 타겟으로 설정한다
            Debug.Log(target.name + " : 타겟");
        }
    }

    private void OnDisable()
    {
        aliveTime = 5f;

    }

    // Update is called once per frame
    void Update()
    {
        aliveTime -= Time.deltaTime;
        if(aliveTime <= 0)
            gameObject.SetActive(false);

        if(target != null)
        {
            Debug.Log("타겟쪽으로 날아간다잉");
            transform.Translate(dir); //날아가~~
        }

        else
        {
            transform.Translate(dir);
            Debug.Log("타겟 없으니까 걍 앞으로간다잉");
            Debug.Log("앞방향 : " + transform.forward);
        }  
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && other.gameObject != transform.parent.transform.Find("Collider").gameObject) //부딪힌 것이 자신의 부모가 아니라면 파.괘.한.다
        {
            Debug.Log("Boom");
            PhotonNetwork.Instantiate("hitEffect", transform.position, Quaternion.identity); 
            other.gameObject.transform.parent.GetComponent<Rigidbody>().AddExplosionForce(1000000, Vector3.up, 50, 10);
            gameObject.SetActive(false); //크큭 사라져라
        }

        else if (other.gameObject != transform.parent.gameObject) // 발사한 플레이어가 아닌 다른 무언가와 부딪힌다면 파.괘.한.다
        {
            PhotonNetwork.Instantiate("hitEffect", transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }


}
