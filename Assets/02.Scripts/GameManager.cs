using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    // 몬스터가 출현할 위치를 저장
    // public Transform[] points;

    // 몬스터가 출현할 위치를 저장할 List 타입 변수
    public List<Transform> points = new List<Transform>();
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
        foreach ( Transform point in spawnPointGroup)
        {
            points.Add(point);
        }
    }

}
