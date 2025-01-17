using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    int gv = 0;

    void Awake()
    {
        gv =1;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        gv = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
