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
        aliveTime = 5f; //�����ð�
        dir = transform.parent.forward;
        transform.LookAt(dir);
        transform.SetParent(null);
        
       colliders = Physics.OverlapSphere(transform.position, 40, playerLayer); //�����Ǹ� �ֺ� ���� ���� �÷��̾ �ִ����� ����
        if(colliders != null)
        {
            for(int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject == transform.parent.transform.Find("Collider").gameObject) // ���� ������ �÷��̾ �ڽ��̶��
                {
                    ++i; //���� �������� �Ѿ��
                }
                else
                {
                    target = colliders[i].gameObject; // ������ �÷��̾� Ÿ�� ���� ON
                }
            }
            
             //�迭�� ���� ù��°�� �ɸ� �༮�� Ÿ������ �����Ѵ�
            Debug.Log(target.name + " : Ÿ��");
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
            Debug.Log("Ÿ�������� ���ư�����");
            transform.Translate(dir); //���ư�~~
        }

        else
        {
            transform.Translate(dir);
            Debug.Log("Ÿ�� �����ϱ� �� �����ΰ�����");
            Debug.Log("�չ��� : " + transform.forward);
        }  
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && other.gameObject != transform.parent.transform.Find("Collider").gameObject) //�ε��� ���� �ڽ��� �θ� �ƴ϶�� ��.��.��.��
        {
            Debug.Log("Boom");
            PhotonNetwork.Instantiate("hitEffect", transform.position, Quaternion.identity); 
            other.gameObject.transform.parent.GetComponent<Rigidbody>().AddExplosionForce(1000000, Vector3.up, 50, 10);
            gameObject.SetActive(false); //ũŪ �������
        }

        else if (other.gameObject != transform.parent.gameObject) // �߻��� �÷��̾ �ƴ� �ٸ� ���𰡿� �ε����ٸ� ��.��.��.��
        {
            PhotonNetwork.Instantiate("hitEffect", transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }


}
