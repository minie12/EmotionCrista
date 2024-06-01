using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternGreen : PatternManager
{
    private GameObject gemPF, bugPF;
    private bool[,] check = new bool[11, 6];

    private float bugSpeed = 7f;
    private float rotateSpeed = 0.15f;
    private float bugInterval = 60f;

    private int[,] aroundGem_o = new int[6, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 } };
    private int[,] aroundGem_e = new int[6, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

    private bool[,] area = new bool[11, 6];

    private Dictionary<GemInfo, List<GameObject>> bugs = new Dictionary<GemInfo, List<GameObject>>();

    private int crushedBiasCnt = 10;
    private int crushedGemLast = 0;

    protected override void Awake()
    {
        base.Awake();
        gemPF = Resources.Load<GameObject>("Prefabs/MiniGame/org_gem");
        bugPF = Resources.Load<GameObject>("Prefabs/MiniGame/bug");
    }

    public override void OnCrushedGem(bool isMatchColor)
    {
        base.OnCrushedGem(isMatchColor);

        // area
        if (gimmick[1])
        {
            int cnt = mini.GetTotalCrushedGem() / crushedBiasCnt;
            if (cnt > crushedGemLast)
            {
                Debug.Log("set area !");
                crushedGemLast = cnt;
                StartCoroutine(BugDisappear());
            }
        }
    }

    public override void StartPattern(int level_)
    {
        base.StartPattern(level_);

        // init values
        check = new bool[11, 6];
        area = new bool[11, 6];

        RestartPattern();
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
                StartGimmick(0);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 2.8f, 3);
                break;
            case 4:
                StartGimmick(1);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 2.8f, 3);
                break;
            case 5:
                StartGimmick(0);
                StartGimmick(1);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 2.8f, 3);
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
                InvokeRepeating("GreenGimmick0", 1f, bugInterval);
                break;
            case 1:
                Invoke(nameof(SetAreaAgain), 1f);
                break;
        }
    }

    public override void StopGimmick(int gimmick_)
    {
        base.StopGimmick(gimmick_);

        switch (gimmick_)
        {
            case 0:
                CancelInvoke("GreenGimmick0");
                break;
            case 1:
                CancelInvoke("GreenGimmick1");
                break;
        }
    }

    GemInfo GetNotGreenGemRandom(int standard_c, int standard_r)
    {
        while (true)
        {
            GemInfo randGem = board.GetRandomGemOnWay(standard_c, standard_r);
            int column_ = randGem.GetColumn();
            int row_ = randGem.GetRow();
            if (check[column_, row_])
            {
                continue;
            }
            if (randGem.GetColor() != (int)PatternType.GREEN)
            {
                check[column_, row_] = true;
                return randGem;
            }
        }
    }

    List<GemInfo> GetHistoryGemRandom(int cnt, int start_c, int start_r)
    {
        List<GemInfo> result = new List<GemInfo>();
        int prev_c = start_c, prev_r = start_r;
        for (int i = 0; i < cnt;)
        {
            GemInfo randGem = board.GetRandomGemOnWay(prev_c, prev_r);
            int new_c = randGem.GetColumn();
            int new_r = randGem.GetRow();

            if (check[new_c, new_r])
            {
                continue;
            }
            result.Add(randGem);
            check[new_c, new_r] = true;
            prev_c = new_c; prev_r = new_r;
            i++;
        }
        return result;
    }

    GameObject CreateBug()
    {
        // get green gem random
        GemInfo greenGem = board.GetPatternGemRandom();
        int prevColumn = greenGem.GetColumn();
        int prevRow = greenGem.GetRow();
        check[prevColumn, prevRow] = true;
       
        greenGem.FadeOut(0.5f);

        // just create image
        GameObject specialGem = Instantiate(gemPF, greenGem.GetComponent<Transform>().position, Quaternion.identity, UICanvas.transform);
        specialGem.transform.localScale = new Vector3(100f, 100f, 1f);
        specialGem.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 3;
        GemInfo specialGemInfo = specialGem.GetComponent<GemInfo>();
        specialGemInfo.SetBackgroundColor(255f, 255f, 255f, 0f);
        specialGemInfo.InitGem(prevColumn, prevRow, (int)PatternType.GREEN);
        specialGemInfo.ChangeSpecialGem();

        specialGemInfo.FadeIn(0.5f);

        // get around gems
        List<GemInfo> aroundGems = GameObject.Find("Board").GetComponent<BoardManager>().GetAroundGems(prevColumn, prevRow);

        // block rotate gems
        board.SetRotate(greenGem.GetColumn(), greenGem.GetRow(), true);
        for (int i = 0; i < aroundGems.Count; i++)
        {
            board.SetRotate(aroundGems[i].GetColumn(), aroundGems[i].GetRow(), true);
        }
        return specialGem;
    }

    void ChangeGemLast(GameObject bug, GemInfo target)
    {
        int targetColor = target.GetColor();
        int startColumn = bug.GetComponent<GemInfo>().GetColumn();
        int startRow = bug.GetComponent<GemInfo>().GetRow();

        // target gem change to green gem
        bug.GetComponent<GemInfo>().FadeOut(1f);
        StartCoroutine(DestroyObject(bug));
        target.ChangeGemColor((int)PatternType.GREEN);
        target.FadeIn(1f);

        // previous gem change to target color
        GemInfo previousGem = GameObject.Find("Board").GetComponent<BoardManager>().GetGem(startColumn, startRow);
        previousGem.ChangeGemColor(targetColor);
        previousGem.FadeIn(1f);

        // turn on rotate true
        StartCoroutine(TurnOnRotateTrue(previousGem));
    }


    IEnumerator TurnOnRotateTrue(GemInfo previousGem)
    {
        yield return new WaitForSeconds(1f);

        board.SetRotate(previousGem.GetColumn(), previousGem.GetRow(), false);

        // get around gems
        List<GemInfo> aroundGems = GameObject.Find("Board").GetComponent<BoardManager>().GetAroundGems(previousGem.GetColumn(), previousGem.GetRow());
        for (int i = 0; i < aroundGems.Count; i++)
        {
            board.SetRotate(aroundGems[i].GetColumn(), aroundGems[i].GetRow(), false);
        }
    }

    IEnumerator MoveStartToTarget(GameObject start, GameObject target, float endTime)
    {
        if (start == null)
            yield return null;

        float currentTime = 0.0f;
        Vector3 current = start.transform.position;

        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while (currentTime < endTime)
        {
            currentTime += Time.deltaTime * bugSpeed;
            if(start != null) start.transform.position = Vector3.Lerp(current, target.transform.position, currentTime / endTime);
            yield return waitForEndOfFrame;
        }
        if (start != null) start.transform.position = target.transform.position;
    }

    IEnumerator ChangeDirectionTowards(GameObject start, GameObject target)
    {
        // set angle
        Vector2 direction = new Vector2(target.transform.position.x - start.transform.position.x, target.transform.position.y - start.transform.position.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

        float currentTime = 0.0f;
        Quaternion current = start.transform.rotation;
        while (currentTime < rotateSpeed)
        {
            currentTime += (Time.deltaTime);
            start.transform.rotation = Quaternion.Slerp(current, angleAxis, currentTime / rotateSpeed);
            yield return null;
        }
        start.transform.rotation = angleAxis;
    }

    IEnumerator MoveBug(List<GemInfo> history, GameObject start)
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < history.Count; i++)
        {
            // get target object
            GameObject target = history[i].gameObject;

            // bug head towards target
            StartCoroutine(ChangeDirectionTowards(start, target));
            // hold time
            yield return new WaitForSeconds(rotateSpeed);

            // get direction start to target
            float endTime = Mathf.Sqrt(Mathf.Pow(start.transform.position.x - target.transform.position.x, 2) + Mathf.Pow(start.transform.position.y - target.transform.position.y, 2));

            // move bug
            StartCoroutine(MoveStartToTarget(start, target, endTime));
            // hold time
            yield return new WaitForSeconds(endTime / bugSpeed);

            //yield return new WaitForSeconds(Random.Range(0.0f, 0.5f));
        }
        ChangeGemLast(start, history[history.Count - 1]);
    }

    IEnumerator DestroyObject(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        Destroy(obj);
    }

    void GreenGimmick0()
    {
        // init
        check = new bool[11, 6];

        // create bug
        GameObject bug = CreateBug();

        // get history
        GemInfo bugInfo = bug.GetComponent<GemInfo>();
        List<GemInfo> history = GetHistoryGemRandom(2, bugInfo.GetColumn(), bugInfo.GetRow());

        // get target
        int idx = history.Count - 1;
        GemInfo target = GetNotGreenGemRandom(history[idx].GetColumn(), history[idx].GetRow());
        history.Add(target);

        // move bug
        StartCoroutine(MoveBug(history, bug));
    }

    // Green Gimmick 1 (Area) -------------------------------------------------------- //

    public bool IsInArea(int col_, int row_)
    {
        if (col_ >= 11 || row_ >= 6 || col_ < 0 || row_ < 0)
        {
            return false;
        }
        return area[col_, row_];
    }

    void ClearArea()
    {
        for(int i = 0; i < 11; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                area[i, j] = false;
            }
        }

        // delete bugs
        foreach (List<GameObject> value in bugs.Values)
        {
            for (int i = 0; i < value.Count; i++)
            {
                Destroy(value[i]);
            }
        }
    }

    List<GemInfo> FindOppositeArea(List<List<GemInfo>> aroundRoadGems)
    {
        List<GemInfo> possibleDirection = new List<GemInfo>();
        for(int i = 0; i < aroundRoadGems.Count; i++)
        {
            bool check = true;
            for (int j = 0; j < aroundRoadGems[i].Count; j++)
            {
                if(area[aroundRoadGems[i][j].GetColumn(), aroundRoadGems[i][j].GetRow()])
                {
                    check = false;
                    break;
                }
            }
            if (check)
            {
                if (aroundRoadGems[i].Count == 0) continue;
                possibleDirection.Add(aroundRoadGems[i][aroundRoadGems[i].Count - 1]);
            }
        }

        return possibleDirection;
    }

    IEnumerator BugDisappear()
    {
        foreach (GemInfo key in bugs.Keys)
        {
            // 0: up, 1: up&right, 2: down&right, 3:down, 4: down&left, 5: up&left
            List<List<GemInfo>> aroundRoadGems = board.GetAroundGemList(key.GetColumn(), key.GetRow());
            List<GemInfo> possibleDirection = FindOppositeArea(aroundRoadGems);

            for (int i = 0; i < bugs[key].Count; i++)
            {
                int randomIdx = Random.Range(0, possibleDirection.Count);
                if(possibleDirection.Count > 0) StartCoroutine(MoveStartToTarget(bugs[key][i].gameObject, possibleDirection[randomIdx].gameObject, 3f));
                bugs[key][i].GetComponent<PatternAreaBug>().FadeOut(0.5f);
            }

        }

        yield return new WaitForSeconds(0.5f);
        ClearArea();
        SetAreaAgain();
    }

    void SetColor()
    {
        // create bugs (extends area)
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if ((j == 5 && i % 2 == 0) || area[i, j])
                {
                    continue;
                }
               
                Vector3 gemPos = board.GetGemPosition(i, j);

                for (int k = 0; k < 2; k++)
                {
                    GameObject bug = Instantiate(bugPF, gemPos, Quaternion.identity, UICanvas.transform);
                    Vector3 originPos = bug.transform.position;
                    float interval = 0.2f;
                    Vector3 newPos = new Vector3(originPos.x + Random.Range(-interval, interval), originPos.y + Random.Range(-interval, interval), originPos.z);
                    //bug.GetComponent<PatternAreaBug>().SetBugPos(newPos);
                    bug.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
                    //bug.transform.position = board.GetDropPosition(i);
                    //bug.GetComponent<PatternAreaBug>().FallBug();
                    bug.transform.position = newPos;
                    bug.GetComponent<PatternAreaBug>().SizeDown();

                    GemInfo tempGem = board.GetGem(i, j);
                    if(bugs.ContainsKey(tempGem) == false)
                    {
                        bugs.Add(tempGem, new List<GameObject>());
                    }
                    bugs[tempGem].Add(bug);
                }
            }
        }
    }


    bool SetAreas(int col_, int row_, int level_)
    {
        area[col_, row_] = true;

        //// hard mode
        //if(mini.patternLevel == (int)LevelType.HARD1 || mini.patternLevel == (int)LevelType.HARD2)
        //{
        //    int idx = col_ % 2;
        //    int[,] goal_area = GameObject.Find("Board").GetComponent<GoalInfo>().GetGoal();
        //    for(int i=0;i<goal_area.GetLength(1); i+=2)
        //    {
        //        int new_col = col_ + goal_area[idx, i];
        //        int new_row = row_ + goal_area[idx, i + 1];
        //        if (new_col >= 11 || new_row >= 6 || new_col < 0 || new_row < 0 || (new_col % 2 == 0 && new_row > 4))
        //        {
        //            Debug.Log("영역 범위 넘어서 Area 재설정");
        //            SetAreaAgain();
        //            return false;
        //        }
        //        area[new_col, new_row] = true;
        //    }
        //    return true;
        //}

        // choose direction vector about even or odd column
        int[,] direction;
        if (col_ % 2 == 0) // even
        {
            direction = aroundGem_e;
        }
        else // odd
        {
            direction = aroundGem_o;
        }

        for(int i = 0; i < 6; i++)
        {
            int new_col = col_ + direction[i, 0];
            int new_row = row_ + direction[i, 1];
            if (new_col >= 11 || new_row >= 6 || new_col < 0 || new_row < 0 || (new_col % 2 == 0 && new_row > 4))
            {
                Debug.Log("영역 범위 넘어서 Area 재설정");
                SetAreaAgain();
                return false;
            }
            area[new_col, new_row] = true;
            GemInfo gem = board.GetGem(new_col, new_row);
            if (gem != null && level_ > 1)
            {
                bool result = SetAreas(new_col, new_row, level_ - 1);
                if (!result)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void SetAreaAgain()
    {
        GemInfo gem = board.GetRandomGemArea();
        int level_ = 1;
        //if (mini.patternLevel == (int)LevelType.EASY1 || mini.patternLevel == (int)LevelType.EASY2)
        //{
        //    level_ = 2;
        //}
        SetAreas(gem.GetColumn(), gem.GetRow(), level_);
        SetColor();
    }
}
