using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterCtrl : MonoBehaviour
{
    
    #region private
    private Transform monsterTr;
    private Transform playrTr;
    private NavMeshAgent agent;
    #endregion
    void Start()
    {
        monsterTr = GetComponent<Transform>();
        playrTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        agent.destination = playrTr.position;
    }

   
    void Update()
    {
        
    }
}
