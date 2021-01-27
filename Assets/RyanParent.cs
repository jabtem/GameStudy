using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RyanParent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.BroadcastMessage("ApplyDamage", 5.0f);
    }

    // Update is called once per frame
    void ApplyDamage(float damage)
    {
        Debug.Log("RyanParent :" + damage);
    }
}
