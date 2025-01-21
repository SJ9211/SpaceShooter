using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    // 컴포넌트를 캐시 처리할 변수
    [SerializeField]private Transform tr;
    // 이동속도 변수
    public float moveSpeed = 10.0f;

    void Start()
    {
        // Transform 컴포넌트를 추출해 변수에 대입
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // -1.0f ~ 0.0f ~ +1.0f
        float v = Input.GetAxis("Vertical"); // -1.0f ~ 0.0f ~ + 1.0f

        Debug.Log("h=" + h);
        Debug.Log("v=" + v);

        //  Transform 컴포넌트의 position 속성값을 변경
        // transform.position += new Vector3(0, 0, 1);

        // 정규화 백터를 사용한 코드
        // transform.position += Vector3.forward * 1;

        tr.Translate(Vector3.forward * Time.deltaTime* v * moveSpeed);
    }
}
