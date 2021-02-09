using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerCtrl : MonoBehaviour
{

    Animator anim;

    [HideInInspector]
    public bool isDie;

    public Text txtKillCount;

    private NavMeshAgent myTraceAgent;

    Vector3 movePoint = Vector3.zero;

    Ray ray;

    RaycastHit hitInfo1;
    RaycastHit hitInfo2;


    // (추가)

    //적과의 거리를 위한 변수
    public float dist1;
    public float dist2;

    //Enemy를 찾기 위한 배열 
    private GameObject[] Enemys;
    public Transform EnemyTarget;

    //자신의 Transform 참조 변수
    private Transform myTr;

    // 회전의 중심축
    public Transform targetTr;

    // 플레이어 발사 변수
    private bool shot;
    // 적을 봐라보는 회전 속도
    private float enemyLookTime;
    //적을 봐라보는 회전각
    private Quaternion enemyLookRotation;

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

    //Ray 센서를 위한 변수
    bool check;

    //레이저 발사를 위한 컴포넌트
    public LineRenderer rayLine;

    //레이저 도트 타겟을 위한 변수
    public Transform rayDot;

    //케릭터 센서 Idle 방향
    public bool turnRight;
    //케릭터 센서 각도
    private float turnValue;

    //플레이어 데미지
    public int power;
    // 이동중에 총 안쏜다(난이도)
    private bool FireAction;

    //애니메이터 연결
    Animator myAnim;

    //플레이어 라이프
    public int life;
    //플레이어 라이프 바
    public Image lifeBar;

    //데미지 효과 프리팹
    public GameObject damageEffect;
    //데미지 프로젝터 연결
    public Projector damageProjector;

    //드럼통 파괴
    [HideInInspector]
    public bool barrelFire;
    public Transform barrelPos;
    public bool isMine;

    void Awake()
    {
        myTraceAgent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        myTr = GetComponent<Transform>();

        source = GetComponent<AudioSource>();
        muzzleFlash.SetActive(false);

        isMine = GetComponent<PhotonView>().isMine;

    }

IEnumerator Start()
    {

        yield return new WaitForSeconds(5.0f);

        // 일정 간격으로 주변의 가장 가까운 Enemy를 찾는 코루틴 
        StartCoroutine(this.TargetSetting());

        // 가장 가까운 적을 찾아 발사...
        StartCoroutine(this.ShotSetting());
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(myTr.localRotation.y);//raidan
        //Debug.Log(myTr.localRotation.y * Mathf.Rad2Deg);//defree 수학적으로 이용할 degree
        //Debug.Log(myTr.rotation.eulerAngles.y);//degree
        //Debug.Log(myTr.localEulerAngles.y);//degree

        anim.SetFloat("velocity", Mathf.Abs(myTraceAgent.velocity.x + myTraceAgent.velocity.z));
        //Debug.Log(myTraceAgent.velocity);
        ray.origin = firePos.position;
        ray.direction = firePos.TransformDirection(Vector3.forward);

        if(Physics.Raycast(ray,out hitInfo1, 30.0f))
        {
            Vector3 posValue = firePos.InverseTransformPoint(hitInfo1.point);
            rayLine.SetPosition(0, posValue);
            rayDot.localPosition = posValue;

            if (shot && (hitInfo1.collider.tag == "Enemy" || hitInfo1.collider.tag == "Barrel"))
            {
                //발사를 위한 변수 true
                check = true;
            }
        }
        else
        {
            //난 달릴땐 레이저 빔 안보이게... 개인의 취향
            if (Mathf.Abs(myTraceAgent.velocity.z) > 0)
            {
                //Debug.Log("2coll");
                //기본 거리체크 레이저 생성
                rayLine.SetPosition(0, new Vector3(0, 0, 0));

                //타겟에 레이저 Dot 초기화 
                rayDot.localPosition = Vector3.zero;
            }
            else
            {
                //Debug.Log("2coll");
                //기본 거리체크 레이저 생성
                rayLine.SetPosition(0, new Vector3(0, 0, 30.0f));

                //타겟에 레이저 Dot 초기화 
                rayDot.localPosition = Vector3.zero;

            }
        }

        if(!shot)
        {
            if(FireAction)
            {
                if(Time.time > turnValue)
                {
                    turnRight = !turnRight;
                    turnValue = Time.time + 2.5f;
                }
                if(turnRight)
                {
                    myTr.Rotate(Vector3.up * Time.deltaTime * 55.0f);
                }
                if(!turnRight)
                {
                    myTr.Rotate(Vector3.up * -Time.deltaTime * 55.0f);
                }
            }

            check = false;
        }
        else
        {
            if(shot)
            {
                if(Time.time > enemyLookTime)
                {
                    Vector3 targetDir = EnemyTarget.position - myTr.position;
                    float dotValue = Vector3.Dot(myTr.forward, targetDir.normalized);

                    //범위 지정안해주면 에러남 실수연산은 오차를동반하기때문에 에러가난다
                    if(dotValue > 1.0f)
                    {
                        dotValue = 1.0f;
                    }
                    else if(dotValue < -1.0f)
                    {
                        dotValue = -1.0f;
                    }
                    float value = Mathf.Acos(dotValue);
                    

                    if(value * Mathf.Rad2Deg>35.0f)
                    {
                        enemyLookRotation = Quaternion.LookRotation(EnemyTarget.position - myTr.position);
                        myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation, Time.deltaTime * 7.0f);
                        enemyLookTime = Time.time + 0.01f;
                    }

                    else
                    {
                        myTr.LookAt(EnemyTarget);
                    }
                }
            }
        }

        if (shot && check)
        {
            if (Time.time > bulletSpeed)
            {
                //일정 주기로 발사
                ShotStart();

                bulletSpeed = Time.time + 0.3f;
            }
        }

        if(shot)
        {
            anim.SetBool("Shot", true);
        }
        else
        {
            anim.SetBool("Shot", false);
        }

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.blue);



