using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
// Iamge 를사용할경우 마이크로소프트가아닌 unityengine 로 바꿔야함
using UnityEngine.UI; 
public class PlayerCtrl : MonoBehaviour
{
    #region Private
    // 컴포넌트를 캐시 처리할 변수
    private Transform tr;
    private Animation anim;
    private readonly float initHp = 100.0f;  //초기 생명값
    private readonly float DAMAGE_HP = 10.0f;
    // Hpbar 연결할 변수
    private Image hpBar;
    #endregion

    #region Public
    public float currHp; // 현재 생명값
    public float moveSpeed = 10.0f;         // 이동속도 변수
    public float turnSpeed = 80.0f;         // 회전속도 변수
    public delegate void PlayerDieHandler(); //Delegate 선언
    public static event PlayerDieHandler OnPlayerDie; // event 선언
    #endregion

    IEnumerator Start()
    {
        // HPBar 연결
        /* ? 연산자
        GameObject go = GameObject.FindGameObjectWithTag("HP_BAR");
        if(go == null)
        {
            hpBar = null;
        }
        else
        {
            hpBar = hpBar.GetComponent<Image>();
        }
        */
        hpBar = GameObject.FindGameObjectWithTag("HP_BAR")?.GetComponent<Image>();
        currHp = initHp;
        // Transform 컴포넌트를 추출해 변수에 대입
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();

        // 애니메이션 실행
        anim.Play("Idle");

        turnSpeed = 0.0f;
        yield return new WaitForSeconds(0.3f);
        turnSpeed = 80.0f;

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

    private void OnTriggerEnter(Collider coll)
    {
        if ( currHp >= 0.0f && coll.CompareTag("PUNCH"))
        {
            currHp -= DAMAGE_HP;
            DisplayHealth();

            Debug.Log($"Player hp = {currHp/initHp}");
        }
        // Player의 생명이 0이하면 사망처리
        if ( currHp <= 0.0f)
        {
            PlayerDie();
        }
    }

    private void DisplayHealth()
    {
        hpBar.fillAmount = currHp/initHp;
    }

    private void PlayerDie()
    {
        Debug.Log("Player Die!");

        /*
        // MONSTER 태그를 가진 모든 게임오브젝트를 찾아옴
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");
        // 모든 몬스터의 OnPlaterDie 함수를 순차적으로 호출
        foreach(GameObject monster in monsters)
        {
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        }
        */

        // todo: UI 에서 "Game over" 라고 보여주게 하기
        // UI 직접 연결 말고 , 이벤트 호출을 통해서 

        //GetComponent<FireCtrl>().OnPlayerDie();
        
        // 주인공 사망 이벤트 호출(발생)
        OnPlayerDie();
        
        // GameManager 스크립트의 IsGameOver 프로퍼티 값을 변경
        GameObject.Find("GameMgr").GetComponent<GameManager>().IsGameOver = true;
    }
}
