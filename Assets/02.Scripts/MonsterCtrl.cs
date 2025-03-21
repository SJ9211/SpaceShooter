using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Timeline;

public class MonsterCtrl : MonoBehaviour
{
    private const int HIT_MONSTER_HP = 10;

    #region Animator 추출값
    // Animator 해시값 추출
    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    private readonly int hashSpped = Animator.StringToHash("Spped");
    private readonly int hashDie = Animator.StringToHash("Die");
    
    #endregion

    public const float TIMER_CHECK = 0.3f;
    public enum State
    {
        IDEL,
        TRACE,
        ATTACK,
        DIE

    }
    #region Public
    public State state = State.IDEL;
    public float traceDist = 10.0f;
    public float attackDist = 2.0f;
    public bool isDie = false;
    #endregion

    #region private
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject bloodEffect;  // 혈흔 효과 프리팹
    private int hp = 100;
    #endregion

    // 스크립트가 활성화 될 떄마다 호출되는 함수
    void OnEnable()
    {
        // 이벤트 발생 시 수정할 함수 연결
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
        // 몬스터의 상태를 체크하는 코루틴 함수 호출
        StartCoroutine(CheckMonsterState());
        // 상태에 따라 몬스터의 행동을 수행하는 코루틴 함수 호출
        StartCoroutine(MonsterAction());

    }

    // 스크립트가 비활성화될 때마다 호출되는 함수
    void OnDisable()
    {
        // 기존에 연결된 함수 해제
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }
    void Awake()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        anim = GetComponent<Animator>();
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");
        //agent.destination = playrTr.position;


    }

    void Update()
    {
        // 목적지까지 남은 거리로 회전 여부 판단
        if (agent.remainingDistance >= 2.0f)
        {
            Vector3 direction = agent.desiredVelocity;
            // 회전 각도 산출
            Quaternion rot = Quaternion.LookRotation(direction);
            // 구면 선형보간 함수로 부드러운 회전 처리
            monsterTr.rotation = Quaternion.Slerp(monsterTr.rotation, rot, Time.deltaTime *10.0f);
        }
    }

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            // 0.3 동안 중지하는 동안 제어권을 메시지 루프에 양보
            yield return null;
            // 몬스터의 상태가 DIE 일때 코루틴을 종료
            if (state == State.DIE) yield break;
            // 몬스터와 주인공 캐릭터 사이의 거리 측정
            float distance = Vector3.Distance(playerTr.position, monsterTr.position);
            if (distance <= attackDist)
            {
                state = State.ATTACK;
            }
            else if (distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDEL;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist);
        }
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDEL:
                    agent.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    break;
                case State.TRACE:
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    anim.SetBool(hashAttack, false);
                    break;
                case State.ATTACK:
                    anim.SetBool(hashAttack, true);
                    break;
                case State.DIE:
                    isDie = true;
                    agent.isStopped = true;
                    anim.SetTrigger(hashDie);
                    // 몬스터의 collider 비활성화
                    GetComponent<CapsuleCollider>().enabled = false;
                    // 몬스터의 손에 달려 있는 collider 비활성화
                    SphereCollider[] sc = GetComponentsInChildren<SphereCollider>();
                    foreach (var item in sc)
                    {
                        item.enabled = false;
                    }

                    // 일정 시간 대기후 오브젝트 풀링으로 환원
                    yield return new WaitForSeconds(3.0f);
                    // 사항 후 다시 사용할 떄를 위해 hp값 초기화
                    hp = 100;
                    isDie = false;

                    GetComponent<CapsuleCollider>().enabled = true;
                    foreach (var item in sc)
                    {
                        item.enabled = false;
                    }
                    // 상태도 평소상태로 변경
                    state = State.IDEL;
                    // 몬스터 비활성화
                    this.gameObject.SetActive(false);
                    break;
            }
            yield return new WaitForSeconds(TIMER_CHECK);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET"))
        {
            // 충돌한 총알 삭제
            Destroy(collision.gameObject);
        }
    }
    public void OnDamage(Vector3 pos, Vector3 normal)
    {
        // 피격 애니메이션 실행
        anim.SetTrigger(hashHit);
          
        // 총알 총돌 지접의 법선 벡터
        Quaternion rot = Quaternion.LookRotation(normal);
        ShowBloodEffect(pos, rot);

        // 몬스터의 HP 처리
        hp -= HIT_MONSTER_HP;
        if ((hp <= 0) && (state != State.DIE))
        {
            state = State.DIE;
            GameManager.instance.DisplayScore(50);
            GameManager.instance.KillCount++;
        }
    }


private void ShowBloodEffect(Vector3 pos, Quaternion rot)
{
    // 혈흔 효과 생성
    GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot, monsterTr);
    Destroy(blood, 1.0f);
}

private void OnTriggerEnter(Collider other)
{
    Debug.Log(other.gameObject.name);
}

void OnPlayerDie()
{
        // 몬스터의 상태를 체크하는 코루틴 함수를 모두정지
        StopAllCoroutines();

        if ( state == State.DIE) return;
        
        // 추적을 정지하고 애니메이션을 수행
        agent.isStopped = true;
        anim.SetFloat(hashSpped, Random.Range(0.8f, 1.2f));
        anim.SetTrigger(hashPlayerDie);
    }
}
