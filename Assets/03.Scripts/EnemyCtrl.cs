#define CBT_MODE
#define RELEASE_MODE


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Rand = UnityEngine.Random;
//using UnityEngine.iOS;

[System.Serializable]
public class Anim
{
    public AnimationClip idle1;
    public AnimationClip idle2;
    public AnimationClip idle3;
    public AnimationClip idle4;

    public AnimationClip move;
    public AnimationClip surprise;

    public AnimationClip attack1;
    public AnimationClip attack2;
    public AnimationClip attack3;
    public AnimationClip attack4;

    public AnimationClip hit1;
    public AnimationClip hit2;

    public AnimationClip eat;

    public AnimationClip sleep;
    public AnimationClip die;
}

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("EnemyCtrl/Follow EnemyCtrl")]

public class EnemyCtrl : MonoBehaviour
{
    [Multiline(7)]
    public string Ex = "안녕하세요";

    //사용자의 메모제공
    [TextArea(7, 11)]
    public string Memo = "";


    [Space(30)]
    [Header("Animatation")]
    public Anim anims;


    Animation _anim;
    AnimationState animState;

    float randAnimTime;
    int randAnim;

    NavMeshAgent myTraceAgent;

    public Transform myTr;
    public Transform traceTarget;

    bool traceObject;
    bool traceAttack;

    public float hungryTime;
    public float nonHungryTime;

    float dist1;
    float dist2;

    GameObject[] players;
    Transform playerTarget;

    public GameObject[] baseAll;
    Transform baseTarget;

    Transform[] roamingCheck;
    int roamingRandCheck;
    public Transform roamingTarget;


    //[System.NonSerialized]
    [HideInInspector]
    public bool isDie;

    //원래 열거형은 대문자여야됨
    public enum MODE_STATE {idle =1,move,surprise,trace,attack,hit,eat,sleep,die };

    public enum MODE_KIND { enemy1 =1,enemy2, enemyboss};

    [Space(20)]
    [Header("STATE")]
    public MODE_STATE enemyMode = MODE_STATE.idle;

    [Header("SETTING")]
    public MODE_KIND enemyKind = MODE_KIND.enemy1;

    [Space(10)]
    [Header("몬스터의 인공지능")]

    [Space(5)]

    [Tooltip("몬스터의 HP")]
    [Range(0, 1000)] public int hp = 100;


    [Tooltip("몬스터의 속도")]
    [Range(0.5f, 4f)] public float speed = 2f;


    //거리에따른체크
    [Tooltip("몬스터 발견거리")]
    [Range(0f, 8f)] [SerializeField]float findDist = 8f;
    [Tooltip("몬스터 추적거리")]
    [Range(0f, 7f)] [SerializeField]float traceDist = 7f;
    [Tooltip("몬스터 공격거리")]
    [Range(0f, 5f)] [SerializeField]float attackDist = 2f;
    [Tooltip("몬스터 로밍 시간")]
    [Range(0f, 50f)] [SerializeField]float hungryTimeSet = 40f;
    [Tooltip("몬스터 로밍대기시간")]
    [Range(0f, 50f)] [SerializeField]float nonHungryTimeSet = 40f;


    [Header("Test")]
    [SerializeField] private bool isHit;
    [SerializeField] private bool hungry;
    [SerializeField] private bool sleep;
    private bool nonHungry;
    private float isHitTime;

    PhotonView pv = null;

    Rigidbody myRbody;
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;

