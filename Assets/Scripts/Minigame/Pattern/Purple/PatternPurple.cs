using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternPurple : PatternManager
{
    private int chainCnt = 1;
    private float interval = 30f;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void StartPattern(int level_)
    {
        base.StartPattern(level_);
        level = level_;
        gimmick = new bool[1];
        mini.patternGimmick = new bool[1];
        OrganizeCharacterChat();

        // [TODO] 기획
        switch (level)
        {
            case 0:
                StartGimmick(0);
                break;
            case 1:
                StartGimmick(0);
                break;
            case 2:
                StartGimmick(0);
                break;
            case 3:
                StartGimmick(0);
                break;
            case 4:
                StartGimmick(0);
                break;
            case 5:
                StartGimmick(0);
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
        gimmick[gimmick_] = true;
        mini.patternGimmick[gimmick_] = true;

        switch (gimmick_)
        {
            case 0:
                switch (level)
                {
                    case 0:
                        chainCnt = 1;
                        interval = 120f;
                        break;
                    case 1:
                        chainCnt = 2;
                        interval = 100f;
                        break;
                    default:
                        chainCnt = 3;
                        interval = 120f;
                        break;
                }
                InvokeRepeating(nameof(PurpleGimmick0), 1f, interval);
                break;
        }
    }

    public override void StopGimmick(int gimmick_)
    {
        base.StopGimmick(gimmick_);
        gimmick[gimmick_] = false;
        mini.patternGimmick[gimmick_] = false;

        switch (gimmick_)
        {
            case 0:
                CancelInvoke(nameof(PurpleGimmick0));
                break;
        }
    }

    public override void RestartPattern()
    {
        base.RestartPattern();
        StartPattern(level);
    }

    // check exit chain around gem
    List<GemInfo> CheckExitChainAround()
    {
        List<GemInfo> result = new List<GemInfo>();
        bool[,] check = new bool[11, 6];

        List<List<int>> crushedGems = GameObject.Find("Board").GetComponent<GoalInfo>().crushedGems;

        for (int i = 0; i < crushedGems.Count; i++)
        {
            List<GemInfo> aroundGems = board.GetAroundGems(crushedGems[i][0], crushedGems[i][1]);

            for (int j = 0; j < aroundGems.Count; j++)
            {
                // exit chain
                int column_ = aroundGems[j].GetColumn();
                int row_ = aroundGems[j].GetRow();
                if (aroundGems[j].GetChainCnt() > 0 && !check[column_, row_])
                {
                    result.Add(aroundGems[j]);
                    check[column_, row_] = true;
                }
            }
        }
        return result;
    }

    bool IsExitChainAround(int col, int r)
    {
        List<GemInfo> aroundGems = board.GetAroundGems(col, r);
        aroundGems.Add(board.GetGem(col, r));

        for (int j = 0; j < aroundGems.Count; j++)
        {
            // exit chain
            if (aroundGems[j].GetChainCnt() > 0)
            {
                return true;
            }
        }
        return false;
    }

    public void CheckAfterCrush()
    {
        List<GemInfo> aroundChainGems = CheckExitChainAround();

        for (int i = 0; i < aroundChainGems.Count; i++)
        {
            Debug.Log("주변 사슬" + aroundChainGems[i].GetColumn() + ", " + aroundChainGems[i].GetRow());
            int extraChain = aroundChainGems[i].MinusChainCnt();

            // end chain
            if (extraChain == 0)
            {
                float fadeTime = 0.5f;
                aroundChainGems[i].FadeOut(fadeTime, 5);
                StartCoroutine(DeleteChain(fadeTime, aroundChainGems[i]));
            }

        }
    }

    IEnumerator DeleteChain(float fadeTime, GemInfo gem)
    {
        yield return new WaitForSeconds(fadeTime); // term fade out 

        gem.chainAnimObj.SetActive(false);
        gem.bLocationFixed = false;


        List<GemInfo> aroundGems = board.GetAroundGems(gem.GetColumn(), gem.GetRow());
        aroundGems.Add(gem);

        for (int i = 0; i < aroundGems.Count; i++)
        {
            bool isChain = IsExitChainAround(aroundGems[i].GetColumn(), aroundGems[i].GetRow());
            Debug.Log("사슬 해제한 주변 광물 " + aroundGems[i].GetColumn() + ", " + aroundGems[i].GetRow() + ", 사슬 유무: " + isChain);
            if (isChain)
            {
                board.SetRotate(aroundGems[i].GetColumn(), aroundGems[i].GetRow(), true);
            }
            else
            {
                board.SetRotate(aroundGems[i].GetColumn(), aroundGems[i].GetRow(), false);
            }
        }

    }

    void BlockAroundGem(GemInfo gem)
    {
        // get around gems
        List<GemInfo> aroundGems = GameObject.Find("Board").GetComponent<BoardManager>().GetAroundGems(gem.GetColumn(), gem.GetRow());

        for (int i = 0; i < aroundGems.Count; i++)
        {
            aroundGems[i].bLocationFixed = true;
            aroundGems[i].SetChainGem(chainCnt);
            List<GemInfo> aroundGemsTemp = GameObject.Find("Board").GetComponent<BoardManager>().GetAroundGems(aroundGems[i].GetColumn(), aroundGems[i].GetRow());
            for(int j = 0; j < aroundGemsTemp.Count; j++)
            {
                board.SetRotate(aroundGemsTemp[j].GetColumn(), aroundGemsTemp[j].GetRow(), true);
            }
        }
    }

    void BlockInitGem(GemInfo gem)
    {
        // get around gems
        List<GemInfo> aroundGems = GameObject.Find("Board").GetComponent<BoardManager>().GetAroundGems(gem.GetColumn(), gem.GetRow());

        // block rotate gems
        board.SetRotate(gem.GetColumn(), gem.GetRow(), true);
        // fix location gems
        gem.bLocationFixed = true;
        for (int i = 0; i < aroundGems.Count; i++)
        {
            board.SetRotate(aroundGems[i].GetColumn(), aroundGems[i].GetRow(), true);
        }
    }

    IEnumerator TwinkleEyes(GemInfo gem, int cnt, float fadeTime)
    {
        // term fade in of purple special gem init
        yield return new WaitForSeconds(1f);

        while (cnt-- > 0)
        {
            gem.FadeOut(fadeTime);
            yield return new WaitForSeconds(fadeTime);
            gem.FadeIn(fadeTime);
            yield return new WaitForSeconds(fadeTime);
        }
        gem.ChangeGemColor(mini.patternIdx);
        gem.FadeIn();
        BlockAroundGem(gem);
    }


    void PurpleGimmick0()
    {
        // get random purple gem
        GemInfo purpleGem = board.GetPatternGemRandom();
        purpleGem.ChangeSpecialGem();
        purpleGem.FadeIn();
        BlockInitGem(purpleGem);

        // twinkle purple gem & block around gems
        StartCoroutine(TwinkleEyes(purpleGem, 2, 0.5f));
    }
}
