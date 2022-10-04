using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCAM : MonoBehaviour
{
    [SerializeField] float myCAMDistance = 8f;
    [SerializeField] float myCAMAngle = 22f;


    Transform myTarget;

    // Start is called before the first frame update
    void Start()
    {
        myTarget = GameObject.FindGameObjectWithTag("Me").transform;
    }

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
