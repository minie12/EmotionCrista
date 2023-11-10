using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartMenuManager : MonoBehaviour
{
    public void FirstRoundStart(string inSceneName)
    {
        GameManager gameManager = GameManager.Get();
        if (null != gameManager)
        {
            gameManager.SetFirstRoundPlayInfo();
            SceneManager.LoadScene("LabCorridor");
        }
        else
        {
            Debug.LogError("No GameManager Found");
        }
    }
}
