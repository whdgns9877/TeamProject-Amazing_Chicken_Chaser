using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticle : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        Destroy(gameObject, 1f);
    }
}
