using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField]
    private ItemScriptableObject itemScriptableObject;
    public ItemScriptableObject ItemScriptableObject { set { itemScriptableObject = value; } }



}
