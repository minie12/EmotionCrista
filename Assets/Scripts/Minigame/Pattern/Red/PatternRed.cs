using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternRed : PatternManager
{
    // check gimmick 0 is running
    private bool isRunning = false;

    // red around gems direction vector (even/odd standard: col)
    private int[,] aroundGem_o = new int[6, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 } };
    private int[,] aroundGem_e = new int[6, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

    // Setting gimmick
    override public void StartPattern(int gimmick_)
    {
        gimmick = gimmick_;
        OrganizeCharacterChat();

        if (gimmick == 0)
        {
            isRunning = true;
            StartCoroutine(RedExplosion());
        }
    }

    override public void StopPattern() { isRunning = false; }
    override public void RestartPattern()
    {
        if (gimmick == 0)
        {
            isRunning = true;
            StartCoroutine(RedExplosion());
        }
    }

    // Red gimmick 0
    IEnumerator RedExplosion(){
        while (isRunning)
        {

            yield return null;
        }
    }


    public void ExplosionAroundGems()
    {
        Debug.Log("gimmick start!");

        int[,] direction = new int[6, 2];

        // crushed gems info in GoalInfo.cs
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

        bool[,] aroundGems = new bool[11, 6];

        // extract without overlap gems
        for (int i = 0; i < crushedGems.Count; i++)
        {
            int cur_col = crushedGems[i][0];
            int cur_row = crushedGems[i][1];

            // choose direction vector about even or odd column
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
                // if around is crushed then continue
                if (crushedCheck[new_col, new_row])
                {
                    continue;
                }
                

                aroundGems[new_col, new_row] = true;
            }
        }

        // one: 80%, two: 20%
        int[] percentage = new int[10];
        for (int i = 0; i < 8; i++)
        {
            percentage[i] = 6;
        }
        for (int i = 8; i < 10; i++)
        {
            percentage[i] = 2;
        }

        int rand = Random.Range(0, 10);
        int explosionNum = percentage[rand];

        // extract around gems for 2nd list
        List<List<int>> aroundGems_ = new List<List<int>>();
        for (int i = 0; i <= 10; i++)
        {
            for (int j = 0; j <= 5; j++)
            {
                if(j==5 && i % 2 == 0)
                {
                    break;
                }
                if (aroundGems[i, j])
                {
                    aroundGems_.Add(new List<int> { i, j });
                }
            }
        }

        // check already explosion
        bool[,] explosionCheck = new bool[11, 6];

        // choose min value 
        explosionNum = System.Math.Min(explosionNum, aroundGems_.Count);

        // choose random explosion gem
        for (int i = 0; i < explosionNum;)
        {
            rand = Random.Range(0, aroundGems_.Count);
            int column_ = aroundGems_[rand][0];
            int row_ = aroundGems_[rand][1];

            if (explosionCheck[column_,row_])
            {
                continue;
            }

            Debug.Log("Æø¹ßÇÏ´Â ±¤¹°: " + column_ + ", " + row_);
            explosionCheck[column_,row_] = true;
            GameObject.Find("Board").GetComponent<GoalInfo>().EraseGems(column_, row_, false);
            i++;
        }

    }

    // Red gimmick 0
    public void RedExplosionTest()
    {
        Invoke("ExplosionAroundGems", 0.2f);

    }

    public void Explosion()
    {

    }
}
