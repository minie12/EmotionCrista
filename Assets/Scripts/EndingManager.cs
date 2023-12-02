using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EndingMode
{
    None = 0, // has not entered ending root yet
    Discard,
    HardWorking,
    Romance,
    TrueEnding,
    HiddenEnding,
    Max
}

public class EndingManager : MonoBehaviour
{
    public void StartEnding(int inEndingMode)
    {
        GameManager.Get().SetEndingMode((EndingMode)inEndingMode);
    }

    public void EndDiscardEnding()
    {
        SystemManager systemManager = SystemManager.Get();

        if (null != systemManager)
        {
            systemManager.SetMultiRound(true);
            systemManager.SaveGameSetting();
        }
        else
        {
            Debug.LogError("SystemManager not found.");
        }    
    }

    public void HardWorkingEnding()
    { }
}
