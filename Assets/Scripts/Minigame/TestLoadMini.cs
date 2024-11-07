using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLoadMini : MonoBehaviour
{
    public static int patternIdx = 0;
    public static int patternGimmick;
    public static int patternLevel = 0;

    public void OnLoadTestMini()
    {
        SceneManager.LoadScene("2_MiniTest");
    }
}

