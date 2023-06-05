using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternPurple : PatternManager
{
    private int chainCnt = 3;
    private float interval = 30f;

    private bool isPlaying = false;

    protected override void Awake()
    {
        base.Awake();
    }

    override public void StartPattern(int gimmick_, int level_)
    {
        gimmick = gimmick_;
        level = level_;
        isPlaying = true;
        OrganizeCharacterChat();

        // give term before choose gem because board init
        if (gimmick == 0)
        {
            InvokeRepeating("PurpleGimmick0", 1f, interval);
        }
    }

    override public void StopPattern() 
    {
        isPlaying = false;
        CancelInvoke(); 
    }

    override public void RestartPattern()
    {
        isPlaying = true;
        if (gimmick == 0)
        {
            InvokeRepeating("PurpleGimmick0", 1f, interval);
        }

    }

    public bool GetIsPlaying()
    {
        return isPlaying;
    }


    void BlockAroundGem(GemInfo gem)
    {
        // get around gems
        List<GemInfo> aroundGems = GameObject.Find("Board").GetComponent<BoardManager>().GetAroundGems(gem.GetColumn(), gem.GetRow());

        // block rotate gems
        gem.bRotateAble = false;
        // fix location gems
        gem.bLocationFixed = true;
        for (int i = 0; i < aroundGems.Count; i++)
        {
            aroundGems[i].bRotateAble = false;
            aroundGems[i].bLocationFixed = true;
            aroundGems[i].SetChainGem(chainCnt);
            List<GemInfo> aroundGemsTemp = GameObject.Find("Board").GetComponent<BoardManager>().GetAroundGems(aroundGems[i].GetColumn(), aroundGems[i].GetRow());
            for(int j = 0; j < aroundGemsTemp.Count; j++)
            {
                aroundGemsTemp[j].bRotateAble = false;
            }
        }
    }

    IEnumerator TwinkleEyes(GemInfo gem, int cnt, float fadeTime)
    {
        // term fade in of purple special gem init
        yield return new WaitForSeconds(1f);

        while (cnt-- > 0)
        {
            gem.FadeOut(1f / fadeTime);
            yield return new WaitForSeconds(fadeTime);
            gem.FadeIn(1f / fadeTime);
            yield return new WaitForSeconds(fadeTime);
        }
        gem.ChangeGemColor((int)PatternType.PURPLE);
        gem.FadeIn();
        BlockAroundGem(gem);
    }


    void PurpleGimmick0()
    {
        // get random purple gem
        GemInfo purpleGem = board.GetPatternGemRandom();
        purpleGem.ChangeSpecialGem();
        purpleGem.FadeIn();

        // twinkle purple gem & block around gems
        StartCoroutine(TwinkleEyes(purpleGem, 2, 0.5f));
    }
}
