using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{

    public float speed = 200f;
    public float range = 400;
    public int power = 10;

    public GameObject ExploPtcl;

    float dist;


    // Start is called before the first frame update
    void Start()
    {
        //Destroy(gameObject, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //GetComponent<Rigidbody>().AddForce(Vector3.forward * speed);
        //GetComponent<Rigidbody>().AddForce(transform.forward*speed);
        //GetComponent<Rigidbody>().AddRelativeForce(transform.forward * speed);//월드좌표 입력시 로컬좌표로 계산해주는함수
        //GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * speed);

       transform.Translate(Vector3.forward * Time.deltaTime * speed);

        dist += Time.deltaTime * speed;

        if(dist>=range)
        {
            Debug.Log(dist);

            Instantiate(ExploPtcl, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter(Collision coll)
    {
        Instantiate(ExploPtcl, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
