using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PatternPurple : PatternManager
{
    // manage chain gimmick
    private int chainCnt = 1;
    private float interval = 30f;

    // related flashlight gimmick
    private GameObject globalLightObj; // get global light object
    private GameObject lightPrefab;
    private GameObject eyePrefab;
    private GameObject flashLight;
    private readonly List<GameObject> eyeObjs = new List<GameObject>();
    private readonly List<List<int>> eyeObjsErea = new List<List<int>>();

    private readonly float eyeFirstTime = 0.5f;
    private readonly float[] eyeEreaX = { 0f, 2f, 4f, 6f };
    private readonly float[] eyeEreaY = { -3f, -1f, 1f, 3f };
    private int[,] eyeEreaCheck = new int[3, 3];
    private readonly float eyeScale = 0.2f;


    protected override void Awake()
    {
        base.Awake();
        globalLightObj = GameObject.Find("GlobalLight");
        lightPrefab = Resources.Load<GameObject>("Prefabs/MiniGame/flashLight");
        eyePrefab = Resources.Load<GameObject>("Prefabs/MiniGame/eye");
    }

    public override void OnCrushedGem(bool isMatchColor)
    {
        base.OnCrushedGem(isMatchColor);

        // purple gimmick
        if (gimmick[0])
        {
            CheckAfterCrush();
        }
    }

    public override void StartPattern(int level_)
    {
        base.StartPattern(level_);

        // [TODO] 기획
        switch (level_)
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
    }

    public override void StartGimmick(int gimmick_)
    {
        base.StartGimmick(gimmick_);

        switch (gimmick_)
        {
            case 0:
                switch (mini.patternLevel)
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
            case 1:
                PurpleGimmick1();
                break;
        }
    }

    public override void StopGimmick(int gimmick_)
    {
        base.StopGimmick(gimmick_);

        switch (gimmick_)
        {
            case 0:
                CancelInvoke(nameof(PurpleGimmick0));
                break;
            case 1:
                Destroy(flashLight);
                for (int i = 0; i < eyeObjs.Count; i++)
                {
                    Destroy(eyeObjs[0]);
                    DeleteEye(0);
                }
                break;
        }
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

    // =============== gimmick 1 (flash light) ================== //
    public void DeleteEye(int index)
    {
        List<int> erea = eyeObjsErea[index];

        eyeObjs.RemoveAt(index);
        eyeObjsErea.RemoveAt(index);

        eyeEreaCheck[erea[0], erea[1]] = 0;
    }

    public void AddEye()
    {
        GameObject eye = CreateEye();
        eyeObjs.Add(eye);
    }

    public void UpdateEye(int index)
    {
        GameObject eye = CreateEye();
        eyeObjs[index] = eye;
    }

    public bool IsMatchGimmick(int index, GameObject obj)
    {
        if(index >= eyeObjs.Count)
        {
            return false;
        }
        return eyeObjs[index] == obj;
    }


    GameObject CreateEye()
    {
        GameObject eyeParent = GameObject.Find("EyeParent");
        GameObject eyeObj = Instantiate(eyePrefab, eyeParent.transform);
        eyeObj.GetComponent<Transform>().localScale = new Vector2(eyeScale, eyeScale);

        int randXIdx = Random.Range(0, 3);
        int randYIdx = Random.Range(0, 3);
        while (eyeEreaCheck[randXIdx, randYIdx] != 0)
        {
            randXIdx = Random.Range(0, 3);
            randYIdx = Random.Range(0, 3);
        }

        eyeEreaCheck[randXIdx, randYIdx] = eyeObjs.Count + 1;

        List<int> erea = new List<int>
        {
            randXIdx,
            randYIdx
        };
        eyeObjsErea.Add(erea);

        float randX = Random.Range(eyeEreaX[randXIdx], eyeEreaX[randXIdx + 1]);
        float randY = Random.Range(eyeEreaY[randYIdx], eyeEreaY[randYIdx + 1]);
        eyeObj.GetComponent<Transform>().localPosition = new Vector2(randX, randY);

        return eyeObj;
    }

    void InitEyes()
    {
        int eyeCnt = 3;
        for (int i = 0; i < eyeCnt; i++)
        {
            AddEye();
        }
    }

    void PurpleGimmick1()
    {
        // set light
        globalLightObj.GetComponent<Light2D>().intensity = 0.2f;
        flashLight = Instantiate(lightPrefab, UICanvas.transform);

        // create init eyes
        Invoke(nameof(InitEyes), eyeFirstTime);
    }
}
