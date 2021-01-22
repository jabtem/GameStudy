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
        offmesh = gameObject.GetComponent<OffMeshLink>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
