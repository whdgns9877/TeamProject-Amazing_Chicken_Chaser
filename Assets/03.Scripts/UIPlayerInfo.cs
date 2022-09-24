using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtNickName = null;



    public void NickName(string name)
    {
        txtNickName.text = name;
    }



    // Update is called once per frame
    void Update()
    {
        // rotate canvas towards to camera
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
