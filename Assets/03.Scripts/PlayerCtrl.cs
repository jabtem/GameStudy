using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCtrl : MonoBehaviour
{
    private NavMeshAgent myTraceAgent;

    Vector3 movePoint = Vector3.zero;

    Ray ray;

    RaycastHit hitInfo1;

    [HideInInspector]
    public bool isDie;

    void Awake()
    {
        myTraceAgent = GetComponent<NavMeshAgent>();

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.blue);


#if UNITY_EDITOR

        if(Input.GetMouseButtonDown(0)&&!isDie)
        {
            if(Physics.Raycast(ray,out hitInfo1, Mathf.Infinity,1 <<LayerMask.NameToLayer("Barrel")))
            {
                movePoint = hitInfo1.point;
                myTraceAgent.destination = movePoint;
                myTraceAgent.stoppingDistance = 2.0f;//대상과 거리가 2미터쯤이면 정지
            }
            else if (Physics.Raycast(ray, out hitInfo1, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
            {
                movePoint = hitInfo1.point;

                myTraceAgent.destination = movePoint;
                myTraceAgent.stoppingDistance = 0.0f;

            }
        }
#endif
    }
}
