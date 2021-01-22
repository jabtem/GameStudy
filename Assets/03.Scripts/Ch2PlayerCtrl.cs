using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ch2PlayerCtrl : MonoBehaviour
{
    public float speed;      // 캐릭터 움직임 스피드.
    public float jumpSpeed; // 캐릭터 점프 힘.
    public float gravity;    // 캐릭터에게 작용하는 중력.

    CharacterController controller;
    Vector3 MoveDir;

    // Start is called before the first frame update
    void Start()
    {
        speed = 6.0f;
        jumpSpeed = 8.0f;
        gravity = 20.0f;

        MoveDir = Vector3.zero;
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(controller.isGrounded)
        {
            MoveDir = new Vector3(0, 0, Input.GetAxis("Vertical"));
            if(Input.GetAxis("Horizontal")>0)
                transform.Rotate(0, 5, 0);
            else if (Input.GetAxis("Horizontal") < 0)
                transform.Rotate(0, -5, 0);

            //벡터를 로컬->월드
            MoveDir = transform.TransformDirection(MoveDir);

            MoveDir *= speed;

            if (Input.GetButton("Jump"))
                MoveDir.y = jumpSpeed;
        }

        //캐릭터에 중력적용
        MoveDir.y -= gravity * Time.deltaTime;

        //캐릭터 움직임
        controller.Move(MoveDir * Time.deltaTime);

    }
}