    //애니메이션동기화
    int net_Anim = 0;
    void Awake()
    {
        //이 부분은 단순 공부용/////////////////////////////////////////////////////////////////

        /*
            -전 처리기

        유니티에선 #define 지시자가 없다 따라서 두 가지 방법으로 추가해야한다.
        1) PlayerSettings => OtherSettings => Scripting Define Symbols
        2) 매번 생성을 반복하면 힘드니깐 파일로 보관하는 경우 및에 참조...
        3) 각 스크립트 최 상위에 #define 지시어로 선언(해당 컴포넌트만 참조)


        #define AAA 를 전역으로 사용하고 싶다면 메모장에 -define:AAA 를 적고
        #define AAA 와 #define BBB 를 모두 사용하고 싶다면 -define:AAA;BBB 와 같은 형식으로 
        사용.(유니티 에디터에서도 사용법은 동일) 
        모두 작성했으면 유니티 프로젝트에 Assets 폴더에 

        (과거 버전)
        Api Compatibility Level이  .net 2.0 일 경우엔 gmcs.rps 
        .net 2.0 subset 일 경우엔 smcs.rps 로 파일저장
        (현재 버전)
        mcs.rps로 통일

        주의) mcs.rps 파일을 수정시엔 해당 define 을 사용하는 스크립트를 재 컴파일 해줘야 됨..


          
        
        
        // 사용 방법

        #if UNITY_EDITOR

               유니티 에디터 상태에서만 동작하는 스크립트 로직

        #endif



        #if UNITY_IOS

             빌드 타켓이 아이폰일 때 동작하는 스크립트 로직 

        #endif



        #if UNITY_ANDROID

             빌드 타켓이 안드로이드일 때 동작하는 스크립트 로직 

        #endif

        #지시어로 다음과 같이 선택적 스크립트 실행
        #if CBT_MODE
        int HP = 10;

        #elif RELEASE_MODE
        int HP = 100;
        #endif
        */

#if AAA
        Debug.Log("AAA");
#endif

#if BBB
        Debug.Log("BBB");  
#endif

#if UNITY_5_3_2
        Debug.Log("CCC");
#endif

#if UNITY_2017_3_1
        Debug.Log("DDD");
#endif


#if UNITY_EDITOR
        //Debug.Log("EDITOR");
#endif

#if UNITY_IPHONE
        Debug.Log("UNITY_IPHONE");
#endif

#if UNITY_ANDROID
        Debug.Log("UNITY_ANDROID");
#endif

#if UNITY_IPHONE
        // DeviceGeneration는 안드로이드는 지원안하고 안드로이드는 필요도 없다.
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {

            switch (Device.generation)
            {
                case DeviceGeneration.iPodTouch5Gen:
                    //처리
                    break;
                case DeviceGeneration.iPadMini1Gen:
                    //처리
                    break;
                case DeviceGeneration.iPodTouch3Gen:
                    //처리
                    break;
            }
        }
        //여기까지 공부용이니 차후 삭제////////////////////////////////////////////////////////////////////
#endif 

        myTraceAgent = GetComponent<NavMeshAgent>();

        _anim = GetComponentInChildren<Animation>();

        net_Anim = 0;

        myTr = GetComponent<Transform>();

        baseAll = GameObject.FindGameObjectsWithTag("Base");

        roamingCheck = GameObject.Find("RoamingPoint").GetComponentsInChildren<Transform>();

        //포톤 추가////////////////////////////////////////////////////////////

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

        //자신의 네트워크 객체가 아닐때...(마스터 클라이언트가 아닐때)
        if (!PhotonNetwork.isMasterClient)
        {
            //원격 네트워크 유저의 아바타는 물리력을 안받게 처리하고
            //또한, 물리엔진으로 이동 처리하지 않고(Rigidbody로 이동 처리시...)
            //실시간 위치값을 전송받아 처리 한다 그러므로 Rigidbody 컴포넌트의
            //isKinematic 옵션을 체크해주자. 한마디로 물리엔진의 영향에서 벗어나게 하여
            //불필요한 물리연산을 하지 않게 해주자...

            //원격 네트워크 플레이어의 아바타는 물리력을 이용하지 않음 (마스터 클라이언트가 아닐때)
            //(원래 게임이 이렇다는거다...우리건 안해도 체크 돼있음...)
            myRbody.isKinematic = true;
            //네비게이션도 중지
            //myTraceAgent.isStopped = true; 이걸로 하면 off Mesh Link 에서 에러 발생 그냥 비활성 하자
            myTraceAgent.enabled = false;
        }

        // 원격 플래이어의 위치 및 회전 값을 처리할 변수의 초기값 설정 
        // 잘 생각해보자 이런처리 안하면 순간이동 현상을 목격
        currPos = myTr.position;
        currRot = myTr.rotation;
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
#if CBT_MODE
        hp = 1000;
#elif RELEASE_MODE
        hp = 100;
#endif
        _anim.clip = anims.idle1;
        _anim.Play();

        if(PhotonNetwork.isMasterClient)
        {


        traceTarget = baseAll[0].transform;
        myTraceAgent.SetDestination(traceTarget.position);

        StartCoroutine(ModeSet());
        StartCoroutine(ModeAction());
        StartCoroutine(this.TargetSetting());
        RoamingCheckStart();

        }
        else
        {
            StartCoroutine(this.NetAnim());
        }
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > randAnimTime)
        {
            randAnim = Rand.Range(0, 4);
            randAnimTime = Time.time + 5.0f;
        }

