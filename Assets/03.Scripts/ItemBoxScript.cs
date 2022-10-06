using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxScript : MonoBehaviour
{
    int ItemNum;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Me"))
        {
            Debug.Log("���� ��Ҵ�!");
            ItemNum = Random.Range(1, 7); //0�� ������ ���� ó��
            if (other.gameObject.GetComponentInParent<PlayerMove>())
            {
                other.gameObject.GetComponentInParent<PlayerMove>().GetItem(ItemNum);

                GetComponentInParent<ItemBoxSpawn>().RegenBox(gameObject);

                gameObject.SetActive(false);
            }

        }
    }

}