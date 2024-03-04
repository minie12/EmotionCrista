using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public GameObject playerSpeechPrefab;
    public GameObject otherSpeechPrefab;
    public GameObject systemSpeechPrefab;

    public GameObject speechBoxHolder;
    public GameObject logUICanvas;

    public void OnLogActivate()
    {
        if (null == speechBoxHolder || null == logUICanvas)
        {
            return;
        }

        List<LogMessage> logQueue = new List<LogMessage>();

        LogCache.PopAllLog(logQueue);

        if (logQueue.Count != 0)
        {
            //string playerName = GameManager.Get().GetPlayerName();

            foreach (LogMessage logPair in logQueue)
            {
                if (logPair.logName.Length <= 0 || logPair.logName.Length <= 0)
                {
                    continue;
                }

                if ("Player" == logPair.logName)
                //if (playerName == logPair.logName)
                {
                    AddPlayerSpeech(logPair.logName, logPair.logText);
                }
                else if ("system" == logPair.logName)
                {
                    AddSystemSpeech(logPair.logText);
                }
                else
                {
                    AddOtherSpeech(logPair.logName, logPair.logText);
                }
            }
        }

        logUICanvas.SetActive(true);
    }

    protected void AddPlayerSpeech(string playerName, string speechText)
    {
        GameObject newPlayerSpeech = Instantiate(playerSpeechPrefab, speechBoxHolder.transform);
        if (null == newPlayerSpeech)
        {
            return;
        }

        LogSpeech LogSpeechComp = newPlayerSpeech.GetComponent<LogSpeech>();
        if (null != LogSpeechComp)
        {
            LogSpeechComp.SetSpeech(playerName, speechText);
        }
    }

    protected void AddOtherSpeech(string characterName, string speechText) 
    {
        GameObject newOtherSpeech = Instantiate(otherSpeechPrefab, speechBoxHolder.transform);
        if (null == newOtherSpeech)
        {
            return;
        }

        LogSpeech LogSpeechComp = newOtherSpeech.GetComponent<LogSpeech>();
        if (null != LogSpeechComp)
        {
            LogSpeechComp.SetSpeech(characterName, speechText);
        }
    }

    protected void AddSystemSpeech(string speechText) 
    {
        GameObject newSystemSpeech = Instantiate(systemSpeechPrefab, speechBoxHolder.transform);
        if (null == newSystemSpeech)
        {
            return;
        }

        LogSpeech LogSpeechComp = newSystemSpeech.GetComponent<LogSpeech>();
        if (null != LogSpeechComp)
        {
            LogSpeechComp.SetSpeech("", speechText);
        }
    }
}
