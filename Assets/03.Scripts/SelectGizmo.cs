using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectGizmo : MonoBehaviour
{
    public Color mycolor = Color.red;
    public float explosionRaidus = 7.0f;

    void OnDrawGizmosSelected()
    {
        Vector3 p = transform.position;
        Gizmos.color = mycolor;
        Gizmos.DrawSphere(p, explosionRaidus);
    }
}
