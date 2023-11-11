using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternBlue : PatternManager
{
    private float waterFillTime = 3.8f;
    private float bubbleTime = 1.8f;

    // bubble
    private GameObject bubble_PF;
    private readonly float totalTime = 5f;
    private int crushedBiasCnt = 15;
    private int crushedGemLast = 0;
    private bool bubbleShowing = false;

    protected override void Awake()
    {
        base.Awake();
        bubble_PF = Resources.Load<GameObject>("Prefabs/MiniGame/bubble");
    }

    public override void OnCrushedGem(bool isMatchColor)
    {
        base.OnCrushedGem(isMatchColor);

        // buble
        if (gimmick[1])
        {
            Debug.Log("crushed gem cnt" + mini.GetTotalCrushedGem());
            int cnt = mini.GetTotalCrushedGem() / crushedBiasCnt;
            Debug.Log("bubble gimmick" + cnt);
            if(!bubbleShowing && cnt > crushedGemLast)
            {
                Debug.Log("bubble ");
                BlueGimmick1();
                bubbleShowing = true;
                crushedGemLast = cnt;
                StartCoroutine(BubbleTimer());
            }
        }
    }

    public override void StartPattern(int level_)
    {
        base.StartPattern(level_);
        crushedGemLast = 0;

        RestartPattern();
    }

    public override void StopPattern()
    {
        base.StopPattern();
    }

    public override void StartGimmick(int gimmick_)
    {
        base.StartGimmick(gimmick_);

        switch (gimmick_)
        {
            case 0:
                break;
            case 1:
                break;
        }
    }

    public override void StopGimmick(int gimmick_)
    {
        base.StopGimmick(gimmick_);

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

        // [TODO] ±‚»π
        switch (mini.patternLevel)
        {
            case 0:
                StartGimmick(0);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 2.8f, 3);
                break;
            case 1:
                StartGimmick(1);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 2.8f, 3);
                break;
            case 2:
                StartGimmick(0);
                StartGimmick(1);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 2.8f, 3);
                break;
            case 3:
                StartGimmick(1);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 2.8f, 3);
                break;
            case 4:
                StartGimmick(0);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 2.8f, 3);
                break;
            case 5:
                StartGimmick(0);
                StartGimmick(1);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 2.8f, 3);
                break;
        }
    }

    // B0 -----------------------------------------------
    void B_StartWaterFill(){
        int waterGemCnt = 1;
        float rand = Random.value;
        switch (mini.patternLevel)
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

    // B1 -----------------------------------------------
    void B_StartBubble()
    {
        // create object
        GameObject temp = Instantiate(bubble_PF, new Vector3(0, 0, 0), Quaternion.identity, UICanvas.transform);
        temp.SetActive(false);

        // set position
        Vector2 rand_pos = new Vector3(Random.Range(800.0f, 1600.0f), 0f);

        // location object in screen
        temp.transform.position = Camera.main.ScreenToWorldPoint(rand_pos);
        float size = Random.Range(0.45f, 1.2f);
        temp.GetComponent<RectTransform>().localScale = new Vector3(size, size, 1);
        temp.SetActive(true);
        Debug.Log(temp);
    }

    private IEnumerator CreateBubble(int bubbleCnt)
    {
        int cnt = 0;
        while(cnt < bubbleCnt)
        {
            B_StartBubble();
            float time = Random.Range(0.5f, totalTime / bubbleCnt);
            cnt++;
            yield return new WaitForSeconds(time);
        }
    }

    private IEnumerator BubbleTimer()
    {
        float time = 0f;
        while(time <= totalTime)
        {
            time += Time.deltaTime;
        }
        bubbleShowing = false;
        yield return null;
    }

    private void BlueGimmick1()
    {
        int bubbleCnt = Random.Range(2, 10);
        StartCoroutine(CreateBubble(bubbleCnt));
    }
}
