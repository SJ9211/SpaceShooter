using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    #region Public
    // 몬스터가 출현할 위치를 저장
    // public Transform[] points;

    // 몬스터가 출현할 위치를 저장할 List 타입 변수
    public List<Transform> points = new List<Transform>();
    public GameObject monster;
    public float createTime = 3.0f; // 몬스터의 생성 간격
    #endregion

    #region  private
    private bool isGameOver;

    #endregion

    
    public bool IsGameOver // 게임의 종료 여부를 저장할 프로퍼티
    {
        get { return isGameOver; }
        set
        {
            isGameOver = value;
            if (isGameOver)
            {
                CancelInvoke("CreateMonster");
            }
        }
    }

    // 싱글턴 인스턴스 선언
    public static GameManager instance = null;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        // 다른 씬으로 넘어가도 삭제되지 않고 유지됨
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        // SpawnPointGroup 게임오브젝트의 Transform 컴포넌트 추출
        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;

        // SpawnPointGroup 하위에 있는 모든 차일드 게임오브젝트의 Transform 컴포넌트 추출
        //points = spawnPointGroup?.GetComponentsInChildren<Transform>();

        /*        (아래있는 코드를 줄여서 한것이 위에있는 코드)
        GameObject spawnPointGroup = GameObject.Find("SpawnPointGroup");
        if (spawnPointGroup != null)
        {
            Transform spawnPointGroup = spawnPointGroup.GetComponent<Transform>();
            points = spawnPointGroup.GetComponentsInChildren<Transform>();
        }
        */

        //spawnPointGroup?.GetComponentsInChildren<Transform>(points); // 하위에있는 모든 차일드 게임오브젝트의 Transform 컴포넌트 추출 (List타입)
        foreach (Transform point in spawnPointGroup)
        {
            points.Add(point);
        }

        // 일정한 시간 간격으로 함수를 호출
        InvokeRepeating("CreateMonster", 2.0f, createTime);
    }

    void CreateMonster()
    {
        // 몬스터의 불규칙한 생성 위치 산출
        int idx = Random.Range(0, points.Count);

        // 몬스터 프리팹 생성
        Instantiate(monster, points[idx].position, points[idx].rotation);
    }

}
