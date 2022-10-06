using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxSpawn : MonoBehaviour
{
    public void RegenBox(GameObject box)
    {
        StartCoroutine(boxRegen(box));
    }

    IEnumerator boxRegen(GameObject box)
    {
        yield return new WaitForSeconds(5f);
        box.SetActive(true);
    }
}
