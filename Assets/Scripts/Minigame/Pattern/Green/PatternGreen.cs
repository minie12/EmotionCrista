using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternGreen : PatternManager
{
    private GameObject gemPF;
    private bool[,] check = new bool[11, 6];

    private float bugSpeed = 3f;
    private float rotateSpeed = 0.5f;
    private float bugInterval = 60f;

    private int[,] aroundGem_o = new int[6, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 } };
    private int[,] aroundGem_e = new int[6, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

    private bool[,] area = new bool[11, 6];
    private bool isPlaying = false; // manage gimmick start & end

    protected override void Awake()
    {
        base.Awake();
        gemPF = Resources.Load<GameObject>("Prefabs/MiniGame/org_gem");
    }

    override public void StartPattern(int gimmick_, int level_)
    {
        gimmick = gimmick_;
        level = level_;
        OrganizeCharacterChat();

        // give term before choose gem because board init
        if (gimmick == 0) 
        {
            InvokeRepeating("GreenGimmick0", 1f, bugInterval); 
        } 
        else if (gimmick == 1)
        {
            Invoke("GreenGimmick1", 1f);
        }
    }

    override public void StopPattern() { CancelInvoke(); }
    override public void RestartPattern()
    {
        if (gimmick == 0) 
        {
            InvokeRepeating("GreenGimmick0", 1f, bugInterval);
        }
        else if (gimmick == 1)
        {
            Invoke("GreenGimmick1", 1f);
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
       
        greenGem.FadeOut(1f);

        // just create image
        GameObject specialGem = Instantiate(gemPF, greenGem.GetComponent<Transform>().position, Quaternion.identity, this.transform);
        specialGem.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 3;
        GemInfo specialGemInfo = specialGem.GetComponent<GemInfo>();
        specialGemInfo.SetBackgroundColor(255f, 255f, 255f, 0f);
        specialGemInfo.InitGem(prevColumn, prevRow, (int)PatternType.GREEN);
        specialGemInfo.ChangeSpecialGem();

        specialGemInfo.FadeIn(1f);

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
        float currentTime = 0.0f;
        Vector3 current = start.transform.position;

        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while (currentTime < endTime)
        {
            currentTime += Time.deltaTime * bugSpeed;
            start.transform.position = Vector3.Lerp(current, target.transform.position, currentTime / endTime);
            yield return waitForEndOfFrame;
        }
        start.transform.position = target.transform.position;
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
        yield return new WaitForSeconds(bugSpeed);
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

            yield return new WaitForSeconds(Random.Range(0.0f, 1.5f));
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
    }

    void SetColor()
    {
        for(int i = 0; i < 11; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                if (area[i, j])
                {
                    GemInfo gem = board.GetGem(i, j);
                    gem?.SetBackgroundColor(255f, 255f, 255f, 255f);
                }
                else
                {
                    GemInfo gem = board.GetGem(i, j);
                    gem?.SetBackgroundColor(100f, 100f, 100f, 100f);
                }
            }
        }
    }


    void SetAreas(int col_, int row_, int level_)
    {
        area[col_, row_] = true;

        // hard mode
        if(level == (int)LevelType.HARD1 || level == (int)LevelType.HARD2)
        {
            int idx = col_ % 2;
            int[,] goal_area = GameObject.Find("Board").GetComponent<GoalInfo>().GetGoal();
            for(int i=0;i<goal_area.GetLength(1); i+=2)
            {
                int new_col = col_ + goal_area[idx, i];
                int new_row = row_ + goal_area[idx, i + 1];
                if (new_col >= 11 || new_row >= 6 || new_col < 0 || new_row < 0 || (new_col % 2 == 0 && new_row > 4))
                {
                    Debug.Log("영역 범위 넘어서 Area 재설정");
                    SetAreaAgain();
                    break;
                }
                area[new_col, new_row] = true;
            }
            return;
        }

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
                break;
            }
            area[new_col, new_row] = true;
            GemInfo gem = board.GetGem(new_col, new_row);
            if (gem != null && level_ > 1)
            {
                SetAreas(new_col, new_row, level_ - 1);
            }
        }
    }

    public void SetAreaAgain()
    {
        ClearArea();
        GemInfo gem = board.GetRandomGemArea();
        int level_ = 1;
        if (level == (int)LevelType.EASY1 || level == (int)LevelType.EASY2)
        {
            level_ = 2;
        }
        SetAreas(gem.GetColumn(), gem.GetRow(), level_);
    }

    void GreenGimmick1()
    {
        isPlaying = true;
        SetAreaAgain();
    }

    private void Update()
    {
        if (isPlaying)
        {
            SetColor();
        }
    }
}
