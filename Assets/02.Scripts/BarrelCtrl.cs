using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    const int MAX_HIT = 3; // 몇번 맞아야 폭파될거인가
    const float DELETE_TIME_EFFECT = 5.0f;
    const float DELETE_TIME_BARREL = 3.0f;
    const float FORCE_BARREL =1500.0f;
    const float MASS_BARREL = 1.0f;

   public GameObject expEffect;
   private Transform tr;
   private Rigidbody rb;
   private int hitCount =0;
    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
    }

    // 충돌 시 발생하는 콜백함수
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BULLET"))
        {
            // hitCount++;  (풀어쓴해석)
            // if(hitCount == MAX_HIT)
            if (++hitCount ==3)
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
        rb.mass = MASS_BARREL;
        // 위로 솟구치는 힘을 가함
        rb.AddForce(Vector3.up * FORCE_BARREL);
        // 3초후 드럼통 제거
        Destroy(gameObject, DELETE_TIME_BARREL);
    }
}
