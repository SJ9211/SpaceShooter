using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;       // UnityEvent 관련 API를 사용하기 위해 선언한 네임스페이스
using UnityEngine.UI;           // Unity-UI를 사용하기 위해 선언한 네임스페이스
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    // 버튼을 연결할 변수
    public Button startButton;
    public Button optionButton;
    public Button shopButton;

    private UnityAction action;

    void Start()
    {
        // UnityAction을 사용한 이벤트 연결 방식
        action = () => OnStartCilck();
        //action = () => OnButtonClick(startButton.name);
        startButton.onClick.AddListener(action);

        // 무명 매서드를 활용한 이벤트 연결 방식
        optionButton.onClick.AddListener(delegate { OnButtonClick(optionButton.name);});

        // 람다식을 활용한 이벤트 연결 방식
        shopButton.onClick.AddListener(()=> OnButtonClick(shopButton.name));

    }
    public void OnButtonClick(string msg)
    {
        Debug.Log($"Click Button : {msg}");
    }

    public void OnStartCilck()
    {
        SceneManager.LoadScene("Level_01");
        SceneManager.LoadScene("Play", LoadSceneMode.Additive);
    }
    #region  ButtonClick Event test
    /*
    public void OnButtonClick2(string msg)
    {
        Debug.Log($"Click Button : {msg}");
    }
    public void OnButtonClick3(int idx)
    {
        Debug.Log($"Click Button : {idx}");
        switch (idx)
        {
            case 1:
                Debug.Log("Start gane");
                break;
            case 2:
                Debug.Log("Options");
                break;
            case 3:
                Debug.Log("Shop");
                break;
        }
    }

    public void OnButtonClick4(RectTransform rt)
    {
        Debug.Log($"Click Button : {rt.localScale.x}");
    }
    */
    #endregion
}
