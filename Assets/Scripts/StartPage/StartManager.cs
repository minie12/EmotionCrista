using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartManager : MonoBehaviour
{
    public SpriteRenderer background;

    public Sprite firstRoundBG;

    void Start()
    {
        RefreshStartScene();
    }

    public void GameStart()
    {
        GameManager gameManager = GameManager.Get();
        if (null != gameManager)
        {
            if(true == SystemManager.Get().IsMultiRound())
            {
                //gameManager.ResetPlayInfo();
                gameManager.ResetPlayInfoDebug();
                SceneManager.LoadScene("LabCorridor");
            }
            else 
            {
                gameManager.ResetPlayInfo();
                SceneManager.LoadScene("LabCorridor");
            }
        }
        else
        {
            Debug.LogError("No GameManager Found");
        }
    }

    public void ResetGame()
    {
        SystemManager.Get().EraseData();
        RefreshStartScene();
    }

    void RefreshStartScene()
    {
        if (false == SystemManager.Get().IsMultiRound())
        {
            background.sprite = firstRoundBG;
        }
    }
}
