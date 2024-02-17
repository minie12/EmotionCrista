using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternRed : PatternManager
{
    // red around gems direction vector (odd/even standard: col)
    private readonly int[,] aroundGem_o = new int[6, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 } };
    private readonly int[,] aroundGem_e = new int[6, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

    private GemInfo startGem;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnCrushedGem(bool isMatchColor)
    {
        base.OnCrushedGem(isMatchColor);

        // red gimmick
        if (isMatchColor && gimmick[0])
        {
            InvokeExplosion();
        }
    }

    // Setting gimmick
    public override void StartPattern(int level_)
    {
        base.StartPattern(level_);

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
            case 2:
                Invoke(nameof(StartRedGimmick2), 2f);
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
        }
    }

    public override void RestartPattern()
    {
        base.RestartPattern();

        gimmick[0] = true;
        // [TODO] 기획
        switch (mini.patternLevel)
        {
            case 0:
                StartGimmick(2);
                break;
            case 1:
                StartGimmick(1);
                StartGimmick(2);
                break;
            case 2:
                StartGimmick(2);
                break;
            case 3:
                StartGimmick(1);
                StartGimmick(2);
                break;
            case 4:
                StartGimmick(2);
                break;
            case 5:
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
        startGem.FireGem(true);

        StartCoroutine(SpreadFire(3f));
    }

    private IEnumerator AfterExplode(float previousTime, GemInfo gem)
    {
        yield return new WaitForSeconds(previousTime);

        if (gem != null)
        {
            gem.ExplosionGem();
            yield return new WaitForSeconds(0.1f); // wait for gem crush
            GameObject.Find("Board").GetComponent<BoardManager>().RefillBoardOut();
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

    private List<int> FindStartFire()
    {
        List<int> startFire = new List<int>();
        for(int i = 0; i <= 10; i++)
        {
            for(int j = 0; j <= 5; j++)
            {
                // outside board
                if (j == 5 && i % 2 == 0)
                {
                    continue;
                }
                GemInfo gem = board.GetGem(i, j);
                if(gem == null)
                {
                    continue;
                }
                gem.isChecked = false;
                // find start fire
                if (gem.isFired == 1)
                {
                    int col = gem.GetColumn();
                    int row = gem.GetRow();
                    startFire.Add(col);
                    startFire.Add(row);
                }
            }
        }
        return startFire;
    }

    private IEnumerator SpreadFire(float interval)
    {
        while (gimmick[1])
        {
            yield return new WaitForSeconds(interval);

            // 0. get fire gem list
            List<GemInfo> gemList = new List<GemInfo>();
            List<List<int>> queueList = new List<List<int>>();
            List<int> startFire = FindStartFire();
           
            if (startFire.Count == 2)
            {
                queueList.Add(FindStartFire());
            }
            while (queueList.Count > 0)
            {
                List<int> cur = queueList[0]; // 0:column, 1:row
                queueList.RemoveAt(0);

                int col = cur[0];
                int row = cur[1];
                GemInfo gem = board.GetGem(col, row);
                if (gem != null)
                {
                    gem.isChecked = true;
                }
                int[,] dir = col % 2 == 0 ? aroundGem_e : aroundGem_o;
                bool chk = false;
                for(int i = 0; i < 6; i++)
                {
                    int newCol = col + dir[i, 0];
                    int newRow = row + dir[i, 1];
                    GemInfo tempGem = board.GetGem(newCol, newRow);
                    if(tempGem != null && tempGem.isChecked == false && tempGem.isFired > 0)
                    {
                        chk = true;
                        tempGem.isChecked = true;
                        List<int> temp = new List<int>();
                        temp.Add(newCol);
                        temp.Add(newRow);
                        queueList.Add(temp);
                    } 
                }
                if (!chk)
                {
                    gemList.Add(gem);
                }
            }
            //for (int i = 0; i <= 10; i++)
            //{
            //    for (int j = 0; j <= 5; j++)
            //    {
            //        // outside board
            //        if (j == 5 && i % 2 == 0)
            //        {
            //            break;
            //        }
            //        GemInfo gem = board.GetGem(i, j);
            //        if (gem != null && gem.isFired > 0)
            //        {
            //            gemList.Add(GameObject.Find("Board").GetComponent<BoardManager>().GetGem(i, j));
            //        }
            //    }
            //}
            if(gemList.Count == 0)
            {
                break;
            }

            // 1. get around gems
            bool[,] aroundCheck = new bool[11, 6];
            for (int i = 0; i < gemList.Count; i++)
            {
                int cur_col = gemList[i].GetColumn();
                int cur_row = gemList[i].GetRow();

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
                    // if around already fired then continue
                    GemInfo gem = board.GetGem(new_col, new_row);
                    if (gem.isFired > 0)
                    {
                        continue;
                    }

                    aroundCheck[new_col, new_row] = true;
                }
            }
            List<GemInfo> aroundGems = new List<GemInfo>();
            for (int i = 0; i <= 10; i++)
            {
                for (int j = 0; j <= 5; j++)
                {
                    // outside board
                    if (j == 5 && i % 2 == 0)
                    {
                        break;
                    }
                    if (aroundCheck[i, j] && GameObject.Find("Board").GetComponent<BoardManager>().GetGem(i, j) != null)
                    {
                        aroundGems.Add(GameObject.Find("Board").GetComponent<BoardManager>().GetGem(i, j));
                    }
                }
            }

            // 2. choose fire gems
            int randCnt = Mathf.Min(Random.Range(1, 4), aroundGems.Count);
            for (int i = 0; i < randCnt;)
            {
                int rand = Random.Range(0, aroundGems.Count);
                GemInfo temp = aroundGems[rand];
                if (temp.isFired > 0)
                {
                    continue;
                }
                temp.FireGem();
                StartCoroutine(AfterExplode(6f, temp));
                i++;
            }
        }
        yield return null;
    }

    // Red gimmick 2

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


    IEnumerator ExplosionGemsStep()
    {
        List<GemInfo> gems = board.GetPatternGems()[2];
        AllGemVibration(gems);
        yield return new WaitForSeconds(1f);
        AllGemExplosion(gems);
        // refil board
        board.StartRefilBoardFever();
        yield return new WaitForSeconds(1f);

        gems = board.GetGemColumns(new List<int>() { 1, 5, 9 });
        AllGemVibration(gems);
        yield return new WaitForSeconds(1f);
        AllGemExplosion(gems);
        // refil board
        board.StartRefilBoardFever();
        yield return new WaitForSeconds(1f);

        gems = board.GetGemColumns(new List<int>() { 1, 3, 5, 7, 9 });
        AllGemVibration(gems);
        yield return new WaitForSeconds(1f);
        AllGemExplosion(gems);
        // refil board
        board.StartRefilBoardFever();
        yield return new WaitForSeconds(1f);

        gems = board.GetGemDiagonalRight(new List<int>() { 1, 4, 7 });
        AllGemVibration(gems);
        yield return new WaitForSeconds(1f);
        AllGemExplosion(gems);
        // refil board
        board.StartRefilBoardFever();
        yield return new WaitForSeconds(1f);

        gems = board.GetGemDiagonalLeft(new List<int>() { 1, 4, 7 });
        AllGemVibration(gems);
        yield return new WaitForSeconds(1f);
        AllGemExplosion(gems);
        // refil board
        board.StartRefilBoardFever();
    }

    void StartRedGimmick2()
    {
        StartCoroutine(ExplosionGemsStep());
    }
}
