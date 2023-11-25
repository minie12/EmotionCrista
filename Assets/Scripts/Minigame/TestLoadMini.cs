using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLoadMini : MonoBehaviour
{
    public static int patternIdx = 4;
    public static int patternGimmick;
    public static int patternLevel = 5;

    public void YellowStamp()
    {
        patternIdx = (int)PatternType.YELLOW;
        patternGimmick = 0;
        patternLevel = (int)LevelType.EASY1;
        SceneManager.LoadScene("2_Mini");
    }
    public void YellowChat()
    {
        patternIdx = (int)PatternType.YELLOW;
        patternGimmick = 1;
        patternLevel = (int)LevelType.EASY1;
        SceneManager.LoadScene("2_Mini");
    }
    public void YellowHeartbeat()
    {
        patternIdx = (int)PatternType.YELLOW;
        patternGimmick = 2;
        patternLevel = (int)LevelType.EASY1;
        SceneManager.LoadScene("2_Mini");
    }

    public void BlueWater()
    {
        patternIdx = (int)PatternType.BLUE;
        patternGimmick = 0;
        patternLevel = (int)LevelType.EASY1;
        SceneManager.LoadScene("2_Mini");
    }
    public void BlueBurble()
    {
        patternIdx = (int)PatternType.BLUE;
        patternGimmick = 1;
        patternLevel = (int)LevelType.EASY1;
        SceneManager.LoadScene("2_Mini");
    }
    public void RedExplosion()
    {
        patternIdx = (int)PatternType.RED;
        patternGimmick = 0;
        patternLevel = (int)LevelType.EASY1;
        SceneManager.LoadScene("2_Mini");
    }
    public void RedFire()
    {
        patternIdx = (int)PatternType.RED;
        patternGimmick = 1;
        patternLevel = (int)LevelType.EASY1;
        SceneManager.LoadScene("2_Mini");
    }
    public void GreenBug()
    {
        patternIdx = (int)PatternType.GREEN;
        patternGimmick = 0;
        patternLevel = (int)LevelType.EASY1;
        SceneManager.LoadScene("2_Mini");
    }
    public void GreenArea()
    {
        patternIdx = (int)PatternType.GREEN;
        patternGimmick = 1;
        patternLevel = (int)LevelType.EASY1;
        SceneManager.LoadScene("2_Mini");
    }
    public void PurpleChain()
    {
        patternIdx = (int)PatternType.PURPLE;
        patternGimmick = 0;
        patternLevel = (int)LevelType.EASY1;
        SceneManager.LoadScene("2_Mini");
    }

    public void OnLoadTestMini()
    {
        SceneManager.LoadScene("2_MiniTest");
    }
}

