using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Freeze : MonoBehaviourPun
{
    [SerializeField] float aliveTime;

    PhotonView PV;

    void Awake()
    {
        PV = photonView;
    }

    [PunRPC]
    private void OnEnable()
    {
        aliveTime = 5f; //유지시간
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

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Ground" || other.gameObject.tag == "Building")
        {
            PhotonNetwork.Instantiate("FreezeExplosion", transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Player" || other.gameObject.tag == "Ground" || other.gameObject.tag == "Building")
    //    {
    //        PhotonNetwork.Instantiate("FreezeExplosion", transform.position, Quaternion.identity);
    //        PhotonNetwork.Destroy(gameObject);
    //    }
    //}
}
