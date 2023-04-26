using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternRed : PatternManager
{
    // red around gems direction vector (odd/even standard: col)
    private int[,] aroundGem_o = new int[6, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 } };
    private int[,] aroundGem_e = new int[6, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

    private bool isPlaying = false; // manage gimmick start & end

    // check exist fire
    private bool[,] fireCheck = new bool[11, 6];
    private GemInfo startGem;

    protected override void Awake()
    {
        base.Awake();
    }


    // Setting gimmick
    override public void StartPattern(int gimmick_, int level_)
    {
        gimmick = gimmick_;
        level = level_;
        isPlaying = true;
        OrganizeCharacterChat();
        Invoke("StartFireRoad", 1f);
    }

    override public void StopPattern() 
    {
        isPlaying = false;
        CancelInvoke();
    }

    override public void RestartPattern() 
    {
        isPlaying = true;
        OrganizeCharacterChat();
        Invoke("StartFireRoad", 1f);
    }

    public bool GetIsPlaying()
    {
        return isPlaying;
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

            Debug.Log("Æø¹ßÇÏ´Â ±¤¹°: " + column_ + ", " + row_);
            check[rand] = true;
            GameObject.Find("Board").GetComponent<BoardManager>().ExplosionGem(column_, row_);
            i++;

            // check start gem
            if (startGem.GetColumn() == column_ && startGem.GetRow() == row_)
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
            Debug.Log("Å©·¯½¬µÈ ±¤¹°: " + cur_col + ", " + cur_row);
            crushedCheck[cur_col, cur_row] = true;

            // check start gem
            if (startGem.GetColumn() == cur_col && startGem.GetRow() == cur_row)
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
        Invoke("ExplosionAroundGems", 0.2f);
    }

    // Red gimmick 1 (fire road)
    private void StartFireRoad()
    {
        // get random gem
        startGem = mini.GetPatternGemRandom();
        startGem.FireGem(true);
        fireCheck[startGem.GetColumn(), startGem.GetRow()] = true;

        StartCoroutine(SpreadFire(3f));
        //StartCoroutine(StopFire(startGem));
    }

    private IEnumerator AfterExplode(float previousTime, GemInfo gem)
    {
        yield return new WaitForSeconds(previousTime);

        if (fireCheck[gem.GetColumn(), gem.GetRow()] == true && gem)
        {
            fireCheck[gem.GetColumn(), gem.GetRow()] = false;
            gem.ExplosionGem();
            yield return new WaitForSeconds(0.1f); // wait for gem crush
            GameObject.Find("Board").GetComponent<BoardManager>().RefillBoardOut();
        }
    }

    public void SetFireCheckFalse(int col, int row)
    {
        fireCheck[col, row] = false;
    }

    public bool GetFireCheck(int col, int row)
    {
        return fireCheck[col, row];
    }

    public void SetFireCheckTrue(int col, int row)
    {
        fireCheck[col, row] = true;
    }

    public IEnumerator InitFire()
    {
        Debug.Log("init gem fired");

        yield return null;
        // yield return new WaitForSeconds(0.3f); // wait for gem crush

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
                if (fireCheck[i, j] == false)
                {
                    continue;
                }
                fireCheck[i, j] = false;
                GemInfo temp = GameObject.Find("Board").GetComponent<BoardManager>().GetGem(i, j);
                if (temp != null)
                {
                    temp.StopFireGem();
                }
            }
        }
    }

    private IEnumerator SpreadFire(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            // 0. get fire gem list
            List<GemInfo> gemList = new List<GemInfo>();
            for (int i = 0; i <= 10; i++)
            {
                for (int j = 0; j <= 5; j++)
                {
                    // outside board
                    if (j == 5 && i % 2 == 0)
                    {
                        break;
                    }
                    if (fireCheck[i, j])
                    {
                        gemList.Add(GameObject.Find("Board").GetComponent<BoardManager>().GetGem(i, j));
                    }
                }
            }
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
                    if (fireCheck[new_col, new_row])
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
                if (fireCheck[temp.GetColumn(), temp.GetRow()])
                {
                    continue;
                }

                fireCheck[temp.GetColumn(), temp.GetRow()] = true;
                temp.FireGem();
                StartCoroutine(AfterExplode(6f, temp));
                i++;
            }
        }
    }
}
