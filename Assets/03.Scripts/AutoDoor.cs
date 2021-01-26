using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class AutoDoor : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();   
    }
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            anim.SetTrigger("Open");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        anim.SetTrigger("Close");
    }
}
