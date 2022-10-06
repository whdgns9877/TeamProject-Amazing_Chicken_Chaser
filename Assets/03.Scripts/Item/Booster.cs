using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Booster : MonoBehaviourPun
{
    [SerializeField] float duration = 3f;
    // Start is called before the first frame update
    private void OnEnable()
    {
        GetComponentInParent<Rigidbody>().AddRelativeForce(0, 0, 2000000f);
        StartCoroutine(DurTime());
        //if (photonView.IsMine)
        //photonView.RPC("RPCDurTime", RpcTarget.AllViaServer);
    }

    //[PunRPC]
    //private void RPCDurTime()
    //{
    //    StartCoroutine(DurTime());
    //}

    IEnumerator DurTime()
    {
        yield return new WaitForSeconds(duration);
        transform.parent.gameObject.GetComponent<PlayerMove>().booster = false;
        gameObject.SetActive(false);
    }
}

