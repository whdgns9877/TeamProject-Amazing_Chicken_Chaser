using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtNickName     = null;
    [SerializeField] Image           ImageForMinimap = null;



    public void NickName(string name)
    {
        txtNickName.text = name;
    }

    public void SetMinimapImageColor(Color color)
    {
        ImageForMinimap.color = color;
    }


    // Update is called once per frame
    void Update()
    {
        // rotate canvas towards to camera
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
