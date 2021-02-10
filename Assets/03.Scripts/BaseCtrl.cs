using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class BaseCtrl : MonoBehaviour
{
    [HideInInspector]
    public bool isDie;

    public float dist1;
    public float dist2;
 
    private GameObject[] Enemys;
    public Transform EnemyTarget;

    //자신의 Transform 참조 변수
    private Transform myTr;

    // 회전의 중심축
    public Transform targetTr;

    // 터렛 발사 변수
    private bool shot;
    // 적을 봐라보는 회전 속도
    private float enemyLookTime;
    //적을 봐라보는 회전각
    private Quaternion enemyLookRotation;

    public Texture test;

    //총탄 프리팹을 위한 레퍼런스
    public GameObject bullet;

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

    //Ray 구조체
    Ray ray;
    //Ray에 충돌된 대상 정보 저장
    RaycastHit hitInfo;

    bool check;

    //레이저발사용
    public LineRenderer rayLine;

    //레이저 도트용
    public Transform rayDot;


    // 포톤 추가/////////////////////////////
    //PhotonView 컴포넌트를 할당할 레퍼런스 
    public PhotonView pv = null;

    //위치 정보를 송수신할 때 사용할 변수 선언 및 초기값 설정 
    Quaternion currRot = Quaternion.identity;

    //플레이어의 Id를 저장하는 변수
    public int playerId = -1;
    //몬스터의 파괴 스코어를 저장하는 변수 
    public int killCount = 0;
    //로컬  플레이어 연결 레퍼런스
    public PlayerCtrl localPlayer;

    // 로비체크용 변수
    public bool lobby;
    //////////////////////////////////////////////////////////


    void Awake()
    {
        //bullet = (GameObject)Resources.Load("Base/bullet", typeof(GameObject));
        //test = (Texture)Resources.Load("Base/bullet", typeof(Texture));

        //bullet = (GameObject)Resources.Load("Base/bullet", typeof(GameObject)) as GameObject;
        //test = (Texture)Resources.Load("Base/bullet");//조건이없으니 못받아옴
        //bullet = (GameObject)Resources.Load("Base/bullet");//조건이없어서 불안함

        bullet = Resources.Load<GameObject>("Base/bullet");
        test = Resources.Load<Texture>("Base/bullet");
        fireSfx = Resources.Load<AudioClip>("Base/bazooka");


        myTr = GetComponent<Transform>();
        source = GetComponent<AudioSource>();
        muzzleFlash.SetActive(false);

        // 포톤 추가/////////////////////////////////////////

        /*
         * (참고) 우린 수업시간에 Head에 컴포넌트 작업을 하였다. 따라서
         * PhotonView 를 이 오브젝트에 추가 하였다. 물론 최상위 Base 오브젝트에
         * 추가하고 이 스크립트를 연결 하여도 상관없지만 RPC 호출을 위하여 
         * 같은 차원상에 PhotonView 가 필요하기 때문에 Head 에 추가한거다.
         * 문제는 네트워크 유저가 나갈때 PhotonView 컴포넌트가 들어가있는
         * 게임오브젝트가 Destroy 되는데 Head에 PhotonView 가 들어가 있으므로
         * 단지, Head 만 사라지고 Base 게임오브젝트는 남는다. 이 문제를 해결하기
         * 위하여 우린 Base 게임오브젝트에 단순히 PhotonView 만 추가하면 된다.
         * 하지만 가장 최선은 수업을 진행하다보니 이렇게 된거지 Base 게임오브젝트
         * 부터 스크립트 작업을 시작하는거다...샘이 항상 말했듯이 
         * 빈 게임오브젝트 => 하위로 모델링 차일드 => 부모 게임오브젝트 부터 스크립트 작업
         * 이 순선의 중요성이다. 그냥 뭘하던 빈 게임오브젝트 부터!!!
         */

        //PhotonView 컴포넌트 할당 
        pv = GetComponent<PhotonView>();
        //PhotonView Observed Components 속성에 BaseCtrl(현재) 스크립트 Component를 연결
        pv.ObservedComponents[0] = this;
        //데이타 전송 타입을 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;

        //PhotonView의 ownerId를 playerId에 저장
        //유저 ownerId 부여(숫자 1부터~)
        playerId = pv.ownerId;

        // 원격 플래이어의 회전 값을 처리할 변수의 초기값 설정 
        currRot = myTr.rotation;

        // 로컬 플레이어 연결
        localPlayer = transform.root.GetComponent<PlayerCtrl>();
        // 플레이어와 분리
        transform.parent.parent.parent = null;

        // 끊긴 베이스 연결
        if (pv.isMine)
        {
            GameObject.FindWithTag("Mgr").GetComponent<StageManager>().baseStart = this;
        }
    }

    public void StartBase()
    {
        if(pv.isMine)
        {
            StartCoroutine(this.TargetSettting());
            StartCoroutine(this.ShotSettting());
        }

    }

    // Update is called once per frame
    void Update()
    {
        ray.origin = firePos.position;

        ray.direction = firePos.TransformDirection(Vector3.forward);
        Debug.DrawRay(ray.origin, ray.direction * 30.0f, Color.green);

        if(Physics.Raycast(ray,out hitInfo,30.0f))
        {
            Vector3 posValue = firePos.InverseTransformPoint(hitInfo.point);

            rayLine.SetPosition(0, posValue);
            rayDot.localPosition = posValue;

            if(pv.isMine && shot && hitInfo.collider.tag == "Enemy")
            {
                check = true;
            }
        }
        else
        {
            rayLine.SetPosition(0, new Vector3(0, 0, 30.0f));
            rayDot.localPosition = Vector3.zero;
        }
        if (pv.isMine || lobby)
        {
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
                    pv.RPC("ShotStart", PhotonTargets.Others, null);
                    bulletSpeed = Time.time + 0.3f;
                }
            }
        }
        else
        {
            myTr.rotation = Quaternion.Slerp(myTr.rotation, currRot, Time.deltaTime * 3.0f);
        }
     
    }

    IEnumerator TargetSettting()
    {
        while(!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            Enemys = GameObject.FindGameObjectsWithTag("EnemyBody");
            Transform EnemyTargets = Enemys[0].transform;
            float dist = (EnemyTargets.position - myTr.position).sqrMagnitude;
            foreach(GameObject _Enemy in Enemys)
            {
                if((_Enemy.transform.position - myTr.position).sqrMagnitude <dist)
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
        while(!isDie)
        {
            yield return new WaitForSeconds(0.2f);
            //dist1 = (EnemyTarget.position - myTr.position).sqrMagnitude;
            dist2 = Vector3.Distance(myTr.position, EnemyTarget.position);

            if(dist2<37.0f)
            {
                shot = true;
            }
            else
            {
                shot = false;
            }
        }
        
    }
    [PunRPC]
    //발사
    private void ShotStart()
    {
        StartCoroutine(this.FireStart());
    }

    IEnumerator FireStart()
    {
        BulletCtrl obj = Instantiate(bullet, firePos.position, firePos.rotation).GetComponent<BulletCtrl>();

        obj.playerId = pv.ownerId;

        source.PlayOneShot(fireSfx, fireSfx.length + 0.2f);
        float scale = Random.Range(1.0f, 2.5f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));

        muzzleFlash.transform.localRotation = rot;

        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

        muzzleFlash.SetActive(false);

        
    }

    void Fire()
    {
        shot = true;
    }

    public void PlusKillCount()
    {
        //Enemy 파괴 스코어 증가
        ++killCount;
        //HUD Text UI 항목에 스코어 표시
        localPlayer.txtKillCount.text = killCount.ToString();

        /* 포톤 클라우드에서 제공하는 플레이어의 점수 관련 메서드
         * 
         * PhotonPlayer.AddScore ( int score )      점수를 누적
         * PhotonPlayer.SetScore( int totScore )    해당 점수로 셋팅
         * PhotonPlayer.GetScore()                  현재 점수를 조회
         * 
         */

        //스코어를 증가시킨 베이스가 자신인 경우에만 저장
        if (pv.isMine)
        {
            /*PhotonNetwork.player는 로컬 플레이어 즉 자신을 의미한다.
               즉 다음 로직은 자기 자신의 스코어에 1점을 증가시킨다. 이 정보는 동일 룸에
               입장해있는 다른 네트워크 플레이어와 실시간으로 공유된다.*/
            PhotonNetwork.player.AddScore(1);
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
            stream.SendNext(myTr.rotation);
        }
        //원격 플레이어의 위치 정보를 수신
        else
        {
            //언박싱
            currRot = (Quaternion)stream.ReceiveNext();
        }

    }
}
