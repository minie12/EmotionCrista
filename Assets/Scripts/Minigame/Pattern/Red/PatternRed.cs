using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternRed : PatternManager
{
    // red around gems direction vector (odd/even standard: col)
    private int[,] aroundGem_o = new int[6, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 } };
    private int[,] aroundGem_e = new int[6, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

    // Setting gimmick
    override public void StartPattern(int gimmick_)
    {
        gimmick = gimmick_;
        OrganizeCharacterChat();
    }

    override public void StopPattern() {}
    override public void RestartPattern() {}

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
            GameObject.Find("Board").GetComponent<GoalInfo>().EraseGems(column_, row_, false);
            i++;
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
}
