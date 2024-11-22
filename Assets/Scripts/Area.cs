using System;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;

public enum ClickableID
{
    CID_None = 0,
    CID_Naria,
    CID_Lulian,
    CID_Russel,
    CID_Nish,
    CID_Ilrak,
    CID_Patrick,
    CID_Raz,
    CID_AndroidA,

    CID_Mascot,

    CID_Max
}

[System.Serializable]
public class Area
{
    [Serializable]
    public class ClickableInfo
    {
        public int locatedIndex;
        public int clickCount;

        public void Reset()
        {
            locatedIndex = -1;
            clickCount = 0;
        }
    }

    public string sceneName;

    public bool bSceneStartBlockCompleted;

    public Dictionary<ClickableID, ClickableInfo> clickableInfoMap = new Dictionary<ClickableID, ClickableInfo>();

    [NonSerialized]
    private ScreenObjectInfo sceneScreenInfo;

    public void OnSceneLoaded()
    {
        if (null == sceneScreenInfo)
        {
            GameObject screenInfo = GameObject.Find("ClickableLocation");
            if (null != screenInfo)
            {
                sceneScreenInfo = screenInfo.GetComponent<ScreenObjectInfo>();
            }
        }

        if (null != sceneScreenInfo)
        {
            foreach (KeyValuePair<ClickableID, ClickableInfo> clickableInfo in clickableInfoMap)
            {
                if (0 <= clickableInfo.Value.locatedIndex)
                {
                    SetClickableObject(clickableInfo.Key, clickableInfo.Value.locatedIndex);
                }
            }
        }

        bool bShowClickables = false;

        GameObject sayDialog = GameObject.Find("SayDialog");
        if (sayDialog == null)
        {
            bShowClickables = true;
        }
        else
        {
            bShowClickables = (false == sayDialog.activeSelf);
        }

        if (true == bShowClickables)
        {
            ChangeAllClickablesVisibility(true);
        }
    }

    public void SetClickableObject(ClickableID inObjectID, int inLocationIndex)
    {
        if (null != sceneScreenInfo)
        {
            if (false == clickableInfoMap.ContainsKey(inObjectID))
            {
                clickableInfoMap[inObjectID] = new ClickableInfo();

                clickableInfoMap[inObjectID].Reset();
            }

            ClickableInfo clickableInfo = clickableInfoMap[inObjectID];
            clickableInfo.locatedIndex = inLocationIndex;

            sceneScreenInfo.SetClickableObject(inObjectID, inLocationIndex);
        }
    }

    public void CleanClickableObject(ClickableID inObjectID)
    {
        if (null != sceneScreenInfo)
        {
            if (true == clickableInfoMap.ContainsKey(inObjectID))
            {
                ClickableInfo clickableInfo = clickableInfoMap[inObjectID];

                clickableInfo.locatedIndex = -1;
            }

            sceneScreenInfo.CleanClickableObject(inObjectID);
        }
    }

    public void ChangeAllClickablesVisibility(bool bShow)
    {
        if (null == sceneScreenInfo)
        {
            return;
        }

        foreach (KeyValuePair<ClickableID, ClickableInfo> clickableInfo in clickableInfoMap)
        {
            if (0 <= clickableInfo.Value.locatedIndex)
            {
                sceneScreenInfo.FadeClickable(bShow, clickableInfo.Key);
            }
        }
    }

    public void OnObjectClicked(ClickableID inObjectID)
    {
        if (true == clickableInfoMap.ContainsKey(inObjectID))
        {
            ClickableInfo clickableInfo = clickableInfoMap[inObjectID];

            ++clickableInfo.clickCount;
        }
    }

    public bool IsObjectClicked(ClickableID inObjectID)
    {
        if (null != sceneScreenInfo)
        {
            if (true == clickableInfoMap.ContainsKey(inObjectID))
            {
                return (0 < clickableInfoMap[inObjectID].clickCount);
            }
        }

        return false;
    }
}
