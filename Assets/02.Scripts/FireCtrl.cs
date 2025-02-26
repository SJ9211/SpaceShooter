using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

// 반드시 필요한 컴포넌트를 명시해 해당 컴포넌트가 삭제 안되도록 하는 어트리뷰트
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    // 총알 프리팹
    public GameObject bullet;
    // 총알 발사 좌표
    public Transform firePos;
    // 총소리에 사용할 오디오
    public AudioClip fireSfx;
    
    // AudioSource 컴포넌트를 저장하는 변수
    private new AudioSource audio;
    private MeshRenderer muzzleFlash;
    private bool isPlayerDie;

    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
    }
    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }
    void Start()
    {
        audio = GetComponent<AudioSource>();

        // FirePos 하위에 있는 MuzzleFlash의 머터리얼 컴포넌트 추출
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        // 처음 시작할떄 비활성
        muzzleFlash.enabled = false;
        isPlayerDie = false;
    }

    public void OnPlayerDie()
    {
        isPlayerDie = true;
    }
    void Update()
    {
        if (isPlayerDie) return;
        // 마우스 왼쪽 버튼을 클릭했을때 Fire 함수 호출
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    void Fire()
    {
        // Bullet 프리팹을 동적으로 생성 (생성할 객체, 위치, 회전)
        Instantiate(bullet, firePos.position, firePos.rotation);
        // 총소리 발생
        audio.PlayOneShot(fireSfx, 1.0f);

        // 총구 화염 효과 코루틴(coroutine)함수 호출
        // ShowMuzzleFlash();  오류는 안남 대신 적용 x 
        // StartCoroutine("ShowMuzzleFlash"); 이것도가능하나, GC발생
        StartCoroutine(ShowMuzzleFlash());

    }

    IEnumerator ShowMuzzleFlash()
    {
        // offset 좌표값을 랜덤 함수로 생성
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        // Texture 의 offset 값 설정
        muzzleFlash.material.mainTextureOffset = offset;

        // MuzzleFlash 회전 변경
        float angle = Random.Range(0, 360);
        muzzleFlash.transform.localRotation = Quaternion.Euler(0, 0, angle);

        // MuzlleFlash 크기 조절
        float scale = Random.Range(0.5f, 1.5f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        // MuzzleFlash 활성화
        muzzleFlash.enabled = true;
        // 0.2초 동안 대기하는 동안 메시지 루프로 제어권을 양보
        yield return new WaitForSeconds(0.2f);
        // MuzzleFlash 비활성화
        muzzleFlash.enabled = false;

    }
}
