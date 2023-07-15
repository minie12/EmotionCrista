using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    private int[] PosX = { 0, 0, 185, 375, 565, 750, 935 }; // first 0 is None. This is because CharacterIndex starts with 1
    [SerializeField]
    private Sprite [] times;
    [SerializeField]
    private Sprite[] colors;

    private int CharacterIndex;

    private GameObject boxObj, timeObj, colorObj;
    private float twinklingTime = 0.5f;
    private int minTwinkleCnt = 2;

    private bool activeBool = false;

    // Start is called before the first frame update
    void Awake()
    {
        boxObj = GameObject.Find("Box");
        timeObj = GameObject.Find("Time");
        colorObj = GameObject.Find("Color");
    }

    void Start()
    {
        //LoadScene("LabCorridor");
        //StartCoroutine(TwinklingChangeObj(timeObj, times[1]));

        
        //CharacterIndex = flowchart.GetVariable<Fungus.IntegerVariable>("CharacterIndex").Value;

        CharacterIndex = PosX.Length;

        GameObject FlowchartObj = GameObject.Find("Flowchart");
        if (FlowchartObj)
        {
            Fungus.Flowchart flowchart = FlowchartObj.GetComponent<Fungus.Flowchart>();
            string prefsKey = Fungus.SetSaveProfile.SaveProfile + "_" + flowchart.SubstituteVariables("CharacterIndex");
            bool validKey = PlayerPrefs.HasKey(prefsKey);

            if (true == validKey)
            {
                CharacterIndex = PlayerPrefs.GetInt(prefsKey);
            }
        }

        if (CharacterIndex < 1 || PosX.Length <= CharacterIndex)
        {
            Debug.LogError("Loading Scene: Invalid CharacterIndex " + CharacterIndex);
            CharacterIndex = 1;
        }

        boxObj.transform.position = new Vector3(PosX[CharacterIndex-1] * 0.01f, 0, 0);
        StartCoroutine(TwinklingChangeObj(timeObj, times[1]));
    }

    void Blink()
    {
        timeObj.SetActive(activeBool);
        activeBool = !activeBool;
    }

    IEnumerator TwinklingObj(GameObject obj)
    {
        yield return new WaitForSeconds(twinklingTime);
        obj.SetActive(false);
        yield return new WaitForSeconds(twinklingTime);
        obj.SetActive(true);
    }
    IEnumerator TwinklingChangeObj(GameObject obj, Sprite change)
    {
        yield return new WaitForSeconds(twinklingTime);
        obj.SetActive(false);
        yield return new WaitForSeconds(twinklingTime);
        obj.SetActive(true);

        yield return new WaitForSeconds(twinklingTime);
        obj.SetActive(false);

        Vector3 startPos = boxObj.transform.position;
        Vector3 endPos = new Vector3(PosX[CharacterIndex] * 0.01f, 0, 0);

        for (float t = 0; t <= 1 * twinklingTime; t += Time.deltaTime)
        {
            boxObj.transform.position = Vector3.Lerp(startPos, endPos, t / twinklingTime);
            yield return 0;
        }
        boxObj.transform.position = endPos;

        obj.GetComponent<SpriteRenderer>().sprite = change;
        obj.SetActive(true);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("LabCorridor");
    }

}
