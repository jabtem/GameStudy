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

    int initLife = 100;

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
    Transform barrelPos;

    //포톤 추가///////////////////////////////////////////////////////
    /*
     * PhotonView : PhotonView 컴포넌트는 네트워크상에 접속한 
     * 유저 간의 데이타를 송/수신하는 통신 모듈
     * 
     * 역할 : 동일 룸에 입장한 다른 유저에게 게임오브젝트 또는 프리팹을 거의 동시에
     * 생성하거나 서로 데이터를 송수신하려면 ( (Ex) 로컬 오브젝트와 아바타 오브젝트) 반드시 필요한 컴포넌트이다.
     * 
     * 참고 : PhotonView 컴포넌트는 유니티 빌트인 네트워크 API의
     * NetworkView 컴포넌트와 동일한 기능(역할)을 하며 속성 도 유사함.
     * 
     * PhotonView 컴포넌트의 속성 View ID는 PhotonView 컴포넌트별 고유ID를 의미한다.
     * 네트워크 유저가 접속한 순서대로 1001, 2001, 3001, ....순으로 1000번 간격으로 자동부여 된다.
     * 그리고 물리적으로 하나의 네트워크 플레이어에게 추가할 수 있는 PhotonView 컴포넌트는 1000개로
     * 제한 되어있다. 그러므로 첫 번째 접속한 플레이어의 PhotonView 컴포넌트가 여러개 추가돼있다면
     * View ID는 1001, 1002, 1003, ...과 같은 규칙으로 부여됨 즉 프로젝트에서 만들어진 순서로...
     * 
     * PhotonView 컴포넌트의 속성 Controlled locally 는 bool형 타입으로
     * 이 속성의 체크 여부로 어느 플레이어 객체가 자신의 것인지 판단할 수 있음
     * 
     * 
     */

    // 포톤 추가///////////////////////////////////////////////////////////////////////

    //참조할 컴포넌트를 할당할 레퍼런스 (미리 할당하는게 좋음)
    Rigidbody myRbody;

    //PhotonView 컴포넌트를 할당할 레퍼런스 
    PhotonView pv = null;

    //메인 카메라가 추적할 CamPivot(플레이어) 게임오브젝트 
    public Transform camPivot;

    //위치 정보를 송수신할 때 사용할 변수 선언 및 초기값 설정 
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;

    //플레이어 하위의 Canvas 객체를 연결할 레퍼런스->Canvas 컴포넌트를 연결 
    public Canvas hudCanvas;
    //Filled 타입의 Image UI 항목을 연결할 레퍼런스->Image 컴포넌트 연결 
    public Image hpBar;
    //플레이어의 HUD에 표현할 스코어 Text UI 항목 연결 레퍼런스
    public Text txtKillCount;

    void Awake()
    {
        myTraceAgent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        myTr = GetComponent<Transform>();

        source = GetComponent<AudioSource>();
        muzzleFlash.SetActive(false);

        //포톤 추가////////////////////////////////////////////////////////////

        // 네트워크 버전으로 변경하면서 링크가 깨졌으니 스크립트로 다시 연결~
        lifeBar = GameObject.Find("HpBar").GetComponent<Image>();

        //컴포넌트를 할당 
        myRbody = GetComponent<Rigidbody>();

        //PhotonView 컴포넌트 할당 
        pv = GetComponent<PhotonView>();

        /*
         * PhotonView 컴포넌트의 Observe 속성에 특정 스크립트를 연결하면
         * PhotonView 컴포넌트는 해당 스크립트에 있는 OnPhotonSerializeView 콜백 함수를 통해
         * 데이터를 전송주기에 맞춰 송/수신하는 역할을 함.
         * 
         * 다음의 두 가지 방법으로 PhotonView 컴포넌트의 Observe 속성에
         * 특정 스크립트를 연결할 수 있다.
         * 
         * 1)Resources 폴더 안에 있는 MainPlayer 프리팹을 선택하고 Inspector 뷰의 
         * PlayerCtrl 컴포넌트(스크립트) 헤더를 드래그드롭해서 PhotonView 컴포넌트의 Observe옵션에 연결
         * 
         * 2)다음과 같이 스크립트에서 직접 연결
         */

        //PhotonView Observed Components 속성에 PlayerCtrl(현재) 스크립트 Component를 연결
        pv.ObservedComponents[0] = this;

        /*
         * PhotonView 컴포넌트의 Observe option 속성
         * 옵션                           설명
         * off                            실시간 데이터 송수신을 하지 않음.
         * ReliableDeltaCompressed        데이터를 정확히 송수신한다(TCP/IP 프로토콜)
         * Unreliable                     데이터의 정합성을 보장할 수 없지만 속도가 빠르다(UDP 프로토콜)
         * UnreliableOnChange             Unreliable 옵션과 같지만 변경사항이 발생했을 경우에만 전송한다
         */

        //데이타 전송 타입을 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;

        //PhotonView.isMine 속성은 bool형 타입으로 자신이 만든 네트워크 게임오브젝트 여부를 판단할 때 사용
        //PhotonView가 자신의 플레이어일 경우 즉, 자신의 PhotonView 여부 판단
        if (pv.isMine)  // PhotonNetwork.isMasterClient 마스터 클라이언트는 이런식 체크
        {
            //메인 카메라에 추가된 SmoothFollowCam 스크립트(컴포넌트)에 추적 대상을 연결 
            Camera.main.GetComponent<smoothFollowCam>().target = camPivot;

        }
        //자신의 네트워크 객체가 아닐때...
        else
        {
            //원격 네트워크 유저의 아바타는 물리력을 안받게 처리하고
            //또한, 물리엔진으로 이동 처리하지 않고(Rigidbody로 이동 처리시...)
            //실시간 위치값을 전송받아 처리 한다 그러므로 Rigidbody 컴포넌트의
            //isKinematic 옵션을 체크해주자. 한마디로 물리엔진의 영향에서 벗어나게 하여
            //불필요한 물리연산을 하지 않게 해주자...(만약 수십명의 플레이어가 접속 한다면???)

            //원격 네트워크 플레이어의 아바타는 물리력을 이용하지 않음 
            //(원래 게임이 이렇다는거다...우리건 안해도 체크 돼있음...)
            myRbody.isKinematic = true;
        }

        // 원격 플래이어의 위치 및 회전 값을 처리할 변수의 초기값 설정 
        // 잘 생각해보자 이런처리 안하면 순간이동 현상을 목격
        currPos = myTr.position;
        currRot = myTr.rotation;

        // (네트워크 UI 버전에서 사용)////////////////////////////////////////
        //현재의 생명력을 초기 생명값으로 초기값 설정 
        life = initLife;

        //Filled 이미지 색상을 파랑으로 셋팅 
        hpBar.color = Color.blue;

        /////////////////////////////////////////////////////////////////////////////////
    }

    IEnumerator Start()
    {

        yield return new WaitForSeconds(5.0f);

        // 일정 간격으로 주변의 가장 가까운 Enemy를 찾는 코루틴 
        if(pv.isMine)
        {
            StartCoroutine(this.TargetSetting());

            // 가장 가까운 적을 찾아 발사...
            StartCoroutine(this.ShotSetting());
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(pv.isMine)
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

                //(포톤 추가)원격 네트워크 플레이어의 자신의 아바타 플레이어에는 RPC로 원격으로 FireStart 함수를 호출 
                pv.RPC("ShotStart", PhotonTargets.Others, null);

                //(포톤 추가)모든 네트웍 유저에게 RPC 데이타를 전송하여 RPC 함수를 호출, 로컬 플레이어는 로컬 Fire 함수를 바로 호출 
                //pv.RPC("ShotStart", PhotonTargets.All, null);

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

                barrelFire = false;

            }
        }

        }
        //원격플레이어일때 수행
        else
        {
            myTr.position = Vector3.Lerp(myTr.position, currPos, Time.deltaTime * 3.0f);
            myTr.rotation = Quaternion.Slerp(myTr.rotation, currRot, Time.deltaTime * 3.0f);
        }
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
    [PunRPC]
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
            // (네트워크 UI 버전에서 사용) //////////////////////////////////////////
            //현재 생명력 백분율 = (현재 생명력) / (초기 생명력)
            hpBar.fillAmount = (float)life / (float)initLife;

            //생명력 수치에 따라 Filled 이미지의 색상을 변경 
            if (hpBar.fillAmount <= 0.4f)
            {
                hpBar.color = Color.red;
            }
            else if (hpBar.fillAmount <= 0.6f)
            {
                hpBar.color = Color.yellow;
            }

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

        hudCanvas.enabled = false;
        yield return null;
    }
    public void BarrelFire(Transform barrelTr)
    {
        barrelPos = barrelTr;
        barrelFire = true;
    }

    //네트워크 객체 생성완료시 자동 호출되는 함수
    void onPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = pv.instantiationData;
        Debug.Log((int)data[0]);
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 플레이어의 위치 정보를 송신
        if (stream.isWriting)
        {
            //박싱
            stream.SendNext(myTr.position);
            stream.SendNext(myTr.rotation);
        }
        //원격 플레이어의 위치 정보를 수신
        else
        {
            //언박싱
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }

    }


}
