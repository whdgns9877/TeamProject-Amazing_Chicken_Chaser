using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Photon.Pun;
using System.Runtime.ExceptionServices;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class PlayerMove : MonoBehaviourPun
{
    [Header("Car Info")]
    public float Acceleration = 100f;       // �ڵ��� �ӵ�
    public float BrakingForce = 1000f;      // �극��ũ 
    public float MaxTurnAngle = 45f;        // ȸ�� ��

    [Header("Wheel Info")]
    [SerializeField] GameObject[] wheelMeshes = new GameObject[4];
    [SerializeField] WheelCollider[] wheelColliders = new WheelCollider[4];
    [SerializeField] TrailRenderer[] trailrenderers = new TrailRenderer[4];

    // ����ȭ�� ���Ǵ� �����
    PhotonView PV;


    // �÷��̾� Rigidbody 
    Rigidbody playerRigid;

    // �÷��̾� ���� �߽� 
    public GameObject mycg;

    // �÷��̾� ī�޶� 
    GameObject myCAM;

    //�÷��̾� sideslipe 
    WheelFrictionCurve myFriction = new WheelFrictionCurve();


    private void Awake()
    {
        // ����並 ĳ���Ͽ� �����´� (GetComponent�� �������°� �����ϸ� ��)
        PV = photonView;
        playerRigid = GetComponent<Rigidbody>();
        myCAM = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Start()
    {
        // turn off all the trail renderers 
        StartCoroutine(skidMark(false, 4, 3f));
    }

    private void Update()
    {
        // Apply brakes
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Rear Wheel Brake
            wheelColliders[2].brakeTorque = BrakingForce;
            wheelColliders[3].brakeTorque = BrakingForce;

            // reduce rear wheel stiffness to drifting
            StartDrift();
            StartCoroutine(skidMark(true, 4, 1.5f));
        }

        // release brakes 
        if (Input.GetKeyUp(KeyCode.Space))
        {
            wheelColliders[0].brakeTorque = 0f;
            wheelColliders[1].brakeTorque = 0f;
            wheelColliders[2].brakeTorque = 0f;
            wheelColliders[3].brakeTorque = 0f;

            //increase rear wheel stiffness to stop drifting
            Endrift();
            StartCoroutine(skidMark(false, 4, 1.5f));
        }
    }


    private void FixedUpdate()
    {
        // ������ ����� ���ʸ� ����
        if (!PV.IsMine)
            return;

        // move C.G of vehicle
        playerRigid.centerOfMass = mycg.transform.localPosition;

        //input keys 
        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");


        // Send xAxis info to myCAM
        myCAM.SendMessage("TurnMyCAM", xAxis, SendMessageOptions.DontRequireReceiver);

        //if has horizontal key input, Steer wheels 
        if (xAxis != 0f)
        {
            // ���� �� ������ 
            wheelColliders[0].steerAngle = xAxis * MaxTurnAngle;    // front left wheel 
            wheelColliders[1].steerAngle = xAxis * MaxTurnAngle;    // front right wheel
        }

        if (zAxis != 0f)
        {
            // ��ũ�� �ִ� �� 
            // FWD 
            wheelColliders[0].motorTorque = zAxis * Acceleration;
            wheelColliders[1].motorTorque = zAxis * Acceleration;

            // RWD
            wheelColliders[2].motorTorque = zAxis * Acceleration;
            wheelColliders[3].motorTorque = zAxis * Acceleration;
        }


        //if no input, set all the motor torque to zero 
        else
        {
            wheelColliders[0].motorTorque = 0;
            wheelColliders[1].motorTorque = 0;
            wheelColliders[2].motorTorque = 0;
            wheelColliders[3].motorTorque = 0;
        }


        //set all the wheelmash position & rotation as same as wheelCollider
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            // wheel collider�� ���� wheel mesh�� ������ �� �ֵ��� �� 
            UpdateWheelPos(wheelColliders[i], wheelMeshes[i].transform);
            //PV.RPC("UpdateWheelPos", RpcTarget.AllViaServer, wheelColliders[i], wheelMeshes[i].transform);
        }

    }

    // to move wheelmash as wheelCollider moves  
    //[PunRPC]
    void UpdateWheelPos(WheelCollider collider, Transform transform)
    {
        Vector3 position;
        Quaternion rotation;

        // GetWorldPos is the function of wheelcollider, you can get position and rotation of the wheel collider 
        collider.GetWorldPose(out position, out rotation);

        //transform wheel mash position and rotation as same as wheel collider 
        transform.position = position;
        transform.rotation = rotation;
    }




    void StartDrift()
    {
        // change only rear wheels
        for (int i = 2; i < 4; i++)
        {
            // increase sideways slip value for drifting
            myFriction.extremumSlip = wheelColliders[i].sidewaysFriction.extremumSlip;
            myFriction.extremumValue = wheelColliders[i].sidewaysFriction.extremumValue;
            myFriction.asymptoteSlip = wheelColliders[i].sidewaysFriction.asymptoteSlip;
            myFriction.asymptoteValue = wheelColliders[i].sidewaysFriction.asymptoteValue;

            //change stiffness Value 
            myFriction.stiffness = 0.01f;

            wheelColliders[i].sidewaysFriction = myFriction;
        }
    }


    void Endrift()
    {

        // change only rear wheels 
        for (int i = 2; i < 4; i++)
        {
            // increase sideways slip value for drifting
            myFriction.extremumSlip = wheelColliders[i].sidewaysFriction.extremumSlip;
            myFriction.extremumValue = wheelColliders[i].sidewaysFriction.extremumValue;
            myFriction.asymptoteSlip = wheelColliders[i].sidewaysFriction.asymptoteSlip;
            myFriction.asymptoteValue = wheelColliders[i].sidewaysFriction.asymptoteValue;

            //change stiffness Value 
            myFriction.stiffness = 3f;

            wheelColliders[i].sidewaysFriction = myFriction;
        }
    }



    // emitting on or off, and how many wheels? 
    IEnumerator skidMark(bool on, int wheels, float sec)
    {
        // if skid mark false, wait for seconds 
        if(!on)
        yield return new WaitForSeconds(sec);

        // set trailrenderer 
        for (int i = 0; i < wheels; i++)
        {
            trailrenderers[i].emitting = on;      // rear wheel only        
        }

        yield return null;

    }
}
