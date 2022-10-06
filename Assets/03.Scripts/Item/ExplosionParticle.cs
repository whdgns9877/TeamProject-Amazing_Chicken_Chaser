using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticle : MonoBehaviour
{
    [SerializeField] float destroyTime;
    // Start is called before the first frame update
    private void OnEnable()
    {
        Destroy(gameObject, destroyTime);
    }
}
