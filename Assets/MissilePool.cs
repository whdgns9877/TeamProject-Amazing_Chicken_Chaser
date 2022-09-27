using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissilePool : MonoBehaviour
{

    //�̱��� �װ��� ���⼭ �ʿ������� �𸣰����� �ϴ� ����� ����.
    #region �̱���
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
