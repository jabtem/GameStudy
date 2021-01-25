using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public BaseCtrl baseStart;


    IEnumerator Start()
    {
        yield return new WaitForSeconds(5.0f);

        baseStart.StartBase();
    }

}
