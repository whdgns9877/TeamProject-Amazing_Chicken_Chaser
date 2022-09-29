using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    NavMeshAgent nav;
    [SerializeField] GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        target = FindObjectOfType<PlayerMove>().gameObject;

    }
    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = target.transform.position;
        
        nav.SetDestination(targetPos);
    }
}
