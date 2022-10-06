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
        //차 움직일 때 나는 소리 코드
        Debug.Log(PlayerSpeed);

        #region 차 움직이면 소리 실행
        if (PlayerSpeed >= 10f && PlayerSpeed <50f)
        {
            if (isDrive) return;
            isDrive = true;
        }
        else
        {
            isDrive = false;
        }
        if(PlayerSpeed >= 50f)
        {
            if (isAccel) return;
            isAccel = true;
        }
        else
        {
            isAccel = false;
        }
        #endregion
        
    }
}
