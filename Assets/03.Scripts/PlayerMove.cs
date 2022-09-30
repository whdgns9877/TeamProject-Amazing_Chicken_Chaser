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
    [SerializeField] public float Acceleration = 1000f;       // 자동차 속도
    [SerializeField] public float BrakingForce = 1000f;      // 브레이크 
    [SerializeField] public float MaxTurnAngle = 45f;        // 회전 각
    [SerializeField] public float MaxSpeed = 100f;

    [Header("Wheel Info")]
    [SerializeField] GameObject[] wheelMeshes = new GameObject[4];          // wheel mesh
    [SerializeField] WheelCollider[] wheelColliders = new WheelCollider[4]; // wheel collider
    [SerializeField] TrailRenderer[] trailrenderers = new TrailRenderer[4]; // wheel trailrenderer

    //미사일 관련 변수
    MissilePool missilePool;
    [SerializeField] public bool Me = false;

    float xAxis;
    float zAxis;

    // 동기화에 사용되는 포톤뷰
    PhotonView PV;

    //닉네임 
    UIPlayerInfo UIPlayerInfo;

    //============================================================
    // 움직임과 관련된 변수
    #region 움직임과 관련된 변수
    // 플레이어 Rigidbody 
    Rigidbody playerRigid;
    // 플레이어 무게 중심 
    public GameObject mycg;

    // 현재 속도
    float currSpeed;

    //플레이어 sideslipe 
    WheelFrictionCurve myFriction = new WheelFrictionCurve();
    #endregion
    //============================================================

    //============================================================
    // 맵에 배치된 발판들과 관련된 변수
    #region 발판 관련 변수
    [Header("MapTypeInfo")]

    [SerializeField] int jumpForceY;
    [SerializeField] int jumpForceZ;

    [SerializeField] int BoostForceZ;
    [SerializeField] int BoostForceZ2;

    [SerializeField] int jumpForceY2;
    [SerializeField] int jumpForceZ2;


    [SerializeField] int jumpForceY3;
    [SerializeField] int jumpForceZ3;


    [SerializeField] int jumpForceY4;
    [SerializeField] int jumpForceZ4;


    [SerializeField] int jumpForceY5;
    [SerializeField] int jumpForceZ5;

    [SerializeField] int jumpForceY6;
    [SerializeField] int jumpForceZ6;

    #endregion
    //============================================================

    //============================================================
    #region 아이템 관련 변수
    [SerializeField] bool missile = false;
    [SerializeField] bool shield = false;
    [SerializeField] bool mine = false;

    [SerializeField] GameObject MissileObj;



    #endregion
    //============================================================

    //============================================================
    // 치킨과 관련된 변수 

    Transform myChicken = null;
    Animator ChickenAni = null;


    //============================================================
    // Network 동기화를 위한 함수 
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
                {
                    ChickenAni = GetComponentInChildren<Animator>();
                    ChickenAni.SetBool("Turn Head", ((bool)stream.ReceiveNext())); 
                }
            }
        }
    }




    //============================================================

    private void Awake()
    {
        // 포톤뷰를 캐싱하여 가져온다 (GetComponent로 가져오는것 생각하면 됨)
        PV = photonView;
        missilePool = FindObjectOfType<MissilePool>().GetComponent<MissilePool>();
        playerRigid = GetComponent<Rigidbody>();

        UIPlayerInfo = GetComponentInChildren<UIPlayerInfo>();

        myChicken = transform.Find("MyChicken");

        MissileObj = Resources.Load<GameObject>("Missile");


        //===============     차가 나라면 나는 나만의 태그를 가질 것이여     ==================
        if (PV.IsMine)
        {
            gameObject.tag = "Me";
            for (int i = 0; i < 7; i++) //자식들도 전부 나로 태그를 바꾼다(치킨이 7번째니까 그 전까지 싹 다)
            {
                transform.GetChild(i).gameObject.tag = "Me";
            }
        }
        else
        {
            gameObject.tag = "Player";
            gameObject.layer = LayerMask.NameToLayer("Player");
            for (int i = 0; i < 7; i++) //자식들도 전부 나로 태그를 바꾼다(치킨이 7번째니까 그 전까지 싹 다)
            {
                transform.GetChild(i).gameObject.tag = "Player";
                gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
        }
    }


    void Start()
    {
        UIPlayerInfo.NickName(photonView.Controller.NickName);

    }

    //==============================================      UPDATE      ===========================================
    private void Update()
    {
        if (!PV.IsMine)
            return;

        //input keys 
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");

        //=====================================================================
        // Player braking
        #region 플레이어 브레이크와 관련된 코드 

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

        #endregion
        //=====================================================================

        //=====================================================================
        #region 아이템 관련 스크립트
<<<<<<< HEAD
        //if(missile == true)
        {
            if(Input.GetKeyDown(KeyCode.LeftControl))
            {
                //if (transform.Find("Missile").gameObject.activeSelf == true) //활성화 중이라면 반환
                //return;
                //Instantiate(MissileObj, transform.position, Quaternion.identity);
                //Debug.Log(missilePool);
                //missilePool.OrderInst(transform, PV.ViewID);
                //transform.GetComponentInChildren<MeshCollider>().enabled = false;
                GameObject myMissile = PhotonNetwork.Instantiate("Missile", transform.position + new Vector3(0, 0.4f, 0f), transform.rotation);
                myMissile.AddComponent<Missile>();
                myMissile.transform.position = transform.position + new Vector3(0, 0.4f, 0f);
                myMissile.transform.rotation = Quaternion.LookRotation(transform.forward);
            }
            else
            {
                //transform.GetComponentInChildren<MeshCollider>().enabled = true;
=======
        if(missile == true)
        {
            if(Input.GetKeyDown(KeyCode.LeftControl))
            {
                Instantiate(MissileObj, transform.position, Quaternion.identity);
>>>>>>> 1ed9c9b0673d4d2198759fe19ee23431cbf69fe0
            }
        }
        #endregion
    }


    private void FixedUpdate()
    {
        // 본인의 제어권 안쪽만 실행
        if (!PV.IsMine)
            return;

        //=====================================================================
        //Player car movment 
        #region 플레이어 움직임에 대한 코드
        // move C.G of vehicle
        playerRigid.centerOfMass = mycg.transform.localPosition;

        

        // currentspeed of car 
        currSpeed = (float)(playerRigid.velocity.magnitude * 3.6f);

        //if has horizontal key input, Steer wheels 
        if (xAxis != 0f)
        {
            // 전방 휠 움직임 
            wheelColliders[0].steerAngle = xAxis * MaxTurnAngle;    // front left wheel 
            wheelColliders[1].steerAngle = xAxis * MaxTurnAngle;    // front right wheel
        }

        else
        {
            // 전방 휠 움직임 
            wheelColliders[0].steerAngle = 0f;    // front left wheel 
            wheelColliders[1].steerAngle = 0f;    // front right wheel
        }

        if (zAxis != 0f && currSpeed <= MaxSpeed)
        {
            // 토크를 주는 휠 
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
            // wheel collider에 맞춰 wheel mesh를 움직일 수 있도록 함 
            UpdateWheelPos(wheelColliders[i], wheelMeshes[i].transform);
        }
        #endregion
        //=====================================================================

    }


    //If get Chicken, active mychicken
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Chicken")
        {
            if (myChicken.gameObject.activeSelf)
                return; 

            myChicken.gameObject.SetActive(true);
            ChickenAni = GetComponentInChildren<Animator>();
            ChickenAni.SetBool("Turn Head", true);

            //Let GameManager to deactivate chicken from chicken pool on the map.
            GameManager.Inst.Destroy(collision.gameObject);

        }

        if (collision.collider.tag == "Player")
        {
            // if detected player's MyChicken is not activated, drop the chicken 
            if (collision.transform.Find("MyChicken").gameObject.activeSelf)
                return;

            if (!PhotonNetwork.IsMasterClient)
                return; 

                // deactive MyChicken
                GameManager.Inst.Instantiate("Chicken", transform.position + Vector3.up * 3f + Vector3.forward * 1f, Quaternion.identity);
            
        }
    }


    //===========================================================================
    // function related to car movement 
    #region 자동차 움직임에 관한 함수들 

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
<<<<<<< HEAD
    #region 충돌 관련 함수
    private void OnTriggerEnter(Collider other)
    {
        if (!PV.IsMine)
            return;
        //=======================================
        #region 미사일 충돌(아이템 충돌)
        if(other.gameObject.tag == "Bomb")
        {
            if (!other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log("미사일충돌!!");
                playerRigid.AddExplosionForce(500000f, transform.position, 10f, 100f);
            }
        }
        #endregion


        //=======================================
        #region 발판 충돌
=======
    #region 발판 관련 함수
    private void OnTriggerEnter(Collider other)
    {
>>>>>>> 1ed9c9b0673d4d2198759fe19ee23431cbf69fe0
        if (other.gameObject.tag == "Jump") //점프발판대 밟았을때
        {
            GetComponent<Rigidbody>().AddForce(0, jumpForceY, jumpForceZ);
        }
        if (other.gameObject.tag == "Jump2") //점프발판대 밟았을때
        {
            GetComponent<Rigidbody>().AddForce(0, jumpForceY2, jumpForceZ2);
        }
        if (other.gameObject.tag == "Jump3") //점프발판대 밟았을때
        {
            GetComponent<Rigidbody>().AddForce(0, jumpForceY3, jumpForceZ3);
        }
        if (other.gameObject.tag == "Jump4") //점프발판대 밟았을때
        {
            GetComponent<Rigidbody>().AddForce(0, jumpForceY4, jumpForceZ4);
        }
        if (other.gameObject.tag == "Jump5") //점프발판대 밟았을때
        {
            GetComponent<Rigidbody>().AddForce(0, jumpForceY5, jumpForceZ5);
        }
        if (other.gameObject.tag == "Jump6") //점프발판대 밟았을때
        {
            GetComponent<Rigidbody>().AddForce(0, jumpForceY6, jumpForceZ6);
        }

        if (other.gameObject.tag == "Boost") //부스트발판 밟았을때
        {
            GetComponent<Rigidbody>().AddRelativeForce(0, 0, BoostForceZ);
        }
        if (other.gameObject.tag == "Boost2") //부스트발판 밟았을때
        {
            GetComponent<Rigidbody>().AddRelativeForce(0, 0, BoostForceZ2);
        }
<<<<<<< HEAD
        #endregion
        //=======================================
    }
    #endregion
    //===========================================================================




=======

    }
    #endregion
    //===========================================================================
>>>>>>> 1ed9c9b0673d4d2198759fe19ee23431cbf69fe0

}

