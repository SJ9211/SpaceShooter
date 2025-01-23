using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    // 컴포넌트를 캐시 처리할 변수
    private Transform tr;
    private Animation anim;
    // 이동속도 변수
    public float moveSpeed = 10.0f;
    // 회전속도 변수
    public float turnSpeed = 80.0f;

    void Start()
    {
        // Transform 컴포넌트를 추출해 변수에 대입
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();

        // 애니메이션 실행
        anim.Play("Idle");

    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // -1.0f ~ 0.0f ~ +1.0f
        float v = Input.GetAxis("Vertical"); // -1.0f ~ 0.0f ~ + 1.0f
        float r = Input.GetAxis("Mouse X");

        // Debug.Log("h=" + h);
        // Debug.Log("v=" + v);

        //  Transform 컴포넌트의 position 속성값을 변경
        // transform.position += new Vector3(0, 0, 1);

        // 정규화 백터를 사용한 코드
        // transform.position += Vector3.forward * 1;

        // 전후좌우 이동 방향 벡터 계산
        Vector3 moveDir= (Vector3.forward *v) + (Vector3.right *h);
        // Translate (이동방향 * 속력 * Time.deltaTime)
        tr.Translate(moveDir * Time.deltaTime * moveSpeed);
        // Vector3.up 축을 기준으로 turnSpeed만큼의 속도를 회전
        tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);

        // 주인공 애니메이션 설정
        PlayerAnim(h,v);
    }

    void PlayerAnim(float h, float v)
    {
        if(v >= 0.1f)
        {
            anim.CrossFade("RunF", 0.25f); // 전진 
        }
        else if (v <= -0.1f)
        {
            anim.CrossFade("RunB", 0.25f); // 후진
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade("RunR", 0.25f); // 오른쪽
        }
        else if (h <= -0.1f)
        {
            anim.CrossFade("RunL", 0.25f);  // 왼쪽
        }
        else
        {
            anim.CrossFade("Idle", 0.25f); // 정지 시
        }
    }
}
