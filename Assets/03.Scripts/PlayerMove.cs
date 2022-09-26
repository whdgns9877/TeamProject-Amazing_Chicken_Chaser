using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Photon.Pun;
using System.Runtime.ExceptionServices;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Photon.Pun.Demo.Cockpit;

public class PlayerMove : MonoBehaviourPun, IPunObservable
{
    [Header("Car Info")]
    [SerializeField] public float Acceleration = 1000f;       // �ڵ��� �ӵ�
    [SerializeField] public float BrakingForce = 1000f;      // �극��ũ 
    [SerializeField] public float MaxTurnAngle = 45f;        // ȸ�� ��
    [SerializeField] public float MaxSpeed = 100f;

    [Header("Wheel Info")]
    [SerializeField] GameObject[] wheelMeshes = new GameObject[4];          // wheel mesh
    [SerializeField] WheelCollider[] wheelColliders = new WheelCollider[4]; // wheel collider
    [SerializeField] TrailRenderer[] trailrenderers = new TrailRenderer[4]; // wheel trailrenderer

    // ����ȭ�� ���Ǵ� �����
    PhotonView PV;

    //�г��� 
    //UIPlayerInfo UIPlayerInfo;

    //============================================================
    // �����Ӱ� ���õ� ����
    #region �����Ӱ� ���õ� ����
    // �÷��̾� Rigidbody 
    Rigidbody playerRigid;
    // �÷��̾� ���� �߽� 
    public GameObject mycg;

    // ���� �ӵ�
    float currSpeed;

    //�÷��̾� sideslipe 
    WheelFrictionCurve myFriction = new WheelFrictionCurve();
    #endregion
    //============================================================

    //============================================================
    // �ʿ� ��ġ�� ���ǵ�� ���õ� ����
    #region ���� ���� ����
    [Header("MapTypeInfo")]
    [SerializeField] int jumpForceX;
    [SerializeField] int jumpForceY;
    [SerializeField] int jumpForceZ;

    [SerializeField] int BoostForceZ;
    [SerializeField] int BoostForceZ2;

    [SerializeField] int jumpForceY2;
    [SerializeField] int jumpForceZ2;

    [SerializeField] int jumpForceX3;
    [SerializeField] int jumpForceY3;
    [SerializeField] int jumpForceZ3;

    [SerializeField] int jumpForceX4;
    [SerializeField] int jumpForceY4;
    [SerializeField] int jumpForceZ4;

    [SerializeField] int jumpForceX5;
    [SerializeField] int jumpForceY5;
    [SerializeField] int jumpForceZ5;

    #endregion
    //============================================================

    //============================================================
    // ġŲ�� ���õ� ���� 

    Transform myChicken = null;
    Animator ChickenAni = null;


