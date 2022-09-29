using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    GameObject target = null;
    Collider[] colliders = null;
    [SerializeField] LayerMask playerLayer;

    void Start()
    {
        
    }

    private void OnEnable()
    {
       colliders = Physics.OverlapSphere(transform.position, 30, playerLayer); //생성되면 주변 범위 내에 플레이어가 있는지를 감지
        if(colliders != null)
        {
            target = colliders[0].gameObject; //배열에 제일 첫번째로 걸린 녀석을 타겟으로 설정한다
            Debug.Log(target.name + " : 타겟");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            Debug.Log("타겟쪽으로 날아간다잉");
            transform.LookAt(target.transform.position); //타겟을 향하여
            transform.Translate(transform.forward); //날아가~~
        }

        else
        {
            Debug.Log("타겟 없으니까 걍 앞으로간다잉");
            transform.LookAt(transform.forward);
            transform.Translate(transform.forward); // 없으면 앞으로 날아가~~
        }  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") //부딪힌 것이 자신의 부모가 아니라면 파.괘.한.다
        {
            Debug.Log("Boom");
            other.gameObject.transform.parent.GetComponent<Rigidbody>().AddExplosionForce(1000000, Vector3.up, 50, 10);
            gameObject.SetActive(false); //크큭 사라져라
        }
        else
        {
            gameObject.SetActive(false); // 뭔가에 부딪히면 일단 폭발한다.
        }

    }

}
