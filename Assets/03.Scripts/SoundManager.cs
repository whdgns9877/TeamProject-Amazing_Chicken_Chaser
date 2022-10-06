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
    [SerializeField] public AudioSource ClickReady;

    [Header("[ InGame Enviornment ]")]
    [SerializeField] public AudioSource StartGame;
    [SerializeField] public AudioSource Alive;
    [SerializeField] public AudioSource Bye;
    [SerializeField] public AudioSource Victory;
    [SerializeField] public AudioSource GetBettingCoin;
    
    [Header("[ InGame Play Sound ]")]
    [SerializeField] public AudioSource SoundGetItem;
    [SerializeField] public AudioSource JumpBooster;

    [SerializeField] public AudioSource SpeedBooster;
    [SerializeField] public AudioSource Missile;
    [SerializeField] public AudioSource Shield;
    [SerializeField] public AudioSource Fog;
    [SerializeField] public AudioSource Banana;

    [SerializeField] public AudioSource Boom;
    [SerializeField] public AudioSource FreezeBoom;
    [SerializeField] public AudioSource Slipped;

    [Header("[ Player Car Sound ]")]
    [SerializeField] public AudioSource Drift;
    [SerializeField] public AudioSource Crash;

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