    //============================================================
    // Network ����ȭ�� ���� �Լ� 
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // this is my player, sned data to other players 
            for (int i = 0; i < 4; i++)
            {
                stream.SendNext(trailrenderers[i].emitting);
                stream.SendNext(myChicken.gameObject.activeSelf);
                if (myChicken.gameObject.activeSelf)
                { stream.SendNext(ChickenAni.GetBool("Turn Head")); }
            }
        }

        else
        {
            //remote player, receive data 
            for (int i = 0; i < 4; i++)
            {
                trailrenderers[i].emitting = (bool)stream.ReceiveNext();
                myChicken.gameObject.SetActive((bool)stream.ReceiveNext());
                if (myChicken.gameObject.activeSelf)
                { ChickenAni.SetBool("Turn Head", ((bool)stream.ReceiveNext())); }
            }
        }
    }

    private void Awake()
    {
        // ����並 ĳ���Ͽ� �����´� (GetComponent�� �������°� �����ϸ� ��)
        PV = photonView;
        playerRigid = GetComponent<Rigidbody>();

        //UIPlayerInfo = GetComponentInChildren<UIPlayerInfo>();

        myChicken = transform.Find("MyChicken");
    }










    void Start()
    {
        //UIPlayerInfo.NickName(photonView.Controller.NickName);
    }

    private void Update()
    {
        if (!PV.IsMine)
            return;

        // Apply brakes
        if (Input.GetKey(KeyCode.Space))
        {
            // Rear Wheel Brake
            wheelColliders[2].brakeTorque = BrakingForce;
            wheelColliders[3].brakeTorque = BrakingForce;
            //start making skidmarks
            SkidMark(0, true);
        }

        // release brakes 
        if (Input.GetKeyUp(KeyCode.Space))
        {
            wheelColliders[0].brakeTorque = 0f;
            wheelColliders[1].brakeTorque = 0f;
            wheelColliders[2].brakeTorque = 0f;
            wheelColliders[3].brakeTorque = 0f;
            //start making skidmarks
            SkidMark(0, false);
        }

        // drift key "shift"
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Drift(0.1f);        // decrease wheel stiffness for drifting
            SkidMark(2, true);  // skidmark on
        }

        // drift key "shift"
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Drift(5f);          // increase wheel stiffness to stop drifting
            SkidMark(2, false); // skidmark off
        }

        // if wheel is off ground, no skidmark
        EraseSkidMark();
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

        // currentspeed of car 
        currSpeed = (float)(playerRigid.velocity.magnitude * 3.6f);

        //if has horizontal key input, Steer wheels 
        if (xAxis != 0f)
        {
            // ���� �� ������ 
            wheelColliders[0].steerAngle = xAxis * MaxTurnAngle;    // front left wheel 
            wheelColliders[1].steerAngle = xAxis * MaxTurnAngle;    // front right wheel
        }

        else
        {
            // ���� �� ������ 
            wheelColliders[0].steerAngle = 0f;    // front left wheel 
            wheelColliders[1].steerAngle = 0f;    // front right wheel
        }

        if (zAxis != 0f && currSpeed <= MaxSpeed)
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
        }
    }


    //If get Chicken, active mychicken
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Chicken")
        {
            myChicken.gameObject.SetActive(true);
            ChickenAni = GetComponentInChildren<Animator>();
            ChickenAni.SetBool("Turn Head", true);

            // Send message to GameManager to deactivate chicken from chicken pool on the map.
            gameObject.SendMessage("GotChicken");
        }

        if (collision.collider.tag == "Player")
        {
            myChicken.gameObject.SetActive(false);

            // send meesage to GameManager to activate chicken from chicken pool on the map
            gameObject.SendMessage("DropTheChicken", myChicken.gameObject);
        }
    }





    //===========================================================================
    // function related to car movement 
    #region �ڵ��� �����ӿ� ���� �Լ��� 

    // to move wheelmash as wheelCollider moves  
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

    void Drift(float stiffness)
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
            myFriction.stiffness = stiffness;

            wheelColliders[i].sidewaysFriction = myFriction;
        }
    }

    // which wheel to be emitted, and how many wheels? 
    void SkidMark(int wheel, bool emitting)
    {

        // set trailrenderer emitting
        for (int i = wheel; i < trailrenderers.Length; i++)
        {
            if (wheelColliders[i].isGrounded)
                trailrenderers[i].emitting = emitting;      // selected wheel only
        }
    }

    // if wheel is not grounded, turn trailrenderer emitting off
    void EraseSkidMark()
    {

        for (int i = 0; i < trailrenderers.Length; i++)
        {
            if (!wheelColliders[i].isGrounded)
                trailrenderers[i].emitting = false;
        }
    }

    #endregion
    //===========================================================================

    //===========================================================================
    #region ���� ���� �Լ���
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Jump") //�������Ǵ� �������
        {
            GetComponent<Rigidbody>().AddForce(jumpForceX, jumpForceY, jumpForceZ);
        }
        if (other.gameObject.tag == "Jump2") //�������Ǵ� �������
        {
            GetComponent<Rigidbody>().AddForce(jumpForceX, jumpForceY2, jumpForceZ2);
        }
        if (other.gameObject.tag == "Jump3") //�������Ǵ� �������
        {
            GetComponent<Rigidbody>().AddForce(jumpForceX3, jumpForceY3, jumpForceZ3);
        }
        if (other.gameObject.tag == "Jump4") //�������Ǵ� �������
        {
            GetComponent<Rigidbody>().AddForce(jumpForceX4, jumpForceY4, jumpForceZ4);
        }
        if (other.gameObject.tag == "Jump5") //�������Ǵ� �������
        {
            GetComponent<Rigidbody>().AddForce(jumpForceX5, jumpForceY5, jumpForceZ5);
        }


        if (other.gameObject.tag == "Boost") //�ν�Ʈ���� �������
        {
            GetComponent<Rigidbody>().AddRelativeForce(0, 0, BoostForceZ);
        }
        if (other.gameObject.tag == "Boost2") //�ν�Ʈ���� �������
        {
            GetComponent<Rigidbody>().AddRelativeForce(0, 0, BoostForceZ2);
        }
    }
    #endregion
    //===========================================================================

}
