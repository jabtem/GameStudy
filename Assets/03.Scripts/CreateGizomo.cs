using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGizomo : MonoBehaviour
{
    public Color mycolor = Color.red;
    public float myradius = 0.05f;

    void OnDrawGizmos()
    {
        Gizmos.color = mycolor;
        Gizmos.DrawSphere(transform.position, myradius);
    }
}
