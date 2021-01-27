using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleClass2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.SendMessage("ApplyDamage", 5.0f);
    }

    void ApplyDamage()
    {
        Debug.Log("Damage : ignore");
    }
    void ApplyDamage(float damage)
    {
        Debug.Log("EX2 Damage :" + damage);
    }
}
