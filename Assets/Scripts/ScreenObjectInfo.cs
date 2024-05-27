using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ScreenObjectInfo : MonoBehaviour
{
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
        CID_Max
    }

    [System.Serializable]
    public struct ClickableObject
    {
        public ClickableID ID;
        public GameObject obj;
    }

    static byte LOCATIONNUM_MAX = 8; // because Flag is byte

    byte objectClickedFlag = 0b_0000_0000;

    public List<ClickableObject> clickableObjects;
    [SerializeField]
    private GameObject[] clickableLocations = new GameObject[LOCATIONNUM_MAX];

    // Index : LocationIndex, Value : Clickable Object to be located
    private ClickableID[] locatedObjects = new ClickableID[LOCATIONNUM_MAX];

    [SerializeField]
    private Transform clickablesContainer;

    // Objects will be visible all at once (ShowAllObjects)
    public void SetClickableObject(ClickableID inObjectID, int inLocationIndex)
    {
        if (ClickableID.CID_None == inObjectID || ClickableID.CID_Max == inObjectID)
            return;
        
        while (true == locatedObjects.Contains(inObjectID))
        {
            CleanClickableObject(inObjectID);
        }

        GameObject clickableObject = GetClickableObject(inObjectID);
        if (null != clickableObject)
        {
            // Set Location : ClickableID info
            locatedObjects[inLocationIndex] = inObjectID;

            // Reset alpha to 0
            SpriteRenderer ren = clickableObject.GetComponent<SpriteRenderer>();
            if (null != ren)
            {
                ren.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }

            // Reset ClickedFlag to False
            byte mask = (byte)(1 << inLocationIndex);
            objectClickedFlag &= (byte)~mask;

            // Locate GameObject
            clickableObject.transform.SetParent(clickableLocations[inLocationIndex].transform, false);
        }
    }

    public void OnObjectClicked(ClickableID inObjectID, bool bClean)
    {
        if (true == bClean)
        {
            CleanClickableObject(inObjectID);
        }
        else
        {
            // set Clicked flag
            for (byte locationIndex = 0; locationIndex < LOCATIONNUM_MAX; ++locationIndex)
            {
                if (inObjectID == locatedObjects[locationIndex])
                {
                    // Set VisibleFlag to True
                    byte mask = (byte)(1 << locationIndex);
                    objectClickedFlag |= mask;

                    break;
                }
            }
        }
    }
    
    public void CleanClickableObject(ClickableID inObjectID)
    {
        GameObject clickableObject = GetClickableObject(inObjectID);
        if (null != clickableObject)
        {
            for (byte locationIndex = 0; locationIndex < LOCATIONNUM_MAX; ++locationIndex)
            {
                if (inObjectID == locatedObjects[locationIndex])
                {
                    // Set VisibleFlag to False
                    byte mask = (byte)(1 << locationIndex);
                    objectClickedFlag &= (byte)~mask;

                    // Clean up Location : ClickableID info
                    locatedObjects[locationIndex] = ClickableID.CID_None;

                    // move to Container
                    clickableObject.transform.SetParent(clickablesContainer, false);

                    break;
                }
            }
        }
    }

    public void ShowAllObjects()
    {
        float fadeDuration = 0.5f;
        // OR fade in ALL ClickableObjects
        for (byte locationIndex = 0; locationIndex < LOCATIONNUM_MAX; ++locationIndex)
        {
            ClickableID objectID = locatedObjects[locationIndex];
            GameObject clickableObject = GetClickableObject(objectID);

            if ((null != clickableObject) &&
                (null != clickableLocations[locationIndex]))
            {
                // fade in
                LeanTween.alpha(clickableObject, 1f, fadeDuration)
                .setEase(LeanTweenType.easeInQuad/*inEaseType*/)
                .setRecursive(false);
                //.setOnComplete(() => );
            }
        }
    }

    public void HideAllObjects()
    {
        float fadeDuration = 0.5f;

        // OR fade in ALL ClickableObjects
        for (byte locationIndex = 0; locationIndex < LOCATIONNUM_MAX; ++locationIndex)
        {
            ClickableID objectID = locatedObjects[locationIndex];
            GameObject clickableObject = GetClickableObject(objectID);

            if ((null != clickableObject) &&
                (null != clickableLocations[locationIndex]))
            {
                // fade in
                LeanTween.alpha(clickableObject, 0f, fadeDuration)
                .setEase(LeanTweenType.easeInQuad/*inEaseType*/)
                .setRecursive(false);
                //.setOnComplete(() => );
            }
        }
    }

    private GameObject GetClickableObject(ClickableID inObjectID)
    {
        if (ClickableID.CID_None == inObjectID || ClickableID.CID_Max == inObjectID)
            return null;

        foreach (ClickableObject clickableObj in clickableObjects)
        {
            if (inObjectID == clickableObj.ID)
            {
                return clickableObj.obj;
            }
        }

        return null;
    }
}
