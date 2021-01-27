using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RyanChild : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.SendMessageUpwards("ApplyDamage", 5.0f);
    }
    void ApplyDamage(float damage)
    {
        Debug.Log("RyanChild");
    }
}
