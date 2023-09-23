using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternBlue : PatternManager
{
    private float waterFillTime = 3.8f;
    private float bubbleTime = 1.8f;

    // bubble
    private GameObject bubble_PF;

    protected override void Awake()
    {
        base.Awake();
        bubble_PF = Resources.Load<GameObject>("Prefabs/MiniGame/bubble");
    }

    public override void StartPattern(int level_)
    {
        base.StartPattern(level_);

        mini.patternGimmick = new bool[2];
        gimmick = new bool[2];
        level = level_;
        OrganizeCharacterChat();

        // [TODO] ±‚»π
        switch (level)
        {
            case 0:
                StartGimmick(0);
                break;
            case 1:
                StartGimmick(1);
                break;
            case 2:
                StartGimmick(0);
                break;
            case 3:
                StartGimmick(1);
                break;
            case 4:
                StartGimmick(0);
                break;
            case 5:
                StartGimmick(1);
                break;
        }
    }

    public override void StopPattern()
    {
        base.StopPattern();
        CancelInvoke();
        for (int i = 0; i < gimmick.GetLength(0); i++)
        {
            gimmick[i] = false;
            mini.patternGimmick[i] = false;
        }
    }

    public override void StartGimmick(int gimmick_)
    {
        base.StartGimmick(gimmick_);
        mini.patternGimmick[gimmick_] = true;
        gimmick[gimmick_] = true;

        switch (gimmick_)
        {
            case 0:
                InvokeRepeating("B_StartWaterFill", 0.4f, waterFillTime);
                break;
            case 1:
                InvokeRepeating("B_StartBubble", 0.4f, bubbleTime);
                break;
        }
    }

    public override void StopGimmick(int gimmick_)
    {
        base.StopGimmick(gimmick_);
        mini.patternGimmick[gimmick_] = false;
        gimmick[gimmick_] = false;

        switch (gimmick_)
        {
            case 0:
                CancelInvoke("B_StartWaterFill");
                break;
            case 1:
                CancelInvoke("B_StartBubble");
                break;
        }
    }

    public override void RestartPattern()
    {
        base.RestartPattern();
        StartPattern(level);
    }

    // B1 -----------------------------------------------
    void B_StartWaterFill(){
        int waterGemCnt = 1;
        float rand = Random.value;
        switch (level)
        {
            case 0:
                if (rand <= 0.6f) waterGemCnt = 1;
                else waterGemCnt = 2;
                break;
            case 1:
                if (rand <= 0.5f) waterGemCnt = 1;
                else if (rand <= 0.8f) waterGemCnt = 2;
                else waterGemCnt = 3;
                break;
            case 2:
                if (rand <= 0.45f) waterGemCnt = 1;
                else if (rand <= 0.35f) waterGemCnt = 2;
                else waterGemCnt = 3;
                break;
            case 3:
                if (rand <= 0.45f) waterGemCnt = 1;
                else if (rand <= 0.8f) waterGemCnt = 2;
                else if (rand <= 0.95f) waterGemCnt = 3;
                else waterGemCnt = 4;
                break;
            case 4:
                if (rand <= 0.5f) waterGemCnt = 1;
                else if (rand <= 0.70f) waterGemCnt = 2;
                else if (rand <= 0.83f) waterGemCnt = 3;
                else if (rand <= 0.95f) waterGemCnt = 4;
                else waterGemCnt = 5;
                break;
        }

        GemInfo[] gems = board.GetRandomGems(waterGemCnt);
        for (int i = 0; i < waterGemCnt; i++)
        {
            gems[i].FillWaterInHex();
        }
    }

    void B_StartBubble()
    {
        // create object
        GameObject temp = Instantiate(bubble_PF, new Vector3(0, 0, 0), Quaternion.identity);
        temp.SetActive(false);

        // set position
        Vector3 rand_pos = new Vector3(Random.Range(800.0f, 1600.0f), 0f, 5);

        // location object in screen
        temp.transform.position = Camera.main.ScreenToWorldPoint(rand_pos);
        float size = Random.Range(0.45f, 1.2f);
        temp.GetComponent<RectTransform>().localScale = new Vector3(size, size, 1);
        temp.SetActive(true);
    }
}
