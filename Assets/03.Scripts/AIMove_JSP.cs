using ExitGames.Client.Photon;
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


    //============================================================================
    // AI target 관련 
    GameObject TargetObject;
    Transform target;

    NavMeshAgent myNavMesh;


    public LayerMask Players;

    float closesTargetDistance = float.MaxValue;
    //============================================================================

    private void Awake()
    {
        myNavMesh = GetComponent<NavMeshAgent>();

        TargetObject = GameObject.FindWithTag("Player");
        target = TargetObject.transform;
    }


    private void FixedUpdate()
    {
        StartCoroutine(UpdatePath());

        for (int i = 0; i < wheelColliders.Length; i++)
        {
            UpdateWheelPos(wheelColliders[i], wheelMeshes[i].transform);
        }

        wheelColliders[0].motorTorque = AItorque;
        wheelColliders[1].motorTorque = AItorque;
        wheelColliders[2].motorTorque = AItorque;
        wheelColliders[3].motorTorque = AItorque;

        //myNavMesh.SetDestination(target.position);

    }



    void WheelSteer(Vector3 waypoint)
    {

        // get target's vector relative with my position 
        Vector3 relativeVector = transform.InverseTransformPoint(waypoint);


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



    //AI의 타겟을 찾는 코루틴
    IEnumerator UpdatePath()
    {
        NavMeshPath myPath = new NavMeshPath();

        Collider[] targetColliders = Physics.OverlapSphere(transform.position, 1000f, Players);

        for (int i = 0; i < targetColliders.Length; i++)
        {
            if (NavMesh.CalculatePath(transform.position, targetColliders[i].transform.position, myNavMesh.areaMask, myPath))
            {
                WheelSteer(myPath.corners[1]);
            }

            for (int j = 0; j < myPath.corners.Length - 1; j++)
            {
                Debug.DrawLine(myPath.corners[j], myPath.corners[j + 1]);
            }
       
        }


        
        yield return null;
    }



}
