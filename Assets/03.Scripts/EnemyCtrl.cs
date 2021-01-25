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
        Debug.Log("EDITOR");
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

        myTr = GetComponent<Transform>();

        baseAll = GameObject.FindGameObjectsWithTag("Base");

        roamingCheck = GameObject.Find("RoamingPoint").GetComponentsInChildren<Transform>();
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

        traceTarget = baseAll[0].transform;
        myTraceAgent.SetDestination(traceTarget.position);

        StartCoroutine(ModeSet());
        StartCoroutine(ModeAction());
        StartCoroutine(this.TargetSetting());
        RoamingCheckStart();

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
                    }
                    else if (randAnim == 1)
                    {
                        _anim.CrossFade(anims.idle2.name, 0.3f);
                    }
                    else if (randAnim == 2)
                    {
                        _anim.CrossFade(anims.idle3.name, 0.3f);
                    }
                    else if (randAnim == 3)
                    {
                        _anim.CrossFade(anims.idle4.name, 0.3f);
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
                    }
                    else
                    {
                        myTraceAgent.speed = speed * 1.5f;

                        //애니메이션 속도 변경
                        _anim[anims.move.name].speed = 1.5f;

                        //run 애니메이션 
                        _anim.CrossFade(anims.move.name, 0.3f);
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
                    }
                    else if (randAnim == 1)
                    {
                        //attack2 애니메이션 
                        _anim.CrossFade(anims.attack2.name, 0.3f);
                    }
                    else if (randAnim == 2)
                    {
                        //attack3 애니메이션 
                        _anim.CrossFade(anims.attack3.name, 0.3f);
                    }
                    else if (randAnim == 3)
                    {
                        //attack3 애니메이션 
                        _anim.CrossFade(anims.attack4.name, 0.3f);
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

                    }
                    else
                    {
                        // 네비게이션의 추적 속도를 현재 속도로...
                        myTraceAgent.speed = speed;

                        //애니메이션 속도 변경
                        _anim[anims.move.name].speed = 1.0f;

                        //walk 애니메이션 
                        _anim.CrossFade(anims.move.name, 0.3f);

                    }
                    break;
                case MODE_STATE.surprise:
                    if(!traceObject)
                    {
                        traceObject = true;
                        myTraceAgent.isStopped = true;
                        _anim.CrossFade(anims.surprise.name, 0.3f);
                        StartCoroutine(this.TraceObject());
                    }
                    break;
                case MODE_STATE.sleep:

                    //사운드 


                    //네비게이션 멈추고 (추적 중지) 
                    myTraceAgent.isStopped = true;

                    //sleep 애니메이션 
                    _anim.CrossFade(anims.sleep.name, 0.3f);
                    break;
                case MODE_STATE.hit:
                    myTraceAgent.isStopped = true;

                    if(randAnim ==0 || randAnim == 1)
                    {
                        _anim.CrossFade(anims.hit1.name, 0.3f);
                    }
                    else if(randAnim ==1 || randAnim ==2)
                    {
                        _anim.CrossFade(anims.hit2.name, 0.3f);
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

        Debug.Log("Checking1");
    }
    IEnumerator TraceObject()
    {
        yield return new WaitForSeconds(2.5f);
        traceAttack = true;

        yield return new WaitForSeconds(5f);
        traceAttack = false;
        traceObject = false;
    }
}
