using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMove_JSP : MonoBehaviour
{

    [Header("Wheel Info")]
    [SerializeField] WheelCollider[] wheelColliders = new WheelCollider[4];
    [SerializeField] GameObject[] wheelMeshes = new GameObject[4];

    [SerializeField] float AItorque = 800;

    public float maxSteerAngle = 45;


    GameObject TargetObject;
    Transform target;

    NavMeshAgent myNavMesh = null;
    NavMeshPath myPath = null;


    private void Awake()
    {
        // myNavMesh = GetComponent<NavMeshAgent>();

        TargetObject = GameObject.FindWithTag("Player");
        target = TargetObject.transform;
    }


    private void FixedUpdate()
    {
        WheelSteer();

        for (int i = 0; i < wheelColliders.Length; i++)
        {
            UpdateWheelPos(wheelColliders[i], wheelMeshes[i].transform);
        }

        wheelColliders[0].motorTorque = AItorque;
        wheelColliders[1].motorTorque = AItorque;

    }



    void WheelSteer()
    {

        // get target's vector relative with my position 
        Vector3 relativeVector = transform.InverseTransformPoint(target.position);


        // divide relativeVector x by lenght of vector => direction between -1 ~ 1
        float steerAngle = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        wheelColliders[0].steerAngle = steerAngle;
        wheelColliders[1].steerAngle = steerAngle;
    }

    void UpdateWheelPos(WheelCollider collider, Transform transform)
    {

        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);
        transform.position = position;
        transform.rotation = rotation;

    }


}
