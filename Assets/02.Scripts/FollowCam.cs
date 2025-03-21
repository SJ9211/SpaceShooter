using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    // 따라가야 할 대상을 연결하는 변수
    public Transform targetTr;
    // Main Camera 자신의 Transform 
    private Transform camTr;

    // 따라갈 대상으로부터 떨어질 거리
    [Range(2.0f, 20.0f)]
    public float distance = 10.0f;

    // Y축으로 이동할 높이
    [Range(0.0f, 10.0f)]
    public float height =2.0f;
    
    void Start()
    {
        camTr = GetComponent<Transform>();
    }

    //반응속도
    public float damping = 10.0f;

    //camera offset
    public float targetOffset = 2.0f;
    // smoothdamp
    private Vector3 velocity = Vector3.zero;
    void LateUpdate()
    {
        // 추적해야 할 대상의 뒤쪽으로 distance만큼 이동
        // 높이를 height만큼 이동
        Vector3 pos =  targetTr.position
                            + (-targetTr.forward * distance)
                            + (Vector3.up * height);
        // 구면 선형 보간 함수를 사용해 부드럽게 위치를 변경
       // camTr.position = Vector3.Slerp(camTr.position,                  //시작위치
        //                                     pos,                        //목표위치
         //                                    Time.deltaTime * damping);  // 시간 t
        // smoothdamp
        camTr.position = Vector3.SmoothDamp(camTr.position,           //시작위치
                                                  pos,                //목표위치
                                                  ref velocity,       //현재속도
                                                  damping);           //목표위치까지 도달할 시간
        
        // Camera를 피벗 좌표를 향해 회전
        camTr.LookAt(targetTr.position + (targetTr.up * targetOffset));
    }
}
