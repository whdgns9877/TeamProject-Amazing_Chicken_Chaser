using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomData : MonoBehaviour
{
    public string roomName = "";
    public int playerCount = 0;
    public int maxPlayer = 0;
    public bool isOpen;

    [System.NonSerialized]
    public TextMeshProUGUI roomDataText;

    void Awake()
    {
        roomDataText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateInfo()
    {
        roomDataText.text = string.Format(" {0} [{1} / {2}]", roomName, playerCount, maxPlayer);
    }
}
