using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionObject : MonoBehaviour
{
    Transform myTr;
    public GameObject exploEffect;

    //히트되는 누적 카운트 변수
    private int hitCount = 0;


    void Awake()
    {
        myTr = GetComponent<Transform>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Bullet")
        {
            if(++hitCount>=5)
            {
                FireObject();
            }
        }
    }

    void OnCollision(object[] _params)
    {
        Vector3 hitPos = (Vector3)_params[0];

        Vector3 firePos = (Vector3)_params[1];

        //정규화된 입사벡터 만들기 => 정규화된 Ray 맞은 각도는 = (Hit좌표 - 발사 원점).normalized
        Vector3 collVector = (hitPos - firePos).normalized;

        GetComponent<Rigidbody>().AddForceAtPosition(collVector * 50f, hitPos);

        if (++hitCount >= 5)
        {
            FireObject();
        }
    }

    void FireObject()
    {
        StartCoroutine(this.ExpObject());
    }

    IEnumerator ExpObject()
    {

        //GameObject.Find("Player").GetComponent<PlayerCtrl>().barrelFire = false;

        Instantiate(exploEffect, myTr.position, Quaternion.identity);

        Collider[] colls = Physics.OverlapSphere(myTr.position, 15.0f);

        foreach(Collider coll in colls)
        {
            Rigidbody rigid = coll.GetComponent<Rigidbody>();

            if(rigid !=null)
            {
                if(rigid.gameObject.tag =="Barrel")
                {
                    rigid.mass = 1.0f;
                    //폭발력, 위로솟구치는힘
                    rigid.AddExplosionForce(1000.0f, myTr.position, 15.0f, 300.0f);
                }
                else if(rigid.gameObject.tag == "Enemy")
                {
                    rigid.gameObject.SendMessage("OnCollisionBarrel", myTr.position, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        Destroy(gameObject, 5.5f);
        yield return null;
    }
}
