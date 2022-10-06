using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Object", order = 1)]
public class ItemScriptableObject : ScriptableObject
{
    [SerializeField]
    private string itemName;
    public string ItemName {get { return itemName; } }

    [SerializeField]
    private int itemNum;
    public int ItemNum { get { return itemNum; } }

    [SerializeField]
    private int itemCount;
    public int ItemCount { get { return itemCount; } }

}
