using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCarCAM : MonoBehaviour
{
    [SerializeField] float myCAMDistance = 8f;
    [SerializeField] float myCAMAngle = 22f;


    [SerializeField] Transform myTarget;


    // Update is called once per frame
    void Update()
    {
        // move my Camera behind of player
        transform.position = myTarget.position - myTarget.forward * myCAMDistance;

        // look at player
        transform.LookAt(myTarget.position);

        // rotate camera
        transform.RotateAround(myTarget.position, myTarget.right, myCAMAngle);

    }

}
