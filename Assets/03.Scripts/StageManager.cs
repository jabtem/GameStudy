using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    Transform[] EnemySpawnPoints;
    public BaseCtrl baseStart;

    public GameObject Enemy;

    bool gameEnd;
    GameObject[] Enemys;

    void Awake()
    {
        EnemySpawnPoints = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        StartCoroutine(CreateEnemy());
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(5.0f);

        baseStart.StartBase();
    }

    IEnumerator CreateEnemy()
    {
        while(!gameEnd)
        {
            yield return new WaitForSeconds(5.0f);

            Enemys = GameObject.FindGameObjectsWithTag("Enemy");

            if(Enemys.Length < 20)
            {
                for(int i = 1; i<EnemySpawnPoints.Length; i++)
                {
                    Instantiate(Enemy, EnemySpawnPoints[i].localPosition, EnemySpawnPoints[i].localRotation);
                }
            }
        }
    }

}
