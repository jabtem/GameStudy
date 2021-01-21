using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class doorMove : MonoBehaviour
{
    private NavMeshAgent myTraceAgent;
    private OffMeshLink offmesh;

    void Awake()
    {
        myTraceAgent = transform.Find("autoDoor").GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
