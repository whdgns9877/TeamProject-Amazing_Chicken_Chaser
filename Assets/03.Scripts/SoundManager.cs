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

    #region 싱글톤
    public static SoundManager instance = null;
    public static SoundManager Inst //호출할 때는 이걸 호출하고
    {
        get // 외부에서 이 함수를 가져다 쓸 때
        {
            if(instance == null) // 정적으로 만든 사운드매니저가 없다면
            {
                instance = FindObjectOfType<SoundManager>(); //사운드매니저를 찾아서 넣는다
                if(instance == null) //근데도 없다면
                {
                    instance = new GameObject("SoundManager").AddComponent<SoundManager>(); //SoundManager라는 이름의 게임오브젝트를 생성하고, 거기다 사운드매니저 컴포넌트 추가
                }
            }
            DontDestroyOnLoad(instance);  //DontDestroyOnLoad 에 인스턴스를 넣는다. = 씬 이동이 되도 사라지지 않도록
            return instance; 
        }
    }
    #endregion




}
