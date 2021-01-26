using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCtrl : MonoBehaviour
{

    Animator anim;

    [HideInInspector]
    public bool isDie;


    private NavMeshAgent myTraceAgent;

    Vector3 movePoint = Vector3.zero;

    Ray ray;

    RaycastHit hitInfo1;


    //자신의 Transform 참조 변수
    private Transform myTr;

    // 회전의 중심축
    public Transform targetTr;


    public float dist2;

    private GameObject[] Enemys;
    private Transform EnemyTarget;


    //발사 변수
    private bool shot;
    // 적을 봐라보는 회전 속도
    private float enemyLookTime;
    //적을 봐라보는 회전각
    private Quaternion enemyLookRotation;

    public Texture test;//머즐플래시텍스쳐

    //총탄의 발사 시작 좌표 연결 변수 
    public Transform firePos;
    //총알 발사 주기
    private float bulletSpeed;
    //AudioSource 컴포넌트 저장할 레퍼런스 
    private AudioSource source = null;

    //MuzzleFlash GameObject를 연결 할 레퍼런스 
    public GameObject muzzleFlash;
    //총탄의 발사 사운드 
    public AudioClip fireSfx;


    //레이저발사용
    public LineRenderer rayLine;

    //레이저 도트용
    public Transform rayDot;

    bool check;

    //Ray 구조체
    Ray ray2;
    //Ray에 충돌된 대상 정보 저장
    RaycastHit hitInfo2;



    void Awake()
    {
        myTraceAgent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        test = Resources.Load<Texture>("Base/bullet");
        fireSfx = Resources.Load<AudioClip>("Base/bazooka");


        myTr = GetComponent<Transform>();
        source = GetComponent<AudioSource>();
        muzzleFlash.SetActive(false);
    }



    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("velocity", Mathf.Abs(myTraceAgent.velocity.x + myTraceAgent.velocity.z));
        Debug.Log(myTraceAgent.velocity);
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.blue);
        ray2.origin = firePos.position;

        ray2.direction = firePos.TransformDirection(Vector3.forward);
        Debug.DrawRay(ray2.origin, ray2.direction * 30.0f, Color.green);

        if (Physics.Raycast(ray2, out hitInfo2, 30.0f))
        {
            Vector3 posValue = firePos.InverseTransformPoint(hitInfo2.point);

            rayLine.SetPosition(0, posValue);
            rayDot.localPosition = posValue;

            if (shot && hitInfo2.collider.tag == "Enemy")
            {
                check = true;
            }
        }
        else
        {
            rayLine.SetPosition(0, new Vector3(0, 0, 30.0f));
            rayDot.localPosition = Vector3.zero;
        }

        if (!shot)
        {
            myTr.RotateAround(targetTr.position, Vector3.up, Time.deltaTime * 55.0f);
            check = false;
        }
        else
        {
            if (shot)
            {
                if (Time.time > enemyLookTime)
                {
                    enemyLookRotation = Quaternion.LookRotation(EnemyTarget.position - myTr.position);
                    myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation, Time.deltaTime * 2.0f);
                    enemyLookTime = Time.time + 0.01f;
                }
            }
        }

        if (shot && check)
        {
            if (Time.time > bulletSpeed)
            {
                ShotStart();
                bulletSpeed = Time.time + 0.3f;
            }
        }


#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0)&&!isDie)
        {
            if(Physics.Raycast(ray,out hitInfo1, Mathf.Infinity,1 <<LayerMask.NameToLayer("Barrel")))
            {
                movePoint = hitInfo1.point;
                myTraceAgent.destination = movePoint;
                myTraceAgent.stoppingDistance = 2.0f;//대상과 거리가 2미터쯤이면 정지
            }
            else if (Physics.Raycast(ray, out hitInfo1, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
            {
                movePoint = hitInfo1.point;

                myTraceAgent.destination = movePoint;
                myTraceAgent.stoppingDistance = 0.0f;

            }
        }
#endif
    }

    IEnumerator TargetSettting()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            Enemys = GameObject.FindGameObjectsWithTag("EnemyBody");
            Transform EnemyTargets = Enemys[0].transform;
            float dist = (EnemyTargets.position - myTr.position).sqrMagnitude;
            foreach (GameObject _Enemy in Enemys)
            {
                if ((_Enemy.transform.position - myTr.position).sqrMagnitude < dist)
                {
                    EnemyTargets = _Enemy.transform;
                    dist = (EnemyTargets.position - myTr.position).sqrMagnitude;
                }
            }
            EnemyTarget = EnemyTargets;
        }

    }
    IEnumerator ShotSettting()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);
            //dist1 = (EnemyTarget.position - myTr.position).sqrMagnitude;
            dist2 = Vector3.Distance(myTr.position, EnemyTarget.position);

            if (dist2 < 5.0f)
            {
                shot = true;
            }
            else
            {
                shot = false;
            }
        }

    }

    private void ShotStart()
    {
        StartCoroutine(this.FireStart());
    }

    IEnumerator FireStart()
    {

        source.PlayOneShot(fireSfx, fireSfx.length + 0.2f);
        float scale = Random.Range(1.0f, 2.5f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));

        muzzleFlash.transform.localRotation = rot;

        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

        muzzleFlash.SetActive(false);

        void Fire()
        {
            shot = true;
        }
    }
}
