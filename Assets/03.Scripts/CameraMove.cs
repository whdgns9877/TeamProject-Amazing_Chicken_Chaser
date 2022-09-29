using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Transform myTr;
    [SerializeField] Transform playerTr;

    // Update is called once per frame
    void Update()
    {
        transform.position = myTr.position;
        transform.LookAt(playerTr.position);
    }
}
