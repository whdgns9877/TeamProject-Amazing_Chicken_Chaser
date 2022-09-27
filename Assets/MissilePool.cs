using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissilePool : MonoBehaviour
{

    //싱글톤 그것이 여기서 필요한지는 모르겠으나 일단 만들고 본다.
    #region 싱글톤
    static MissilePool instance = null;
    public static MissilePool Inst
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<MissilePool>();
                if (instance == null)
                    instance = new GameObject("MissilePool").AddComponent<MissilePool>();
            }
            return instance; 
        }
    }
    #endregion
    //========================================================

    [SerializeField] Missile missilePrefab = null;
    Queue<Missile> pool = new Queue<Missile>();

}
