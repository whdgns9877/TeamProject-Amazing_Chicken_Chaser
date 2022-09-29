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
       colliders = Physics.OverlapSphere(transform.position, 30, playerLayer); //�����Ǹ� �ֺ� ���� ���� �÷��̾ �ִ����� ����
        if(colliders != null)
        {
            target = colliders[0].gameObject; //�迭�� ���� ù��°�� �ɸ� �༮�� Ÿ������ �����Ѵ�
            Debug.Log(target.name + " : Ÿ��");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            Debug.Log("Ÿ�������� ���ư�����");
            transform.LookAt(target.transform.position); //Ÿ���� ���Ͽ�
            transform.Translate(transform.forward); //���ư�~~
        }

        else
        {
            Debug.Log("Ÿ�� �����ϱ� �� �����ΰ�����");
            transform.LookAt(transform.forward);
            transform.Translate(transform.forward); // ������ ������ ���ư�~~
        }  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") //�ε��� ���� �ڽ��� �θ� �ƴ϶�� ��.��.��.��
        {
            Debug.Log("Boom");
            other.gameObject.transform.parent.GetComponent<Rigidbody>().AddExplosionForce(1000000, Vector3.up, 50, 10);
            gameObject.SetActive(false); //ũŪ �������
        }
        else
        {
            gameObject.SetActive(false); // ������ �ε����� �ϴ� �����Ѵ�.
        }

    }

}
