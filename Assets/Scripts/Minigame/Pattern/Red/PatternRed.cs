using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternRed : PatternManager
{
    // red around gems direction vector (odd/even standard: col)
    private readonly int[,] aroundGem_o = new int[6, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 } };
    private readonly int[,] aroundGem_e = new int[6, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

    private GemInfo startGem;
    private ShakeObjectManager shakeObjectManager;

    // red gimmick 2 (attack desk)
    private bool isPlayingGimmick2 = false;
    private bool isAttacking = false;
    private int crushedBiasCnt = 10;
    private int crushedGemLast = 0;
    private IEnumerator attackCoroutine, explosioningCoroutine, continueAttackCoroutine;
    private readonly List<List<int>> optionNums = new List<List<int>> { new List<int> { 2 },
                                                                        new List<int> { 2 },
                                                                        new List<int> { 1, 2 },
                                                                        new List<int> { 1, 2, 3, 5 },
                                                                        new List<int> { 1, 2, 3, 4, 5 },
                                                                        new List<int> { 1, 2, 3, 4, 5, 6 }};
    private readonly List<List<int>> continuityPer = new List<List<int>> { new List<int> { 100 },
                                                                            new List<int> { 100 },
                                                                            new List<int> { 70, 30 },
                                                                            new List<int> { 65, 35 },
                                                                            new List<int> { 55, 40, 5 },
                                                                            new List<int> { 50, 40, 10 }};
    private readonly List<List<float>> continuityInterval = new List<List<float>> { new List<float> { 0, 0 },
                                                                                    new List<float> { 0, 0 },
                                                                                    new List<float> { 6, 6},
                                                                                    new List<float> { 6, 6 },
                                                                                    new List<float> { 5, 6 },
                                                                                    new List<float> { 4, 6 }};

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        shakeObjectManager = GameObject.Find("ShakeObjectManager").GetComponent<ShakeObjectManager>();
    }

    public override void OnCrushedGem(bool isMatchColor, List<List<int>> crushedGems)
    {
        base.OnCrushedGem(isMatchColor, crushedGems);

        // red gimmick 0
        if (isMatchColor && gimmick[0])
        {
            InvokeExplosion();
        }

        // red gimmick 2
        if (gimmick[2])
        {
            Debug.Log("attack crushed gem cnt" + mini.GetTotalCrushedGem());
            int cnt = mini.GetTotalCrushedGem() / crushedBiasCnt;
            Debug.Log("attack desk gimmick" + cnt);
            if (!isPlayingGimmick2 && cnt > crushedGemLast)
            {
                Debug.Log("attacking !!");
                isPlayingGimmick2 = true;
                crushedGemLast = cnt;

                // 기믹 번호 랜덤으로 정하기
                int optionIdx = Random.Range(0, optionNums[mini.patternLevel].Count);
                int option = optionNums[mini.patternLevel][optionIdx];
                Debug.Log("attack option " + option);

                // 연속 여부 처리하기
                int randNum = Random.Range(0, 100);
                int sum = 0;
                int continuityCnt = 0;
                for(int i = 0; i < continuityPer[mini.patternLevel].Count; i++)
                {
                    if (randNum < sum + continuityPer[mini.patternLevel][i])
                    {
                        continuityCnt = i;
                        break;
                    }
                    sum += continuityPer[mini.patternLevel][i];
                }
                Debug.Log("attack 연속 횟수 !! " + continuityCnt);

                continueAttackCoroutine = ExplodeContinue(option, continuityCnt);
                StartCoroutine(continueAttackCoroutine);
            }
        }
    }

    // Setting gimmick
    public override void StartPattern(int level_)
    {
        base.StartPattern(level_);

        crushedGemLast = 0;
        isPlayingGimmick2 = false;
        isAttacking = false;

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
            case 1:
                Invoke(nameof(StartFireRoad), 1f);
                break;
        }
    }

    public override void StopGimmick(int gimmick_)
    {
        base.StopGimmick(gimmick_);

        switch (gimmick_)
        {
            case 1:
                CancelInvoke(nameof(StartFireRoad));
                StartCoroutine(InitFire());
                break;
            case 2:
                if (attackCoroutine != null) StopCoroutine(attackCoroutine);
                if (explosioningCoroutine != null) StopCoroutine(explosioningCoroutine);
                if (continueAttackCoroutine != null) StopCoroutine(continueAttackCoroutine);
                break;
        }
    }

    public override void RestartPattern()
    {
        base.RestartPattern();

        gimmick[0] = true;
        mini.SetGameTimeInit(200f, 2f, 1f, 100f, 2.8f, (int)GoalUnit.TWO);

        // [TODO] 기획
        switch (mini.patternLevel)
        {
            case (int)LevelType.EASY1:
                StartGimmick(2);
                break;
            case (int)LevelType.NORMAL1:
                StartGimmick(2);
                break;
            case (int)LevelType.HARD1:
                StartGimmick(1);
                StartGimmick(2);
                break;
            case (int)LevelType.EASY2:
                StartGimmick(2);
                break;
            case (int)LevelType.NORMAL2:
                StartGimmick(1);
                StartGimmick(2);
                break;
            case (int)LevelType.HARD2:
                StartGimmick(1);
                StartGimmick(2);
                break;
        }
    }

    // get explosion gem cnt on percentage
    int GetExplosionGemCnt(int [] percentage)
    {
        // get random num
        int rand = Random.Range(0, 100);

        int sum = 0;
        for(int i = 0; i < percentage.Length; i++)
        {
            sum += percentage[i];

            // find
            if(rand < sum)
            {
                return i + 1;
            }
        }
        return 0;
    }

    // extract boolean board to list
    List<List<int>> BoardToList(int colSize, int rowSize, bool[,] board)
    {
        List<List<int>> result = new List<List<int>>();

        for (int i = 0; i <= colSize; i++)
        {
            for (int j = 0; j <= rowSize; j++)
            {
                // outside board
                if (j == 5 && i % 2 == 0)
                {
                    break;
                }
                if (board[i, j])
                {
                    result.Add(new List<int> { i, j });
                }
            }
        }
        return result;
    }

    void Explode(List<List<int>> aroundGems, int explosionNum)
    {
        // check already explosion
        int size = aroundGems.Count;
        bool[] check = new bool[size];
        for (int i = 0; i < explosionNum;)
        {
            int rand = Random.Range(0, aroundGems.Count);
            int column_ = aroundGems[rand][0];
            int row_ = aroundGems[rand][1];

            if (check[rand])
            {
                continue;
            }

            Debug.Log("폭발하는 광물: " + column_ + ", " + row_);
            check[rand] = true;
            GameObject.Find("Board").GetComponent<BoardManager>().ExplosionGem(column_, row_);
            i++;

            // check start gem
            if (gimmick[1] && startGem.GetColumn() == column_ && startGem.GetRow() == row_)
            {
                StartCoroutine(InitFire());
            }
        }
    }

    // Red gimmick 0
    public void ExplosionAroundGems()
    {
        // crushed gems info load of GoalInfo.cs
        List<List<int>> crushedGems = GameObject.Find("Board").GetComponent<GoalInfo>().crushedGems;

        // crushed gems check (because seperate around and crushed)
        bool[,] crushedCheck = new bool[11, 6];
        for(int i = 0; i < crushedGems.Count; i++)
        {
            int cur_col = crushedGems[i][0];
            int cur_row = crushedGems[i][1];
            Debug.Log("크러쉬된 광물: " + cur_col + ", " + cur_row);
            crushedCheck[cur_col, cur_row] = true;

            // check start gem
            if (gimmick[1] && startGem.GetColumn() == cur_col && startGem.GetRow() == cur_row)
            {
                StartCoroutine(InitFire());
            }
        }

        // extract without overlap gems
        bool[,] aroundCheck = new bool[11, 6];
        for (int i = 0; i < crushedGems.Count; i++)
        {
            int cur_col = crushedGems[i][0];
            int cur_row = crushedGems[i][1];

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
                // if around already crushed then continue
                if (crushedCheck[new_col, new_row])
                {
                    continue;
                }

                aroundCheck[new_col, new_row] = true;
            }
        }

        // extract around gems for 2nd list
        List<List<int>> aroundGems = BoardToList(10, 5, aroundCheck);

        // get explosion num with percentage
        int[] percentage = { 80, 20 };
        int explosionNum = GetExplosionGemCnt(percentage);
        if (explosionNum > 8) explosionNum = aroundGems.Count;
        // choose min value 
        explosionNum = System.Math.Min(explosionNum, aroundGems.Count);

        // choose random explosion gem
        Explode(aroundGems, explosionNum);

        // refil board
        GameObject.Find("Board").GetComponent<BoardManager>().StartRefilBoardFever();
    }

    // Red gimmick 0
    public void InvokeExplosion()
    {
        Invoke(nameof(ExplosionAroundGems), 0.2f);
    }

    // Red gimmick 1 (fire road)
    private void StartFireRoad()
    {
        // get random gem
        startGem = board.GetPatternGemRandom();

        if(startGem == null)
        {
            return;
        }

        startGem.FireGem(true);
        startGem.bPatternApplied = true;

        StartCoroutine(SpreadFire(3f));
    }

    private IEnumerator AfterExplode(float previousTime, GemInfo gem)
    {
        yield return new WaitForSeconds(previousTime);

        if (gem != null)
        {
            gem.ExplosionGem();
            yield return new WaitForSeconds(0.1f); // wait for gem crush
            board.RefillBoardOut();
        }
    }

    public IEnumerator InitFire()
    {
        Debug.Log("init gem fired");

        yield return null;

        // stop coroutine
        StopAllCoroutines();

        for (int i = 0; i <= 10; i++)
        {
            for (int j = 0; j <= 5; j++)
            {
                // outside board
                if (j == 5 && i % 2 == 0)
                {
                    break;
                }
                GemInfo gem = board.GetGem(i, j);
                if (gem == null || gem.isFired == 0)
                {
                    continue;
                }
                gem.StopFireGem();
            }
        }
    }

    // BFS 탐색으로 현재 불길의 가장 가장자리 Gem List 얻기
    List<GemInfo> GetBoundaryFireGem(int checkIdx)
    {
        List<GemInfo> gemList = new List<GemInfo>();
        List<List<int>> queueList = new List<List<int>>();

        // 불길 광물 큐에 넣기
        queueList.Add(new List<int> { startGem.GetColumn(), startGem.GetRow() });
        while (queueList.Count > 0)
        {
            List<int> cur = queueList[0]; // 0:column, 1:row
            queueList.RemoveAt(0);

            int col = cur[0];
            int row = cur[1];
            GemInfo gem = board.GetGem(col, row);
            if (gem != null)
            {
                gem.isChecked = checkIdx;
            }
            int[,] dir = col % 2 == 0 ? aroundGem_e : aroundGem_o;
            bool chk = false;
            for (int i = 0; i < 6; i++)
            {
                int newCol = col + dir[i, 0];
                int newRow = row + dir[i, 1];
                GemInfo tempGem = board.GetGem(newCol, newRow);
                if (tempGem != null && tempGem.isChecked != checkIdx && tempGem.isFired > 0)
                {
                    chk = true;
                    tempGem.isChecked = checkIdx;
                    queueList.Add(new List<int> { newCol, newRow });
                }
            }
            if (!chk)
            {
                gemList.Add(gem);
            }
        }

        return gemList;
    }

    private IEnumerator SpreadFire(float interval)
    {
        int checkIdx = 1; // 불길 번짐 index (check 구분하기 위해 사용)

        while (gimmick[1])
        {
            yield return new WaitForSeconds(interval);

            // 현재 불길 광물이 없다면
            if (startGem == null)
            {
                InitFire();
                yield break;
            }

            // 0. get fire gem list (BFS 탐색으로 가장 가장자리 Gem 얻어오기)
            List<GemInfo> gemList = GetBoundaryFireGem(checkIdx);

            if (gemList.Count == 0)
            {
                InitFire();
                yield break;
            }

            // 1. get around gems
            List<GemInfo> allAroundGems = new List<GemInfo>(); // 가장자리에 있는 불길의 주변 광물 전부
            for (int i = 0; i < gemList.Count; i++)
            {
                int curCol = gemList[i].GetColumn();
                int curRow = gemList[i].GetRow();

                List<GemInfo> eachAroundGems = board.GetAroundGems(curCol, curRow);

                // aroundGems
                foreach (GemInfo gemInfo in eachAroundGems)
                {
                    // if around already fired then continue
                    int newCol = gemInfo.GetColumn();
                    int newRow = gemInfo.GetRow();
                    GemInfo gem = board.GetGem(newCol, newRow);
                    if (gem != null && gem.isFired > 0)
                    {
                        continue;
                    }

                    allAroundGems.Add(gemInfo);
                }
            }
            if (allAroundGems.Count == 0)
            {
                InitFire();
                yield break;
            }


            // 2. choose fire gems
            int randCnt = Mathf.Min(Random.Range(1, 4), allAroundGems.Count);
            for (int i = 0; i < randCnt; i++)
            {
                if(allAroundGems.Count == 0)
                {
                    break;
                }

                int rand = Random.Range(0, allAroundGems.Count);
                GemInfo temp = allAroundGems[rand];
                if (board.GetGem(temp.GetColumn(), temp.GetRow()) == null || temp.isFired > 0)
                {
                    allAroundGems.RemoveAt(rand);
                    continue;
                }
                temp.FireGem();
                temp.bPatternApplied = true;
                StartCoroutine(AfterExplode(6f, temp));
                allAroundGems.RemoveAt(rand);
            }

            checkIdx++;
        }
        yield return null;
    }

    // Red gimmick 2 (책상 치기)
    void AllGemVibration(List<GemInfo> gems)
    {
        for (int i = 0; i < gems.Count; i++)
        {
            gems[i].GemShake(0f, 0.1f, 0.5f);
        }
    }

    void AllGemExplosion(List<GemInfo> gems)
    {
        for (int i = 0; i < gems.Count; i++)
        {
            board.ExplosionGem(gems[i].GetColumn(), gems[i].GetRow());
        }
    }

    IEnumerator ExplodeGems(List<GemInfo> gems)
    {
        AllGemVibration(gems);
        yield return new WaitForSeconds(1f);
        AllGemExplosion(gems);
    }

    /** 광물 터지기
     * 1. 빨간 광물만 터짐
     * 2. 세로 폭발 (3줄)
     * 3. 대각선 폭발 (왼쪽 대각선 3줄)
     * 4. 세로 폭발 (6줄)
     * 5. 대각선 폭발 (오른쪽 대각선 3줄)
     * 6. 전체 다 폭발
     * */
    IEnumerator ExplodeOptionalGem(int option)
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.5f);

        // 광물 터지기 전 보드판 잠금
        board.SetGemMovable(false);
        board.SetGemClicked(false);
        yield return new WaitForSeconds(0.1f);

        // 옵션에 따라 광물 터지기 & 진동
        List<GemInfo> gems = null;
        switch (option)
        {
            case 1:
                gems = board.GetPatternGems()[(int)PatternType.RED];
                if(gems.Count == 0)
                {
                    break;
                }
                explosioningCoroutine = ExplodeGems(gems);
                StartCoroutine(explosioningCoroutine);

                // 배경만 흔들기
                yield return new WaitForSeconds(1f);
                shakeObjectManager.ShakeBackground();
                break;
            case 2:
                gems = board.GetGemColumns(new List<int>() { 1, 5, 9 });
                if (gems.Count == 0)
                {
                    break;
                }
                explosioningCoroutine = ExplodeGems(gems);
                StartCoroutine(explosioningCoroutine);

                // 보드판만 흔들기
                yield return new WaitForSeconds(1f);
                shakeObjectManager.ShakePuzzle(); 
                break;
            case 3:
                gems = board.GetGemDiagonalRight(new List<int>() { 1, 4, 7 });
                if (gems.Count == 0)
                {
                    break;
                }
                explosioningCoroutine = ExplodeGems(gems);
                StartCoroutine(explosioningCoroutine);

                // 보드판만 흔들기
                yield return new WaitForSeconds(1f);
                shakeObjectManager.ShakePuzzle();
                break;
            case 4:
                gems = board.GetGemColumns(new List<int>() { 1, 3, 5, 7, 9 });
                if (gems.Count == 0)
                {
                    break;
                }
                explosioningCoroutine = ExplodeGems(gems);
                StartCoroutine(explosioningCoroutine);

                // 보드판 & 보드판 UI 흔들기
                yield return new WaitForSeconds(1f);
                shakeObjectManager.ShakePuzzleUi();
                break;
            case 5:
                gems = board.GetGemDiagonalLeft(new List<int>() { 1, 4, 7 });
                if (gems.Count == 0)
                {
                    break;
                }
                explosioningCoroutine = ExplodeGems(gems);
                StartCoroutine(explosioningCoroutine);

                // 보드판 & 보드판 UI 흔들기
                yield return new WaitForSeconds(1f);
                shakeObjectManager.ShakePuzzleUi();
                break;
            case 6:
                gems = board.GetGemAll();
                if (gems.Count == 0)
                {
                    break;
                }
                explosioningCoroutine = ExplodeGems(gems);
                StartCoroutine(explosioningCoroutine);

                // 전체 다 흔들기
                yield return new WaitForSeconds(1f);
                shakeObjectManager.ShakeAll();
                break;
        }
        if(gems.Count != 0)
        {
            board.StartRefilBoardFever();
        }
        yield return new WaitForSeconds(0.5f);

        board.SetGemMovable(true);
        isAttacking = false;
    }

    IEnumerator ExplodeContinue(int option, int continuityCnt)
    {
        isPlayingGimmick2 = true;

        attackCoroutine = ExplodeOptionalGem(option);
        StartCoroutine(attackCoroutine);

        while (isAttacking == true)
        {
            yield return null;
        }

        for (int i = 0; i < continuityCnt; i++)
        {
            float minInterval = continuityInterval[mini.patternLevel][0];
            float maxInterval = continuityInterval[mini.patternLevel][1];
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            attackCoroutine = ExplodeOptionalGem(option);
            StartCoroutine(attackCoroutine);
            while (isAttacking == true)
            {
                yield return null;
            }
        }
      
        isPlayingGimmick2 = false;
    }
}
