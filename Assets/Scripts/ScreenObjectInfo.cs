using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScreenObjectInfo : MonoBehaviour
{
    [Serializable]
    class ClickableObject
    {
        public ClickableID ID;
        public GameObject obj;
    }

    [SerializeField]
    private ClickableObject[] clickableObjects;

    [SerializeField]
    private GameObject[] clickableLocations;

    [SerializeField]
    private Transform clickablesContainer;

    // Objects will be visible all at once (ShowAllObjects)
    public void SetClickableObject(ClickableID inObjectID, int inLocationIndex)
    {
        if (ClickableID.CID_None == inObjectID || ClickableID.CID_Max == inObjectID)
            return;

        ClickableObject clickableObject = GetClickableObject(inObjectID);
        if ((null != clickableObject) && (null != clickableObject.obj))
        {
            // Reset alpha to 0
            SpriteRenderer ren = clickableObject.obj.GetComponent<SpriteRenderer>();
            if (null != ren)
            {
                ren.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }

            // Locate GameObject
            clickableObject.obj.transform.SetParent(clickableLocations[inLocationIndex].transform, false);
        }
    }

    public bool IsLocationOccupied(int inLocationIndex)
    {
        return ((null != clickableLocations[inLocationIndex]) && (0 < clickableLocations[inLocationIndex].transform.childCount));
    }

    public void CleanClickableObject(ClickableID inObjectID)
    {
        ClickableObject clickableObject = GetClickableObject(inObjectID);
        if ((null != clickableObject) && (null != clickableObject.obj))
        {
            // move to Container
            clickableObject.obj.transform.SetParent(clickablesContainer, false);
        }
    }

    public void FadeClickable(bool bFadeIn, ClickableID inClickableID)
    {
        float fadeDuration = 0.5f;
        float fadeValue = (true == bFadeIn) ? 1.0f : 0.0f;

        ClickableObject clickableObject = GetClickableObject(inClickableID);

        if ((null != clickableObject) &&
            (null != clickableObject.obj))
        {
            // fade in
            LeanTween.alpha(clickableObject.obj, fadeValue, fadeDuration)
            .setEase(LeanTweenType.easeInQuad/*inEaseType*/)
            .setRecursive(false);
            //.setOnComplete(() => );
        }
    }

    private ClickableObject GetClickableObject(ClickableID inObjectID)
    {
        if (ClickableID.CID_None == inObjectID || ClickableID.CID_Max == inObjectID)
            return null;

        foreach (ClickableObject clickableObj in clickableObjects)
        {
            if (inObjectID == clickableObj.ID)
            {
                return clickableObj;
            }
        }

        return null;
    }
}