#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0)&&!isDie&&isMine)
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

                barrelFire = false;

            }
        }
#endif
    }

    IEnumerator TargetSetting()
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
            //우선순위지정
            if(barrelFire)
            {
                EnemyTarget = barrelPos;
            }
            else
            {
                EnemyTarget = EnemyTargets;
            }

        }

    }
    IEnumerator ShotSetting()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);
            //dist1 = (EnemyTarget.position - myTr.position).sqrMagnitude;
            dist2 = Vector3.Distance(myTr.position, EnemyTarget.position);

            if (myTraceAgent.velocity.z != 0.0f)
            {
                FireAction = false;
            }
            else
            {
                FireAction = true;
            }

            if(FireAction)
            {
                if (dist2 < 37.0f)
                {
                    shot = true;
                }
                else
                {
                    shot = false;
                }
            }
            else
            {
                shot = false;
            }
        }
    }

    private void ShotStart()
    {
        //Debug.Log(true);
        if(!isDie)
            StartCoroutine(this.FireStart());
    }

    IEnumerator FireStart()
    {

        source.PlayOneShot(fireSfx, fireSfx.length + 0.2f);
        float scale = Random.Range(1.0f, 1.3f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));

        muzzleFlash.transform.localRotation = rot;

        muzzleFlash.SetActive(true);

        if(Physics.Raycast(firePos.position,firePos.forward,out hitInfo2,37.0f))
        {
            if(hitInfo2.collider.tag=="Enemy")
            {
                object[] _params = new object[2];
                //Enemy가 Ray에 닿은 정확한 월드값
                _params[0] = hitInfo2.point;

                _params[1] = power;

                hitInfo2.collider.gameObject.SendMessage("OnCollision", _params, SendMessageOptions.DontRequireReceiver);
            }
            //Ray에 충돌된 게임오브젝트의 Tag 값으로 Barrel 인지 아닌지 체킹 
            else if (hitInfo2.collider.tag == "Barrel")
            {
                //Debug.Log(123);
                //Barrel이 Ray에 충돌했을때의 입사각을 알기위하여 맞은 지점과 발사원점을 전달하여
                //정확한 위치에 타격을 줘서 확실한 물리효과 연출 
                object[] _params = new object[2];
                _params[0] = hitInfo2.point;
                _params[1] = firePos.position;
                //BarrelCtrl의 OnCollision 함수 호출 (_params 배열이름 전달)
                hitInfo2.collider.gameObject.SendMessage("OnCollision", _params
                                                    , SendMessageOptions.DontRequireReceiver);
            }
        }

        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

        muzzleFlash.SetActive(false);
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "EnemyWeapon")
        {

            //총알이 맞은 위치를 위한 ContactPoint 구조체 선언 및 할당
            ContactPoint contact = coll.contacts[0];

            //데미지 효과 함수를 호출
            CreateDamage(contact.point);

            int damage = coll.gameObject.GetComponent<EnemyWeapon>().power;
            //적 무기에 당했을때 데미지를 받는데
            life -= damage;
            //라이프바를 증가 시켜서 위험 상태 표시
            lifeBar.fillAmount += damage / 100.0f;
            //프로젝터로 피 효과
            damageProjector.farClipPlane -= damage / 20.0f;

            // 생명력이 바닥이면 죽이자
            if (life <= 0)
            {
                StartCoroutine(PlayerDie());
            }
        }
    }
    void CreateDamage(Vector3 pos)
    {
        //데미지 효과를 위한 코루틴 함수 호출 (생성과 소멸은 항상 코루틴으로...)
        StartCoroutine(this.CreateDamageEffect(pos));
    }
    IEnumerator CreateDamageEffect(Vector3 pos)
    {
        Instantiate(damageEffect, pos, Quaternion.identity);
        yield return null;
    }
    IEnumerator PlayerDie()
    {
        //죽이고
        isDie = true;
        //총구 비활성
        firePos.gameObject.SetActive(false);
        //적이 추적 못하게 테그 바꾸고
        gameObject.tag = "Untagged";
        //네비게이션 비 활성화
        myTraceAgent.enabled = false;

        //모든 리지드 바디를 얼리고
        Rigidbody[] rbody = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody _rbody in rbody)
        {
            //_rbody.constraints = RigidbodyConstraints.FreezeAll;
            //_rbody.constraints = RigidbodyConstraints.FreezePositionX;
            //_rbody.constraints = RigidbodyConstraints.FreezePositionY;
            _rbody.constraints = RigidbodyConstraints.FreezePositionZ;
        }

        //애니메이션 스톱으로 Ragdoll 효과 
        anim.enabled = false;

        yield return null;
    }
    public void BarrelFire(Transform barrelTr)
    {
        barrelPos = barrelTr;
        barrelFire = true;
    }
}
