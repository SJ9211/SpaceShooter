using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using JetBrains.Annotations;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    #region Public
    // 몬스터가 출현할 위치를 저장
    // public Transform[] points;
    public const string KEY_SCORE = "TOT_SCORE";
    public const int MAX_KILL = 99;
    public const int MAX_SCORE = 99999;
    // 몬스터가 출현할 위치를 저장할 List 타입 변수
    public List<Transform> points = new List<Transform>();
    // 몬스터를 미리 생성해 저장할 리스트 
    public List<GameObject> monsterPool = new List<GameObject>();
    // 오브젝트 풀에 생성할 몬스터의 최대 개수
    public int maxMonsters = 10;
    public GameObject monster;
    public float createTime = 3.0f; // 몬스터의 생성 간격
    public TMP_Text scoreText; // 스코어 텍스트를 연결할 변수
    public GameObject PanelGameOver;
    public TMP_Text killText;
       
    #endregion

    #region  private
    private bool isGameOver;
    private int totScore = 0; // 누적 점수를 기록하기 위한 변수

    #region  Propoty
    private int killCount= 0;
    #endregion

    #endregion
    public int KillCount
    {
        get { return killCount;}
        set { 
            killCount = value;
            if ( killCount > MAX_KILL)
            {
                killCount = MAX_KILL;
            }
            killText.text = $"{KillCount:00}";
        }
    }

    public bool IsGameOver // 게임의 종료 여부를 저장할 프로퍼티
    {
        get { return isGameOver; }
        set
        {
            isGameOver = value;
            if (isGameOver)
            {
                PanelGameOver.SetActive(true);
                CancelInvoke("CreateMonster");
            }
        }
    }

    // 싱글턴 인스턴스 선언
    public static GameManager instance = null;

    void Awake()
    {
        if (instance == null)
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
        PanelGameOver.SetActive(false);
        // 몬스터 오브젝트 풀 생성
        CreateMonsterPool();

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

        // 스코어 점수 출력
        totScore = PlayerPrefs.GetInt(KEY_SCORE, 0);
        DisplayScore(0);
    }

    void CreateMonster()
    {
        // 몬스터의 불규칙한 생성 위치 산출
        int idx = Random.Range(0, points.Count);

        // 몬스터 프리팹 생성
        //Instantiate(monster, points[idx].position, points[idx].rotation);


        // 오브젝트 풀에서 몬스터 추출
        GameObject _monster = GetMonsterInPool();

        // 추출한 몬스터의 위치와 회전을 설정
        _monster?.transform.SetPositionAndRotation(points[idx].position,
                                                    points[idx].rotation);

        // 추출한 몬스터 활성화
        _monster?.SetActive(true);
    }
    // 오브젝트 풀에 몬스터 생성
    private void CreateMonsterPool()
    {
        for (int i = 0; i < maxMonsters; i++)
        {
            // 몬스터 생성
            var _monster = Instantiate<GameObject>(monster);
            // 몬스터 이름
            _monster.name = $"Monster_{i:00}";
            // 몬스터 비활성화
            _monster.SetActive(false);
            // 생성한 몬스터를 오브젝트 풀에 추가
            monsterPool.Add(_monster);
        }
    }

    public GameObject GetMonsterInPool()
    {
        // 오브젝트 풀의 처음부터 끝까지 순회
        foreach (var _monster in monsterPool)
        {
            // 비활성화 여부로 사용 가능한 몬스터를 판단
            if (_monster.activeSelf == false)
            {
                // 몬스터 반환
                return _monster;
            }
        }

        return null;
    }

    public void DisplayScore(int score)
    {
        totScore += score;
        // 스코어 최대수치 
        if ( totScore > MAX_SCORE)
        {
            totScore = MAX_SCORE;
        }
        scoreText.text = $"<color=#00ff00>SCORE : </color> <color=#ff0000>{totScore:#,##0}</color>";
        // 스코어 저장
        PlayerPrefs.SetInt(KEY_SCORE, totScore);
    }

    // 에디터에 내가 만든 파일이 만들어짐
    [MenuItem("JungKang/Reset score")]
    public static void ResetScore()
    {
        PlayerPrefs.SetInt(KEY_SCORE, 0);
        Debug.Log("Successfully reset score");
    }
}