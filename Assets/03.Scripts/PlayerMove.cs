using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Runtime.ExceptionServices;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Photon.Pun.Demo.Cockpit;
using Cinemachine;
using TMPro;
using Color = UnityEngine.Color;


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

    float xAxis;
    float zAxis;


    // ����ȭ�� ���Ǵ� �����
    PhotonView PV;

    //�г��� 
    UIPlayerInfo UIPlayerInfo;


    //GameObject VictoryText = null;
    GameObject GGText = null;
    TextMeshProUGUI GameOvertext;

    //============================================================
    // �����Ӱ� ���õ� ����
    #region �����Ӱ� ���õ� ����
    // �÷��̾� Rigidbody 
    Rigidbody playerRigid;
    // �÷��̾� ���� �߽� 
    public GameObject mycg;

    // ���� �ӵ�
    public float currSpeed;

    // ���� ���� �Ǽ��� ��
    public float currAccel = 0f;
    public float currBackAccel = 0f;

    //�÷��̾� sideslipe 
    WheelFrictionCurve myFriction = new WheelFrictionCurve();
    #endregion
    //============================================================

    //============================================================
    // �ʿ� ��ġ�� ���ǵ�� ���õ� ����
    #region ���� ���� ����
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
    #region ������ ���� ����
    [SerializeField] bool missile = false;
    [SerializeField] public bool shield = false;
    [SerializeField] public bool booster = false;
    [SerializeField] public bool banana = false;

    [SerializeField] bool mine = false;

    #endregion
    //============================================================

    //============================================================
    #region ġŲ�� ���õ� ���� 
    Transform myChicken = null;
    Animator ChickenAni = null;
    #endregion
    //============================================================

    //============================================================
    #region �÷��̾� ��ġ �ʱ�ȭ�� ���� ����
    private bool canMove = true;
    #endregion
    //============================================================

    bool check = false;
    bool winner = false;
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

            }
        }

        else
        {
            //remote player, receive data
            for (int i = 0; i < 4; i++)
            {
                trailrenderers[i].emitting = (bool)stream.ReceiveNext();
                myChicken.gameObject.SetActive((bool)stream.ReceiveNext());

            }
        }
    }

    //============================================================

    private void Awake()
    {
        // ����並 ĳ���Ͽ� �����´� (GetComponent�� �������°� �����ϸ� ��)
        PV = photonView;
        playerRigid = GetComponent<Rigidbody>();

        UIPlayerInfo = GetComponentInChildren<UIPlayerInfo>();

        myChicken = transform.Find("MyChicken");

        ChickenAni = myChicken.GetComponent<Animator>();

        //VictoryText = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        GGText = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
        GameOvertext = GGText.GetComponent<TextMeshProUGUI>();


        // �ڽ��� �±׸� �ٲ��ִ� �κ�

        if (PV.IsMine)
        {
            gameObject.tag = "Me";
            for (int i = 0; i < 7; i++) //�ڽĵ鵵 ���� ���� �±׸� �ٲ۴�(ġŲ�� 7��°�ϱ� �� ������ �� ��)
            {
                transform.GetChild(i).gameObject.tag = "Me";
            }
        }
        else
        {
            gameObject.tag = "Player";
            gameObject.layer = LayerMask.NameToLayer("Player");
            for (int i = 0; i < 7; i++) //�ڽĵ鵵 ���� ���� �±׸� �ٲ۴�(ġŲ�� 7��°�ϱ� �� ������ �� ��)
            {
                transform.GetChild(i).gameObject.tag = "Player";
            }
        }
    }





    void Start()
    {
        UIPlayerInfo.NickName(photonView.Controller.NickName);

        if (PV.IsMine)
            UIPlayerInfo.SetMinimapImageColor(Color.blue);
        else
            UIPlayerInfo.SetMinimapImageColor(Color.red);

        Debug.Log("my ID " + PV.ViewID);

    }

    //==============================================      UPDATE      ===========================================
    private void Update()
    {
        if (!PV.IsMine || !canMove)
            return;

        //=====================================================================
        // Imsi Code for Siyeon
        #region Temp key for test
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ChickenTimer.Inst.StopAllCoroutines();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            LeftGame();
        }

        #endregion
        //=====================================================================


        //=====================================================================
        // Player GameOver
        if (ChickenTimer.Inst.IsGameOver == false && check == false)
        {
            Debug.Log("game is over!");

            // check if I have chicken 
            StartCoroutine(CheckHasChicken());

            check = true;
        }


        //=====================================================================

        //=====================================================================
        // Player Pos Reset
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ResetPlayerPos());
        }
        //=====================================================================

        //=====================================================================
        // Player braking
        #region �÷��̾� ������, �극��ũ�� ���õ� �ڵ� 

        //input keys 
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");

        // Apply brakes
        if (Input.GetKey(KeyCode.Space))
        {
            // Rear Wheel Brake
            wheelColliders[2].brakeTorque = BrakingForce;
            wheelColliders[3].brakeTorque = BrakingForce;
            //start making skidmarks
            SkidMark(0, true);

            // play eating animation
            PV.RPC("ChickenPadak", RpcTarget.AllViaServer, "Eat", true);
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

            // stop eating animation
            PV.RPC("ChickenPadak", RpcTarget.AllViaServer, "Eat", false);
        }

        // drift key "shift"
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Drift(0.5f);        // decrease wheel stiffness for drifting
            SkidMark(2, true);  // skidmark on

            // play running animation during drift
            PV.RPC("ChickenPadak", RpcTarget.AllViaServer, "Run", true);
        }

        // drift key "shift"
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Drift(5f);          // increase wheel stiffness to stop drifting
            SkidMark(2, false); // skidmark off

            // stop running animation 
            PV.RPC("ChickenPadak", RpcTarget.AllViaServer, "Run", false);
        }

        // if wheel is off ground, no skidmark
        EraseSkidMark();


        // if Forward Left & rear right wheel is off the ground 
        if (!wheelColliders[0].isGrounded && wheelColliders[3].isGrounded)
            PV.RPC("ChickenPadak", RpcTarget.AllViaServer, "Fly", true);

        else
            PV.RPC("ChickenPadak", RpcTarget.AllViaServer, "Fly", false);

        #endregion
        //=====================================================================

        //=====================================================================
        #region ������ ���� ��ũ��Ʈ
        // �̻��� �߻�
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            GameObject myMissile = PhotonNetwork.Instantiate("Missile", transform.position + new Vector3(0f, 0.4f, 0f), transform.rotation);
            myMissile.AddComponent<Missile>();
            myMissile.transform.position = transform.position + new Vector3(0, 0.4f, 0f);
            myMissile.transform.rotation = Quaternion.LookRotation(transform.forward);
        }

        // ice bullet 
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameObject myMissile = PhotonNetwork.Instantiate("Freeze", transform.position + new Vector3(0f, 0.4f, 0.1f), transform.rotation * Quaternion.Euler(-50f, 0f, 0f));
            myMissile.AddComponent<Freeze>();
            myMissile.transform.position = transform.position + new Vector3(0, 0.4f, 1f);
            myMissile.transform.rotation = Quaternion.LookRotation(transform.forward);
        }

        // shield 
        if (Input.GetKeyDown(KeyCode.T))
        {
            PV.RPC("ShieldActive", RpcTarget.AllViaServer);
            shield = true;
        }

        // booster
        if (Input.GetKeyDown(KeyCode.B))
        {
            PV.RPC("BoosterActive", RpcTarget.AllViaServer);
            booster = true;
        }

        // banana
        if (Input.GetKeyDown(KeyCode.N))
        {
            GameObject myBanana = PhotonNetwork.Instantiate("Banana", transform.position + (-transform.forward * 2), Quaternion.Euler(90f, 0f, 0f));
            myBanana.AddComponent<Banana>();
        }

        // smoke 
        if (Input.GetKeyDown(KeyCode.M))
        {
            PhotonNetwork.Instantiate("Smoke", transform.position, Quaternion.Euler(0f, 0f, 0f));
        }
        #endregion
        //=====================================================================



    }



    private void FixedUpdate()
    {
        // ������ ����� ���ʸ� ����
        // if game is not started, do not move 
        if (!PV.IsMine || !ChickenTimer.Inst.GameStart)
            return;


        //=====================================================================
        // Player car movment 
        #region �÷��̾� �����ӿ� ���� �ڵ�
        // move C.G of vehicle
        playerRigid.centerOfMass = mycg.transform.localPosition;



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
        #endregion
        //=====================================================================

    }


    //If get Chicken, active mychicken
    private void OnCollisionEnter(Collision collision)
    {


        //===========================================================================
        // Chicken detection
        #region ġŲ �浹, �÷��̾�� �浹 �� ġŲ ó�� ���� �ڵ� 
        if (collision.collider.tag == "Chicken")
        {
            PhotonView colliPV = collision.gameObject.GetComponent<PhotonView>();

            // If I have chicken
            if (myChicken.gameObject.activeSelf)
                return;

            PV.RPC("MyChicken", RpcTarget.AllViaServer, true);

            //Let ChickenSpawn deactivates chicken from chicken pool on the map.
            PV.RPC("DestroyChicken", RpcTarget.AllViaServer, colliPV.ViewID);
        }

        if (collision.collider.tag == "Player")
        {
            PhotonView colliPV = collision.gameObject.GetComponent<PhotonView>();

            // if I have chicken and opponent have chicken too 
            if (myChicken.gameObject.activeSelf && collision.transform.Find("MyChicken").gameObject.activeSelf)
                return;

            // if I don't have a chicken, opponent doesn't have chicken too 
            else if (!myChicken.gameObject.activeSelf && !collision.transform.Find("MyChicken").gameObject.activeSelf)
                return;

            else
            {
                // if I have chicken
                if (myChicken.gameObject.activeSelf)
                {
                    // deactive my chicken 
                    PV.RPC("MyChicken", RpcTarget.AllViaServer, false);

                    // request master client to active chicken 
                    PV.RPC("CreateChicken", RpcTarget.AllViaServer, transform.position);
                }
            }
        }
        #endregion
        //===========================================================================



    }

    //===========================================================================
    // Pun RPCs 
    #region ġŲ�� ���õ� RPC

    [PunRPC]
    public void MyChicken(bool has)
    {
        myChicken.gameObject.SetActive(has);
    }

    // play chicken animation
    [PunRPC]
    public void ChickenPadak(string dosomething, bool doing)
    {
        // if I have no chicken, return
        if (!myChicken.gameObject.activeSelf)
            return;

        ChickenAni.SetBool($"{dosomething}", doing);
    }

    [PunRPC]
    public void DestroyChicken(int ChickenID)
    {
        GameObject Chicken = PhotonView.Find(ChickenID).gameObject;
        ChickenSpawn.Inst.Destroy(Chicken);
    }

    [PunRPC]
    public void CreateChicken(Vector3 where)
    {
        ChickenSpawn.Inst.Instantiate("Chicken", where, Quaternion.identity);
    }
    #endregion
    //===========================================================================



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
    private void OnTriggerEnter(Collider other)
    {
        //=======================================
        // ���� ���� �κ� 
        #region jumps in the map 
        if (other.gameObject.tag == "Jump") //�������Ǵ� �������
        {
            GetComponent<Rigidbody>().AddForce(0, jumpForceY, jumpForceZ);
        }
        if (other.gameObject.tag == "Jump2") //�������Ǵ� �������
        {
            GetComponent<Rigidbody>().AddForce(0, jumpForceY2, jumpForceZ2);
        }
        if (other.gameObject.tag == "Jump3") //�������Ǵ� �������
        {
            GetComponent<Rigidbody>().AddForce(0, jumpForceY3, jumpForceZ3);
        }
        if (other.gameObject.tag == "Jump4") //�������Ǵ� �������
        {
            GetComponent<Rigidbody>().AddForce(0, jumpForceY4, jumpForceZ4);
        }
        if (other.gameObject.tag == "Jump5") //�������Ǵ� �������
        {
            GetComponent<Rigidbody>().AddForce(0, jumpForceY5, jumpForceZ5);
        }
        if (other.gameObject.tag == "Jump6") //�������Ǵ� �������
        {
            GetComponent<Rigidbody>().AddForce(0, jumpForceY6, jumpForceZ6);
        }

        if (other.gameObject.tag == "Boost") //�ν�Ʈ���� �������
        {
            GetComponent<Rigidbody>().AddRelativeForce(0, 0, BoostForceZ);
        }
        if (other.gameObject.tag == "Boost2") //�ν�Ʈ���� �������
        {
            GetComponent<Rigidbody>().AddRelativeForce(0, 0, BoostForceZ2);
        }
        #endregion
        //=======================================

        //=======================================
        // �̻��� ���� �κ� 

        if (!PV.IsMine)
            return;

        // attacked by missile
        if (other.gameObject.tag == "Bomb" && shield == false)
        {
            if (!other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                playerRigid.AddExplosionForce(500000f, transform.position, 10f, 100f);
            }
            // if I have chicken
            if (myChicken.gameObject.activeSelf)
            {
                // deactive my chicken 
                PV.RPC("MyChicken", RpcTarget.AllViaServer, false);

                // request master client to active chicken 
                PV.RPC("CreateChicken", RpcTarget.AllViaServer, transform.position);
            }
        }

        // attacked by ice bullet 
        if (other.gameObject.tag == "Freeze" && shield == false)
        {
            if (!other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                StartCoroutine(SlowTime());
            }
        }

        // stepped on banana
        if (other.gameObject.tag == "Banana")
        {
            Drift(-10f);
            StartCoroutine(BananaSliding());
        }

    }
    //===========================================================================

    //===========================================================================
    // function related to player reset pos
    IEnumerator ResetPlayerPos()
    {
        canMove = false;

        transform.position = Vector3.up;
        transform.rotation = Quaternion.identity;
        playerRigid.velocity = Vector3.zero;

        yield return new WaitForSeconds(3f);
        canMove = true;
    }
    //===========================================================================

    //===========================================================================
    // function Check my Chicken Onw when ChickenTimer is Finish
    IEnumerator CheckHasChicken()
    {
        // doAgain == false == no one got chicken
        if (GameManager.Inst.DoAgain)
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            GGText.SetActive(true);
            GameOvertext.faceColor = Color.blue;
            GameOvertext.text = "All Chickens ran away.. \n Let's try again!!";


            yield return new WaitForSeconds(2f);
            PhotonNetwork.LoadLevel("GameScene");

            Debug.Log("Do again!");
            yield return null;
        }

        else
        {
            Debug.Log("go next!");
            // no one got chicken, re-start round 
            if (!transform.GetChild(7).gameObject.activeInHierarchy)
            {
                StartCoroutine(GoodGame("You Missed Chicken!", "StartScene", Color.red, 2f));
            }

            // if player has chicken 
            if (transform.GetChild(7).gameObject.activeInHierarchy)
            {
                GGText.SetActive(true);
                GameOvertext.faceColor = Color.yellow;
                GameOvertext.text = "Lucky! You Got Chicken!";
            }

            yield return new WaitForSeconds(5f);

            // checked chicken and only one player left 
            if (check && PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                winner = true;
                Debug.Log("????" + winner);
                ZeraAPIHandler.Inst.DeclareWinner();
                StartCoroutine(GoodGame("You Win!!", "StartScene", Color.green, 2f));
            }


            yield return new WaitForSeconds(2f);

            if (check && !winner && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
            {
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.LoadLevel("GameScene");
            }
            yield return null;
        }
    }

    // ���� ������ �Լ�
    private void LeftGame()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("StartScene");
    }

    // ���� ���� �� winner/loser text ��� �� �� ��ȯ �Լ� 
    IEnumerator GoodGame(string text, string scene, Color color, float wait)
    {
        // set active GG text 
        GGText.SetActive(true);

        GameOvertext.text = text;
        GameOvertext.faceColor = color;
        yield return new WaitForSeconds(wait);

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(scene);

        yield return null;
    }

    //===========================================================================
    /// <summary>
    /// all item functions 
    /// </summary>
    #region function related to items 
    [PunRPC]
    void ShieldActive() // active shield
    {
        transform.GetChild(9).gameObject.SetActive(true);
    }

    [PunRPC]
    void BoosterActive() // active booster
    {
        transform.GetChild(8).gameObject.SetActive(true);
    }

    // coroutine related to banana
    IEnumerator BananaSliding() // slip by stepping banana
    {
        yield return new WaitForSeconds(0.5f);
        Drift(5f); // reset drift value 
    }

    IEnumerator SlowTime() // slow motion
    {
        wheelColliders[0].brakeTorque = -10f;
        wheelColliders[1].brakeTorque = -10f;
        wheelColliders[2].brakeTorque = -10f;
        wheelColliders[3].brakeTorque = -10f;
        yield return new WaitForSeconds(3f);
    }
    #endregion

    //===========================================================================
}

