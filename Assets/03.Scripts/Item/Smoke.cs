using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Smoke : MonoBehaviourPun
{
    private void OnEnable()
    {
        Destroy(gameObject, 8f);
    }
}
