//전처리기는 최상단
#define UNITY_EDTIOR
#define CBT_MODE
#define RELEASE_MODE


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ios를쓸경우
//using UnityEngine.iOS

public class DestructionRay : MonoBehaviour
{
    Ray ray;
    //ray에 맞은 오브젝트 저정보 저장하는 구조체
    RaycastHit hitInfo;

    public GameObject fireEffect;
    // Start is called before the first frame update


    private void OnDrawGizmosSelected()
    {
        Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(100, 100, Camera.main.nearClipPlane));
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(p, 5.0f);
    }    
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //메인카메라에서 마우스커서 (vector3이나 z값무시한값(0~1280,0~800,0)의 위치 해상도따라다름
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#if MODE_1
        Debug.DrawRay(ray.origin, ray.direction * 150.0f, Color.green);

#elif MODE_2
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.red);
#endif
        //PC기준
#if UNITY_EDTIOR
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hitInfo, 150.0f))
            {
                if (hitInfo.collider.tag == "Destory")
                {
                    //파티클 생성
                    Instantiate(fireEffect, hitInfo.point, Quaternion.identity);

                    //오브젝트 제거
                    Destroy(hitInfo.collider.gameObject);
                }
            }
        }
#endif


//모바일
#if UNITY_ANDROID
     if (Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Began)
     {
            ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            if(Physics.Raycast(ray,out hitInfo,150.0f))
            {
                if(hitInfo.collider.tag == "Destory")
                {
                    //파티클 생성
                    Instantiate(fireEffect, hitInfo.point, Quaternion.identity);

                    //오브젝트 제거
                    Destroy(hitInfo.collider.gameObject);
                }
            }
     }
#endif

#if UNITY_IPHONE
     if (Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Began)
     {
            ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            if(Physics.Raycast(ray,out hitInfo,150.0f))
            {
                if(hitInfo.collider.tag == "Destory")
                {
                    //파티클 생성
                    Instantiate(fireEffect, hitInfo.point, Quaternion.identity);

                    //오브젝트 제거
                    Destroy(hitInfo.collider.gameObject);
                }
            }
     }
#endif
    }
}
