using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    [Header("[ BGM ]")]
    [SerializeField] public AudioSource StartBGM;
    [SerializeField] public AudioSource InGameBGM;

    [Header("[ Click Sound ]")]
    [SerializeField] public AudioSource ClickSound;
    [SerializeField] public AudioSource ClickNagative;

    [Header("[ InGame Enviornment ]")]
    [SerializeField] public AudioSource ReadyToStart;
    [SerializeField] public AudioSource RoundOver;
    [SerializeField] public AudioSource GameOver;
    [SerializeField] public AudioSource Victory;
    [Header("")]

    [SerializeField] public AudioSource JumpBooster;
    [SerializeField] public AudioSource SpeedBooster;

    [Header("[ Player Car Sound ]")]
    [SerializeField] public AudioSource Drive;
    [SerializeField] public AudioSource Accel;
    [SerializeField] public AudioSource Drift;
    [SerializeField] public AudioSource Honk;



    #region �̱���
    public static SoundManager instance = null;
    public static SoundManager Inst //ȣ���� ���� �̰� ȣ���ϰ�
    {
        get // �ܺο��� �� �Լ��� ������ �� ��
        {
            if(instance == null) // �������� ���� ����Ŵ����� ���ٸ�
            {
                instance = FindObjectOfType<SoundManager>(); //����Ŵ����� ã�Ƽ� �ִ´�
                if(instance == null) //�ٵ��� ���ٸ�
                {
                    instance = new GameObject("SoundManager").AddComponent<SoundManager>(); //SoundManager��� �̸��� ���ӿ�����Ʈ�� �����ϰ�, �ű�� ����Ŵ��� ������Ʈ �߰�
                }
            }
            DontDestroyOnLoad(instance);  //DontDestroyOnLoad �� �ν��Ͻ��� �ִ´�. = �� �̵��� �ǵ� ������� �ʵ���
            return instance; 
        }
    }
    #endregion




}
