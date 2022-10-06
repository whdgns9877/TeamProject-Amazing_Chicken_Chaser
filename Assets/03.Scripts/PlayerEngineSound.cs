using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEngineSound : MonoBehaviour
{
    float PlayerSpeed;
    bool isDrive = false;
    bool isAccel = false;

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //�� ������ �� ���� �Ҹ� �ڵ�
        Debug.Log(PlayerSpeed);

        #region �� �����̸� �Ҹ� ����
        if (PlayerSpeed >= 10f && PlayerSpeed <50f)
        {
            if (isDrive) return;
            isDrive = true;
            SoundManager.Inst.Drive.Play();
        }
        else
        {
            SoundManager.Inst.Drive.Stop();
            isDrive = false;
        }
        if(PlayerSpeed >= 50f)
        {
            if (isAccel) return;
            isAccel = true;
            SoundManager.Inst.Accel.Play();
        }
        else
        {
            SoundManager.Inst.Accel.Stop();
            isAccel = false;
        }
        #endregion
        
    }
}
