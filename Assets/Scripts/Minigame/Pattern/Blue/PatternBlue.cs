using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternBlue : PatternManager
{
    private float waterFillTime = 3.8f;
    private float bubbleTime = 1.8f;
    private string day_and_level = "2hard"; // 1easy: 1, 1normal: 2, 1hard: 3, 2easy: 3, 2normal: 4, 2hard: 5

    // bubble
    private GameObject bubble_PF;

    protected override void Awake()
    {
        base.Awake();
        bubble_PF = Resources.Load<GameObject>("Prefabs/MiniGame/bubble");
        Debug.Log("Awake");
    }

    override public void StartPattern(int gimmick_){
        gimmick = gimmick_;
        OrganizeCharacterChat();

        if(gimmick == 0) InvokeRepeating("B_StartWaterFill", 0.4f, waterFillTime);
        if(gimmick == 1)
        {
            InvokeRepeating("B_StartBubble", 0.4f, bubbleTime);
        }
    }

    override public void StopPattern(){ CancelInvoke(); }
    override public void RestartPattern(){
        if (gimmick == 0) InvokeRepeating("B_StartWaterFill", 0.4f, waterFillTime);
        if (gimmick == 1)
        {
            InvokeRepeating("B_StartBubble", 0.4f, bubbleTime);
        }
    }

    // B1 -----------------------------------------------
    void B_StartWaterFill(){
        int waterGemCnt = 1;
        float rand = Random.value;
        switch (day_and_level)
        {
            case "1normal":
                if (rand <= 0.6f) waterGemCnt = 1;
                else waterGemCnt = 2;
                break;
            case "1hard":
                if (rand <= 0.5f) waterGemCnt = 1;
                else if (rand <= 0.8f) waterGemCnt = 2;
                else waterGemCnt = 3;
                break;
            case "2easy":
                if (rand <= 0.45f) waterGemCnt = 1;
                else if (rand <= 0.35f) waterGemCnt = 2;
                else waterGemCnt = 3;
                break;
            case "2normal":
                if (rand <= 0.45f) waterGemCnt = 1;
                else if (rand <= 0.8f) waterGemCnt = 2;
                else if (rand <= 0.95f) waterGemCnt = 3;
                else waterGemCnt = 4;
                break;
            case "2hard":
                if (rand <= 0.5f) waterGemCnt = 1;
                else if (rand <= 0.70f) waterGemCnt = 2;
                else if (rand <= 0.83f) waterGemCnt = 3;
                else if (rand <= 0.95f) waterGemCnt = 4;
                else waterGemCnt = 5;
                break;
        }

        GemInfo[] gems = mini.GetRandomGems(waterGemCnt);
        for (int i = 0; i < waterGemCnt; i++)
        {
            gems[i].FillWaterInHex();
        }
    }


    void B_StartBubble()
    {
        // create object
        GameObject temp = Instantiate(bubble_PF, new Vector3(0, 0, 0), Quaternion.identity, UICanvas.transform);
        temp.SetActive(false);

        // set position
        Vector3 rand_pos = new Vector3(Random.Range(0f, 1920.0f), 0f, 5);

        // location object in screen
        temp.transform.position = Camera.main.ScreenToWorldPoint(rand_pos);
        float size = Random.Range(0.45f, 1.2f);
        temp.GetComponent<RectTransform>().localScale = new Vector3(size, size, 1);
        temp.SetActive(true);
    }
}
