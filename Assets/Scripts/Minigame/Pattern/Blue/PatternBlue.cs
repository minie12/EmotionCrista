using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternBlue : PatternManager
{
    // water fill
    private List<int> clearGaugeSet = new List<int>() { 10, 30, 50, 70, 80, 90, 95 };
    private int lastClearGaugeIdx = 0;

    // bubble
    private GameObject bubblePF;
    private readonly float totalTime = 5f;
    private int crushedBiasCnt = 10;
    private int crushedGemLast = 0;
    private bool bubbleShowing = false;
    private int bubbleCnt = 0;
    private IEnumerator bubbleCoroutine;

    protected override void Awake()
    {
        base.Awake();
        bubblePF = Resources.Load<GameObject>("Prefabs/MiniGame/bubble");
    }

    public override void OnCrushedGem(bool isMatchColor)
    {
        base.OnCrushedGem(isMatchColor);

        // water fill
        if (gimmick[0])
        {
            int clearGauge = mini.GetClearGauge();
            if (clearGauge >= clearGaugeSet[lastClearGaugeIdx])
            {
                lastClearGaugeIdx += 1;
                B_StartWaterFill();
            }
        }

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
        lastClearGaugeIdx = 0;

        bubbleShowing = false;
        crushedGemLast = 0;

        Invoke(nameof(B_CryGem), 1f);

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
                //
                break;
            case 1:
                if(bubbleCoroutine != null) StopCoroutine(bubbleCoroutine);
                break;
        }
    }
    public override void RestartPattern()
    {
        base.RestartPattern();

        // [TODO] 기획
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
                if (rand <= 0.8f) waterGemCnt = 2;
                else waterGemCnt = 3;
                break;
            case 2:
                if (rand <= 0.35f) waterGemCnt = 2;
                else waterGemCnt = 3;
                break;
            case 3:
                if (rand <= 0.45f) waterGemCnt = 1;
                else if (rand <= 0.8f) waterGemCnt = 2;
                else if (rand <= 0.95f) waterGemCnt = 3;
                else waterGemCnt = 4;
                break;
            case 4:
                if (rand <= 0.5f) waterGemCnt = 2;
                else if (rand <= 0.7f) waterGemCnt = 3;
                else waterGemCnt = 4;
                break;
            case 5:
                if (rand <= 0.3f) waterGemCnt = 2;
                else if (rand <= 0.5f) waterGemCnt = 3;
                else waterGemCnt = 4;
                break;
        }

        List<GemInfo> gems = board.GetPatternGemManyRandom(waterGemCnt);
        for (int i = 0; i < gems.Count; i++)
        {
            if(gems[i] == null)
            {
                continue;
            }
            gems[i].FillWaterInHex();
        }
    }

    // B1 -----------------------------------------------
    void B_StartBubble()
    {
        // create object
        GameObject bubble = Instantiate(bubblePF, new Vector3(0f, 0f, 0f), Quaternion.identity, UICanvas.transform);
        bubble.SetActive(false);

        // set position & size
        Vector3 newPos = new Vector3(Random.Range(-220f, 800f), -550f, 0f);
        Vector3 originSize = bubble.GetComponent<RectTransform>().localScale;
        float size = Random.Range(0.5f, 1.5f);

        bubble.GetComponent<RectTransform>().anchoredPosition = newPos;
        bubble.GetComponent<RectTransform>().localScale  = new Vector3(originSize.x * size, originSize.y * size, 1);
        bubble.SetActive(true);
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
        bubbleCnt = Random.Range(2, 10);
        bubbleCoroutine = CreateBubble(bubbleCnt);
        StartCoroutine(bubbleCoroutine);
    }

    // B2 -----------------------------------------------
    void B_CryGem()
    {
        // 맨 윗 줄 (row: 4)에서 랜덤으로 광물 얻어오기
        List<GemInfo> upperGems = board.GetGemRows(new List<int> { 4 });
        GemInfo gem = upperGems[Random.Range(0, upperGems.Count)];

        // 눈물 광물로 변경
        gem.ChangeGemColor(1);
        gem.ChangeSpecialGem();
        gem.FadeIn(0.5f);
        gem.isCryGem = true;

        StartCoroutine(GemCrying(gem));
    }

    IEnumerator GemCrying(GemInfo gem)
    {
        yield return new WaitForSeconds(1f);
        float dropTime = 0.3f;

        while (true)
        {
            int curr_col = gem.GetColumn();
            int curr_row = gem.GetRow();
            int next_row = curr_row - 1;

            GemInfo next_gem = board.GetGem(curr_col, next_row);

            if (next_gem == null)
            {
                gem.DestroyGem();
                board.StartRefilBoardFever();
                break;
            }

            next_gem.MoveGem(curr_col, curr_row, dropTime);
            gem.FadeOut(0.3f);
            next_gem.FadeOut(0.3f);
            gem.MoveGem(curr_col, next_row, dropTime);
            board.SetGem(curr_col, next_row, gem);
            board.SetGem(curr_col, curr_row, next_gem);
            yield return new WaitForSeconds(0.3f);
            gem.FadeIn(0.3f);
            next_gem.FadeIn(0.3f);

            yield return new WaitForSeconds(2f);
        }
        yield return null;
    }
}
