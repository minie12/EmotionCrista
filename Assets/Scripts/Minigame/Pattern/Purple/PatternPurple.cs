using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PatternPurple : PatternManager
{
    // around gems direction vector (odd/even standard: col)
    private readonly int[,] aroundGem_o = new int[6, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 } };
    private readonly int[,] aroundGem_e = new int[6, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

    // manage chain gimmick
    private int chainCnt = 1;
    private float interval = 30f;

    // related flashlight gimmick
    private GameObject globalLightObj; // get global light object
    private GameObject lightPrefab;
    private GameObject eyePrefab;
    private GameObject flashLight;
    private readonly Dictionary<string, GameObject> eyeObjs = new Dictionary<string, GameObject>(); // key: eye id, value: eye obj
    private readonly Dictionary<string, List<int>> eyeObjsArea = new Dictionary<string, List<int>>(); // key: eye id, value: eye area idx list

    private readonly float eyeFirstTime = 0.5f;
    private readonly float[] eyeAreaX = { 0f, 2f, 4f, 6f };
    private readonly float[] eyeAreaY = { -3f, -1f, 1f, 3f };
    private bool[,] eyeAreaCheck = new bool[3, 3];
    private readonly float eyeScale = 0.2f;


    protected override void Awake()
    {
        base.Awake();
        globalLightObj = GameObject.Find("GlobalLight");
        lightPrefab = Resources.Load<GameObject>("Prefabs/MiniGame/flashLight");
        eyePrefab = Resources.Load<GameObject>("Prefabs/MiniGame/eye");
    }

    public override void OnCrushedGem(bool isMatchColor, List<List<int>> crushedGems)
    {
        base.OnCrushedGem(isMatchColor, crushedGems);

        // purple gimmick
        if (gimmick[0])
        {
            CheckAfterCrush(crushedGems);
        }
    }

    public override void StartPattern(int level_)
    {
        base.StartPattern(level_);

        RestartPattern();
    }

    public override void RestartPattern()
    {
        base.RestartPattern();
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
                ResetAllChainGem();
                CancelInvoke(nameof(PurpleGimmick0));
                break;
            case 1:
                Destroy(flashLight);
                globalLightObj.GetComponent<Light2D>().intensity = 1f;

                // all eye obj delete
                foreach (KeyValuePair<string, GameObject> entry in eyeObjs)
                {
                    Destroy(entry.Value);
                }
                eyeObjs.Clear();
                eyeObjsArea.Clear();
                eyeAreaCheck = new bool[3, 3];
                break;
        }
    }

    // 존재하는 모든 사슬 초기화
    void ResetAllChainGem()
    {
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (j == 5 && i % 2 == 0)
                {
                    continue;
                }

                GemInfo gem = board.GetGem(i, j);
                if (gem == null)
                {
                    continue;
                }

                gem.bLocationFixed = false;
                gem.SetChainZero();

                // 외곽선 넘어가기 
                if (i == 0 || i == 10 || j == 0 || j == 5 || (j == 4 && i % 2 == 0))
                {
                    continue;
                }

                 board.SetRotate(i, j, false);
            }
        }
    }

    // check exit chain around gem
    List<GemInfo> CheckExitChainAround(List<List<int>> crushedGems)
    {
        List<GemInfo> result = new List<GemInfo>();
        bool[,] check = new bool[11, 6];

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

    public void CheckAfterCrush(List<List<int>> crushedGems)
    {
        List<GemInfo> aroundChainGems = CheckExitChainAround(crushedGems);

        for (int i = 0; i < aroundChainGems.Count; i++)
        {
            Debug.Log("주변 사슬" + aroundChainGems[i].GetColumn() + ", " + aroundChainGems[i].GetRow());
            int extraChain = aroundChainGems[i].MinusChainCnt();

            // end chain
            if (extraChain == 0)
            {
                float fadeTime = 0.5f;
                aroundChainGems[i].FadeOut(fadeTime, 7);
                aroundChainGems[i].bLocationFixed = false;
                int column = aroundChainGems[i].GetColumn();
                int row = aroundChainGems[i].GetRow();
                Debug.Log("사슬 fix 없앰 " + column + ", " + row);

                StartCoroutine(DeleteChain(fadeTime, aroundChainGems[i], column, row));
            }
        }
    }

    IEnumerator DeleteChain(float fadeTime, GemInfo gem, int cur_col, int cur_row)
    {
        yield return new WaitForSeconds(fadeTime); // term fade out 
        gem.chainAnimObj.SetActive(false);

        List<List<int>> aroundGems = new List<List<int>> { new List<int> { cur_col, cur_row } };

        // choose direction vector about even or odd column
        int[,] direction = new int[6, 2];
        if (cur_col % 2 == 0) // even
        {
            direction = aroundGem_e;
        }
        else // odd
        {
            direction = aroundGem_o;
        }

        // 6 direction
        for (int j = 0; j < 6; j++)
        {
            int new_col = cur_col + direction[j, 0];
            int new_row = cur_row + direction[j, 1];

            // outside range
            if (new_col < 0 || new_row < 0 || new_col > 10 || (new_col % 2 == 0 && new_row > 4) || (new_col % 2 == 1 && new_row > 5))
            {
                continue;
            }

            aroundGems.Add(new List<int> { new_col, new_row });
        }

        for (int i = 0; i < aroundGems.Count; i++)
        {
            bool isChain = IsExitChainAround(aroundGems[i][0], aroundGems[i][1]);
            Debug.Log("사슬 해제한 주변 광물 " + aroundGems[i][0] + ", " + aroundGems[i][1] + ", 사슬 유무: " + isChain);
            if (isChain)
            {
                board.SetRotate(aroundGems[i][0], aroundGems[i][1], true);
            }
            else
            {
                board.SetRotate(aroundGems[i][0], aroundGems[i][1], false);
            }
        }
    }

    void BlockAroundGem(GemInfo gem)
    {
        // get around gems
        List<GemInfo> aroundGems = board.GetAroundGems(gem.GetColumn(), gem.GetRow());

        for (int i = 0; i < aroundGems.Count; i++)
        {
            aroundGems[i].bLocationFixed = true;
            aroundGems[i].SetChainGem(chainCnt); // 사슬 초기화
            List<GemInfo> aroundGemsTemp = board.GetAroundGems(aroundGems[i].GetColumn(), aroundGems[i].GetRow());
            for(int j = 0; j < aroundGemsTemp.Count; j++)
            {
                board.SetRotate(aroundGemsTemp[j].GetColumn(), aroundGemsTemp[j].GetRow(), true);
            }
        }
    }

    void BlockInitGem(GemInfo gem)
    {
        // get around gems
        List<GemInfo> aroundGems = board.GetAroundGems(gem.GetColumn(), gem.GetRow());

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

        // 사슬 눈 뜬 후 주변 광물 잠그기
        gem.bLocationFixed = false;
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
    public void DeleteEye(string eyeId)
    {
        List<int> area = eyeObjsArea[eyeId];

        eyeObjs.Remove(eyeId);
        eyeObjsArea.Remove(eyeId);

        Debug.Log("eye area " + area[0] + " , " + area[1] + " delete!");

        eyeAreaCheck[area[0], area[1]] = false;
    }

    private void AddEye()
    {
        // eye id
        string eyeId = System.Guid.NewGuid().ToString();

        GameObject eyeObj = CreateEye(eyeId); // eye object 생성
        eyeObj.GetComponent<PatternEye>().SetEyeId(eyeId); // eyeId 실제 눈 오브젝트에 세팅

        eyeObjs.Add(eyeId, eyeObj);
    }

    public IEnumerator AddEyeAfterTime(float interval)
    {
        yield return new WaitForSeconds(interval);
        Debug.Log(interval + " 초 후 눈 생성 ");

        AddEye();
    }

    public bool ExistEyeId(string eyeId)
    {
        return eyeObjs.ContainsKey(eyeId);

    }

    GameObject CreateEye(string eyeId)
    {
        Debug.Log(eyeId + " 눈 생성 !!");

        GameObject eyeParent = GameObject.Find("EyeParent");
        GameObject eyeObj = Instantiate(eyePrefab, eyeParent.transform);
        eyeObj.GetComponent<Transform>().localScale = new Vector2(eyeScale, eyeScale);

        int randXIdx = Random.Range(0, 3);
        int randYIdx = Random.Range(0, 3);
        while (eyeAreaCheck[randXIdx, randYIdx] == true)
        {
            randXIdx = Random.Range(0, 3);
            randYIdx = Random.Range(0, 3);
        }

        eyeAreaCheck[randXIdx, randYIdx] = true;

        List<int> area = new List<int>
        {
            randXIdx,
            randYIdx
        };
        eyeObjsArea.Add(eyeId, area);

        float randX = Random.Range(eyeAreaX[randXIdx], eyeAreaX[randXIdx + 1]);
        float randY = Random.Range(eyeAreaY[randYIdx], eyeAreaY[randYIdx + 1]);
        eyeObj.GetComponent<Transform>().localPosition = new Vector2(randX, randY);

        return eyeObj;
    }

    void InitEyes()
    {
        int eyeCnt = 3;
        for (int i = 0; i < eyeCnt; i++)
        {
            this.AddEye();
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
