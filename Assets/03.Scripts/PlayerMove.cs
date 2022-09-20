using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Photon.Pun;

public class PlayerMove : MonoBehaviourPun
{
    [Header("Car Info")]
    public float Acceleration = 100f;       // �ڵ��� �ӵ�
    public float BrakingForce = 1000f;      // �극��ũ 
    public float MaxTurnAngle = 45f;        // ȸ�� ��

    [Header("Wheel Info")]
    [SerializeField] GameObject[] wheelMeshes = new GameObject[4];
    [SerializeField] WheelCollider[] wheelColliders = new WheelCollider[4];

    // ����ȭ�� ���Ǵ� �����
    PhotonView PV;

    Rigidbody playerRigid;

    public GameObject mycg;

    private void Awake()
    {
        // ����並 ĳ���Ͽ� �����´� (GetComponent�� �������°� �����ϸ� ��)
        PV = photonView;
        playerRigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // move center of gravity to increase stability 
        // �ڵ����� �������� ������ ���� ���� �߽��� �̵� ��Ŵ 
        //playerRigid.centerOfMass = new Vector3(0, -1f, 0.25f);
    
    }

    //private void Update()
    //{
    //    // Apply brakes
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Debug.Log("## apply brake~!");
    //        // Rear Wheel Brake
    //        wheelColliders[2].brakeTorque = BrakingForce;
    //        wheelColliders[3].brakeTorque = BrakingForce;

    //        // wheelColliders[0].brakeTorque = BrakingForce;
    //        // wheelColliders[1].brakeTorque = BrakingForce;

    //    }

    //    // release brakes 
    //    if (Input.GetKeyUp(KeyCode.Space))
    //    {
    //        Debug.Log("## release brake~!");

    //        wheelColliders[0].brakeTorque = 0f;
    //        wheelColliders[1].brakeTorque = 0f;
    //        wheelColliders[2].brakeTorque = 0f;
    //        wheelColliders[3].brakeTorque = 0f;
    //    }
    //}


    private void Update()
    {
        // ������ ����� ���ʸ� ����
        if (!PV.IsMine)
            return;

        playerRigid.centerOfMass = mycg.transform.localPosition;

        //input keys 
        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");


        //if has horizontal key input, Steer wheels 
        if (xAxis != 0f)
        {
            // ���� �� ������ 
            wheelColliders[0].steerAngle = xAxis * MaxTurnAngle;    // front left wheel 
            wheelColliders[1].steerAngle = xAxis * MaxTurnAngle;    // front right wheel
        }

        //Debug.Log("### move!" + zAxis);
        //Debug.Log("### Speed!" + playerRigid.velocity);

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

        else
        {
            wheelColliders[0].motorTorque = 0;
            wheelColliders[1].motorTorque = 0;
            wheelColliders[2].motorTorque = 0;
            wheelColliders[3].motorTorque = 0;

        }

        Debug.Log("## wheel speed" + wheelColliders[0].motorTorque);


        //set all the wheelmash position & rotation as same as wheelCollider
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            // wheel collider�� ���� wheel mesh�� ������ �� �ֵ��� �� 
            UpdateWheelPos(wheelColliders[i], wheelMeshes[i].transform);
            //PV.RPC("UpdateWheelPos", RpcTarget.AllViaServer, wheelColliders[i], wheelMeshes[i].transform);
        }



        // Apply brakes
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("## apply brake~!");
            // Rear Wheel Brake
            // space Ű�� ������ �극��ũ

            // �Ĺ� �� 
            wheelColliders[2].brakeTorque = BrakingForce;
            wheelColliders[3].brakeTorque = BrakingForce;

            ////���� �� 
            //wheelColliders[0].brakeTorque = BrakingForce;
            //wheelColliders[1].brakeTorque = BrakingForce;

            //// RWD
            //wheelColliders[2].motorTorque = 0;
            //wheelColliders[3].motorTorque = 0;

        }

        // release brakes 
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("## release brake~!");

            // spcaeŰ�� ���� �극��ũ�� 0���� ���� 
            for (int i = 0; i < wheelColliders.Length; i++)
            {
                wheelColliders[i].brakeTorque = 0f;
            }
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
}
