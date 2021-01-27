using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleClass1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SendMessage("ApplyDamage", 5.0f);
    }

    void ApplyDamage(float damage)
    {
        Debug.Log("EX1 Damage :" + damage);
    }
}
