using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bilboard : MonoBehaviour
{
    Transform myTr;

    Transform mainCameraTr;
    // Start is called before the first frame update
    void Start()
    {
        myTr = GetComponent<Transform>();

        mainCameraTr = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //항상 카메라를 90도로 바라봄
        myTr.LookAt(mainCameraTr);
    }
}
