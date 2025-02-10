using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    const int MAX_HIT = 3; // 몇번 맞아야 폭파될거인가
    const float DELETE_TIME_EFFECT = 5.0f;
    const float DELETE_TIME_BARREL = 3.0f;
    const float FORCE_BARREL = 1500.0f;
    const float MASS_BARREL = 1.0f;

    public GameObject expEffect;
    public Texture[] textures;
    // 폭발 반경
    public float radius = 10.0f;
    private new MeshRenderer renderer;
    private Transform tr;
    private Rigidbody rb;
    private int hitCount = 0;
    private Collider[] colls = new Collider[10];
    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        renderer = GetComponentInChildren<MeshRenderer>();

        // 난수 발생
        int idx = Random.Range(0, textures.Length);
        // 텍스터 지정
        renderer.material.mainTexture = textures[idx];
    }

    // 충돌 시 발생하는 콜백함수
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BULLET"))
        {
            // hitCount++;  (풀어쓴해석)
            // if(hitCount == MAX_HIT)
            if (++hitCount == 3)
            {
                ExpBarrel();
            }
        }
    }
    void ExpBarrel()
    {
        //폭발 효과 파티클 생성
        GameObject exp = Instantiate(expEffect, tr.position, Quaternion.identity);
        // 5초후 파티클 제거
        Destroy(exp, DELETE_TIME_EFFECT);
        // rb mass를 1.0으로 수정해 무게를 가볍게 함
        // rb.mass = MASS_BARREL;
        // 위로 솟구치는 힘을 가함
        //rb.AddForce(Vector3.up * FORCE_BARREL);
        // 간접 폭발력 전달
        IndirectDamage(tr.position);
        // 3초후 드럼통 제거
        Destroy(gameObject, DELETE_TIME_BARREL);
    }

    void IndirectDamage(Vector3 pos)
    {
        // 주변에 있는 드럼통을 모두 추출
        //Collider[] colls = Physics.OverlapSphere(pos, radius, 1 << 3);
        // GC 없이 사용하는 걸로 변경        
        Physics.OverlapSphereNonAlloc(pos, radius, colls, 1 << 3);
        // 1<<8 | 1<<9 : OR연산 . 8번 또는 9번 레이어
        // ~( 1<<8 ) : NOT연산. 8번 레이어를 제외한 나머지 레이어
        // 1<<3 & 1<<4 & 1<<7
        foreach (var coll in colls)
        {
            //  coll의 null 체크 : null일때는 아래 코드 건너뜀
            if (coll == null) continue;
            {
                // 폭발 범위에 포함된 드럼통의 rb 추출
                rb = coll.GetComponent<Rigidbody>();
                // 드럼통 무게를 가볍게
                rb.mass = 1.0f;
                // freezeRptation 제한값 해제
                rb.constraints = RigidbodyConstraints.None;
                // 폭발력 전달
                rb.AddExplosionForce(FORCE_BARREL, pos, radius, 1200.0f);
            }
        }
    }
}