        if(!hungry)
        {
            if(Time.time>hungryTime)
            {
                //RoamingCheckStart();
                hungry = true;
                nonHungryTime = Time.time + nonHungryTimeSet + Rand.Range(10f, 15f);
                nonHungry = true;
            }
        }

        if(nonHungry)
        {
            if (Time.time > nonHungryTime)
            {
                
                nonHungry = false;
                hungryTime = Time.time + hungryTimeSet + Rand.Range(10f, 15f);
                hungry = false;
            }
        }

        if(isHit)
        {
            if(Time.time > isHitTime)
            {
                isHit = false;
            }
        }

        //적을 봐라봄
        /*  if (enemyLook)
            {
                if (Time.time > enemyLookTime)
                {
                    //    enemyLookRotation = Quaternion.LookRotation(-(enemyTr.forward)); // - 해줘야 바라봄
                    enemyLookRotation = Quaternion.LookRotation(enemyTr.position - myTr.position); // - 해줘야 바라봄
                    myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation, Time.deltaTime * 10.0f);
                    enemyLookTime = Time.time + 0.01f;
                }
            }*/
        //포톤 추가
        // 마스터 클라이언트가 직접 Ai 및 애니메이션 로직 수행
        // pv.isMine 해도 됨
        if (PhotonNetwork.isMasterClient)
        {
            // 처리
        }
        //포톤 추가
        //원격 플레이어일 때 수행
        else
        {
            //원격 플레이어의 아바타를 수신받은 위치까지 부드럽게 이동시키자
            myTr.position = Vector3.Lerp(myTr.position, currPos, Time.deltaTime * 3.0f);
            //원격 플레이어의 아바타를 수신받은 각도만큼 부드럽게 회전시키자
            myTr.rotation = Quaternion.Slerp(myTr.rotation, currRot, Time.deltaTime * 3.0f);
        }
    }

    [ContextMenu("FuncStart")]
    void FuncStart()
    {
        Debug.Log("Func start");
        //Debug.Log(speed);
    }

    IEnumerator ModeSet()
    {
        while(!isDie)
        {
            yield return new WaitForSeconds(0.2f);
            float dist = Vector3.Distance(myTr.position, traceTarget.position);


            if(isHit)
            {
                enemyMode = MODE_STATE.hit;
            }
            else if(dist <= attackDist)
            {
                enemyMode = MODE_STATE.attack;
            }
            else if (traceAttack) 
            {
                enemyMode = MODE_STATE.trace; 
            }
            else if (dist <= traceDist) 
            {
                enemyMode = MODE_STATE.trace; 
            }
            else if (dist <= findDist) 
            {
                enemyMode = MODE_STATE.surprise;  
            }
            else if (hungry)
            {
                enemyMode = MODE_STATE.move;
            }
            else if (sleep)
            {
                enemyMode = MODE_STATE.sleep;
            }
            else
            {
                enemyMode = MODE_STATE.idle;
            }
        }

    }

    IEnumerator ModeAction()
    {
        while(!isDie)
        {
            switch(enemyMode)
            {
                case MODE_STATE.idle:
                    myTraceAgent.isStopped = true;

                    if(randAnim ==0)
                    {
                        _anim.CrossFade(anims.idle1.name, 0.3f);
                        net_Anim = 0;
                    }
                    else if (randAnim == 1)
                    {
                        _anim.CrossFade(anims.idle2.name, 0.3f);
                        net_Anim = 1;
                    }
                    else if (randAnim == 2)
                    {
                        _anim.CrossFade(anims.idle3.name, 0.3f);
                        net_Anim = 2;
                    }
                    else if (randAnim == 3)
                    {
                        _anim.CrossFade(anims.idle4.name, 0.3f);
                        net_Anim = 3;
                    }
                    break;

                case MODE_STATE.trace:
                    myTraceAgent.isStopped = false;
                    myTraceAgent.destination = traceTarget.position;

                    if(enemyKind == MODE_KIND.enemyboss)
                    {
                        myTraceAgent.speed = speed * 1.8f;
                        _anim[anims.move.name].speed = 1.8f;
                        _anim.CrossFade(anims.move.name, 0.3f);
                        net_Anim = 4;
                    }
                    else
                    {
                        myTraceAgent.speed = speed * 1.5f;

                        //애니메이션 속도 변경
                        _anim[anims.move.name].speed = 1.5f;

                        //run 애니메이션 
                        _anim.CrossFade(anims.move.name, 0.3f);
                        net_Anim = 5;
                    }
                    break;

                case MODE_STATE.attack:
                    myTraceAgent.isStopped = true;

                    //myTr.LookAt(traceTarget.position); //바로쳐다봄
                    Quaternion enemyLookRotation = Quaternion.LookRotation(traceTarget.position - myTr.position);
                    //enemyLookRotation.eulerAngles = new Vector3(myTr.rotation.x, enemyLookRotation.eulerAngles.y, myTr.rotation.z);
                    myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation, Time.deltaTime * 10.0f);
                    if (randAnim == 0)
                    {
                        //attack1 애니메이션 
                        _anim.CrossFade(anims.attack1.name, 0.3f);
                        net_Anim = 6;
                    }
                    else if (randAnim == 1)
                    {
                        //attack2 애니메이션 
                        _anim.CrossFade(anims.attack2.name, 0.3f);
                        net_Anim = 7;
                    }
                    else if (randAnim == 2)
                    {
                        //attack3 애니메이션 
                        _anim.CrossFade(anims.attack3.name, 0.3f);
                        net_Anim = 8;
                    }
                    else if (randAnim == 3)
                    {
                        //attack3 애니메이션 
                        _anim.CrossFade(anims.attack4.name, 0.3f);
                        net_Anim = 9;
                    }
                    break;
                case MODE_STATE.move:

                    // 네비게이션 재시작(추적)
                    myTraceAgent.isStopped = false;
                    // 추적대상 설정(로밍장소)
                    myTraceAgent.destination = roamingTarget.position;

                    //네비속도 및 애니메이션 속도 제어
                    if (enemyKind == MODE_KIND.enemyboss)
                    {
                        // 네비게이션의 추적 속도를 현재보다 1.2배
                        myTraceAgent.speed = speed * 1.2f;

                        //애니메이션 속도 변경
                        _anim[anims.move.name].speed = 1.2f;

                        //run 애니메이션 
                        _anim.CrossFade(anims.move.name, 0.3f);
                        net_Anim = 10;

                    }
                    else
                    {
                        // 네비게이션의 추적 속도를 현재 속도로...
                        myTraceAgent.speed = speed;

                        //애니메이션 속도 변경
                        _anim[anims.move.name].speed = 1.0f;

                        //walk 애니메이션 
                        _anim.CrossFade(anims.move.name, 0.3f);
                        net_Anim = 11;

                    }
                    break;
                case MODE_STATE.surprise:
                    if(!traceObject)
                    {
                        traceObject = true;
                        myTraceAgent.isStopped = true;
                        _anim.CrossFade(anims.surprise.name, 0.3f);
                        net_Anim = 12;
                        StartCoroutine(this.TraceObject());
                    }
                    break;
                case MODE_STATE.sleep:

                    //사운드 


                    //네비게이션 멈추고 (추적 중지) 
                    myTraceAgent.isStopped = true;

                    //sleep 애니메이션 
                    _anim.CrossFade(anims.sleep.name, 0.3f);
                    net_Anim = 13;
                    break;
                case MODE_STATE.hit:
                    myTraceAgent.isStopped = true;

                    if(randAnim ==0 || randAnim == 1)
                    {
                        _anim.CrossFade(anims.hit1.name, 0.3f);
                        net_Anim = 14;
                    }
                    else if(randAnim ==1 || randAnim ==2)
                    {
                        _anim.CrossFade(anims.hit2.name, 0.3f);
                        net_Anim = 15;
                    }
                    break;

            }

            yield return null;
        }
       
    }
    IEnumerator TargetSetting()
    {
        while (!isDie)
        {

            yield return new WaitForSeconds(0.2f);

            // 자신과 가장 가까운 플레이어 찾음
            players = GameObject.FindGameObjectsWithTag("Player");

            //플레이어가 있을경우 
            if (players.Length != 0)
            {
                playerTarget = players[0].transform;
                dist1 = (playerTarget.position - myTr.position).sqrMagnitude;
                foreach (GameObject _players in players)
                {
                    if ((_players.transform.position - myTr.position).sqrMagnitude < dist1)
                    {
                        playerTarget = _players.transform;
                        dist1 = (playerTarget.position - myTr.position).sqrMagnitude;
                    }
                }
            }

            //자신과 가장 가까운 베이스 찾음
           baseAll = GameObject.FindGameObjectsWithTag("Base");
            baseTarget = baseAll[0].transform;
            dist2 = (baseTarget.position - myTr.position).sqrMagnitude;
            foreach (GameObject _baseAll in baseAll)
            {
                if ((_baseAll.transform.position - myTr.position).sqrMagnitude < dist2)
                {
                    baseTarget = _baseAll.transform;
                    dist2 = (baseTarget.position - myTr.position).sqrMagnitude;
                }
            }

            //만약 플레이어가 없으면 베이스 목표 설정  
            if (players.Length == 0)
            {
                traceTarget = baseTarget;
            }
            //그렇지 않으면...
            else
            {
                // 플레이어가 베이스보다 우선순위가 높게 셋팅 (게임마다 틀리다 즉 자기 맘)
                if (dist1 <= dist2)
                {
                    traceTarget = playerTarget;
                }
                else
                {
                    traceTarget = baseTarget;
                }
            }

        }

    }

    /* Vetor3.Distance || Vetor3.sqrMagnitude 
 * 
 * Vetor3.Distance : 두 점간의 거리를 구해주는 메소드
 * Vetor3.sqrMagnitude : 두 점간의 거리의 제곱에 루트를 한 값 (두 점간의 거리의 차이를 2차원 함수의 값으로 계산)
 * 
 * 연산속도 : Vetor3.Distance < Vetor3.sqrMagnitude (퍼포먼스 향상)
 * 정확성   : Vetor3.Distance > Vetor3.sqrMagnitude
 */


    public void RoamingCheckStart()
    {
        StartCoroutine(this.RoamingCheck(roamingRandCheck));
    }
    IEnumerator RoamingCheck(int pos)
    {

        roamingRandCheck = Rand.Range(1, roamingCheck.Length);

        //같은 자리 안가게....
        if (roamingRandCheck == pos)
        {
            //중복값을 막기위하여 재귀함수 호출
            RoamingCheckStart();

            yield break;

        }

        //로밍 타겟 셋팅
        roamingTarget = roamingCheck[roamingRandCheck];

        //Debug.Log("Checking1");
    }
    IEnumerator TraceObject()
    {
        yield return new WaitForSeconds(2.5f);
        traceAttack = true;

        yield return new WaitForSeconds(5f);
        traceAttack = false;
        traceObject = false;
    }

    public void EnemyDie()
    {
        if(pv.isMine)
            StartCoroutine(this.Die());
    }
    IEnumerator Die()
    {
        isDie = true;
        _anim.CrossFade(anims.die.name, 0.3f);
        enemyMode = MODE_STATE.die;
        this.gameObject.tag = "Untagged";
        this.gameObject.transform.Find("EnemyBody").tag = "Untagged";

        myTraceAgent.isStopped = true;

        foreach(Collider coll in gameObject.GetComponentsInChildren<Collider>())
        {
            coll.enabled = false;
        }

        yield return new WaitForSeconds(4.5f);
        PhotonNetwork.Destroy(gameObject);
    }

    public void EnemyBarrelDie(Vector3 firePos)
    {
        StartCoroutine(this.BarrelDie(firePos));
    }
    IEnumerator BarrelDie(Vector3 firePos)
    {
        isDie = true;

        _anim.CrossFade(anims.die.name, 0.3f);
        net_Anim = 16;
        enemyMode = MODE_STATE.die;

        this.gameObject.tag = "Untagged";
        this.gameObject.transform.Find("EnemyBody").tag = "Untagged";
        myTraceAgent.enabled = false;

        Rigidbody rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = false;

        rigid.mass = 1.0f;
        //폭발력,원점,반경,위로솟구치는힘
        rigid.AddExplosionForce(1000.0f, firePos, 15.0f, 300.0f);
        yield return new WaitForSeconds(3.5f);
        rigid.isKinematic = true;

        foreach(Collider coll in gameObject.GetComponentsInChildren<Collider>())
        {
            coll.enabled = false;
        }

        yield return new WaitForSeconds(4.5f);
        PhotonNetwork.Destroy(gameObject);
    }


    void OnDestory()
    {
        Debug.Log("Destroy");
        //모든 코루틴 정지
        StopAllCoroutines();
    }
    public void HItEenmey()
    {
        int rand = Rand.Range(0, 100);
        if(rand<30)
        {
            if(randAnim == 0 || randAnim == 1)
            {
                isHitTime = Time.time + anims.hit1.length + 0.2f;
                isHit = true;
            }
            else if(randAnim == 1 || randAnim == 2)
            {
                isHitTime = Time.time + anims.hit1.length + 0.2f;
                isHit = true;
            }
        }

    }
    // 포톤 추가///////////////////////////////////////////////////////////////////////

    /*
	 * 마스터 클라이언트가 아닐때 애니메이션 상태를 동기화 하는 로직
     * RPC 로도 애니메이션 동기화 가능~!
	 */
    IEnumerator NetAnim()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            if (!PhotonNetwork.isMasterClient)
            {
                if (net_Anim == 0)
                {
                    _anim.CrossFade(anims.idle1.name, 0.3f);
                }
                else if (net_Anim == 1)
                {
                    _anim.CrossFade(anims.idle2.name, 0.3f);
                }
                else if (net_Anim == 2)
                {
                    _anim.CrossFade(anims.idle3.name, 0.3f);
                }
                else if (net_Anim == 3)
                {
                    _anim.CrossFade(anims.idle4.name, 0.3f);
                }
                else if (net_Anim == 4)
                {
                    //애니메이션 속도 변경
                    _anim[anims.move.name].speed = 1.8f;

                    //run 애니메이션 
                    _anim.CrossFade(anims.move.name, 0.3f);
                }
                else if (net_Anim == 5)
                {
                    //애니메이션 속도 변경
                    _anim[anims.move.name].speed = 1.5f;

                    //run 애니메이션 
                    _anim.CrossFade(anims.move.name, 0.3f);
                }
                else if (net_Anim == 6)
                {
                    //attack1 애니메이션 
                    _anim.CrossFade(anims.attack1.name, 0.3f);
                }
                else if (net_Anim == 7)
                {
                    //attack2 애니메이션 
                    _anim.CrossFade(anims.attack2.name, 0.3f);
                }
                else if (net_Anim == 8)
                {
                    //attack3 애니메이션 
                    _anim.CrossFade(anims.attack3.name, 0.3f);
                }
                else if (net_Anim == 9)
                {
                    //attack4 애니메이션 
                    _anim.CrossFade(anims.attack4.name, 0.3f);
                }
                else if (net_Anim == 10)
                {
                    //애니메이션 속도 변경
                    _anim[anims.move.name].speed = 1.2f;

                    //run 애니메이션 
                    _anim.CrossFade(anims.move.name, 0.3f);
                }
                else if (net_Anim == 11)
                {
                    //애니메이션 속도 변경
                    _anim[anims.move.name].speed = 1.0f;

                    //walk 애니메이션 
                    _anim.CrossFade(anims.move.name, 0.3f);
                }
                else if (net_Anim == 12)
                {
                    //roar 애니메이션 
                    _anim.CrossFade(anims.surprise.name, 0.3f);
                }
                else if (net_Anim == 13)
                {
                    //sleep 애니메이션 
                    _anim.CrossFade(anims.sleep.name, 0.3f);
                }
                else if (net_Anim == 14)
                {
                    // hit1 애니메이션 
                    _anim.CrossFade(anims.hit1.name, 0.3f);
                }
                else if (net_Anim == 15)
                {
                    // hit2 애니메이션 
                    _anim.CrossFade(anims.hit2.name, 0.3f);
                }
                else if (net_Anim == 16)
                {
                    //죽는 애니메이션 시작
                    _anim.CrossFade(anims.die.name, 0.3f);

                    // 코루틴 함수를 빠져나감(종료)
                    yield break;
                }

            }
        }

    }

    /*
     * PhotonView 컴포넌트의 Observe 속성이 스크립트 컴포넌트로 지정되면 PhotonView
     * 컴포넌트는 데이터를 송수신할 때, 해당 스크립트의 OnPhotonSerializeView 콜백 함수를 호출한다. 
     */
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 플레이어의 위치 정보를 송신
        if (stream.isWriting)
        {
            //박싱
            stream.SendNext(myTr.position);
            stream.SendNext(myTr.rotation);
            stream.SendNext(net_Anim);
        }
        //원격 플레이어의 위치 정보를 수신
        else
        {
            //언박싱
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            net_Anim = (int)stream.ReceiveNext();
        }

    }

    // 마스터 클라이언트가 변경되면 호출
    void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        if (PhotonNetwork.isMasterClient)
        {
            //일단 첫 Base의 Transform만 연결
            traceTarget = baseAll[0].transform;

            //추적하는 대상의 위치(Vector3)를 셋팅하면 바로 추적 시작 (가독성이 좋다)
            myTraceAgent.SetDestination(traceTarget.position);
            // 위와 같은 동작을 수행하지만...가독성이 별로다
            // myTraceAgent.destination = traceObj.position;

            // 정해진 시간 간격으로 Enemy의 Ai 변화 상태를 셋팅하는 코루틴
            StartCoroutine(ModeSet());

            // Enemy의 상태 변화에 따라 일정 행동을 수행하는 코루틴
            StartCoroutine(ModeAction());

            // 일정 간격으로 주변의 가장 가까운 Base와 플레이어를 찾는 코루틴 
            StartCoroutine(this.TargetSetting());

            // 로밍 루트 설정
            RoamingCheckStart();

            //myRbody.isKinematic = false;
            //네비게이션도 실행
            myTraceAgent.enabled = true;
        }
    }

    /*

    // 다음은 마스터 클라이언트의 명시적 변경을 예를 든거다

    // 접속 플레이어 정보 추가
    List<PhotonPlayer> setPlayer;

    //네트워크 플레이어가 룸으로 접속했을 때 호출되는 콜백 함수
    //PhotonPlayer 클래스 타입의 파라미터가 전달(서버에서...)
    //PhotonPlayer 파라미터는 해당 네트워크 플레이어의 정보를 담고 있다.
    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        // 플레이어 ID (접속 순번), 이름, 커스텀 속성
        Debug.Log(newPlayer.ToStringFull());

        if(PhotonNetwork.isMasterClient)
        {
            setPlayer.Add(newPlayer);
        }

    }

    // 마스터 클라이언트만 아래 함수 호출
    // 마스터 클라이언트 명시적 변경
    void SetMasterClient()
    {
        PhotonNetwork.SetMasterClient(setPlayer[0]);
    }

    */
}
