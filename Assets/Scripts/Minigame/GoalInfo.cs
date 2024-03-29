using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ERASE!

public class GoalInfo : MonoBehaviour
{
    private BoardManager board;
    private MiniManager mini;

    // ui, sprite
    public SpriteRenderer goalSprite;
    public Sprite[] goal2_sprites;
    public Sprite[] goal3_sprites;

    // goal board
    private int[,] goal2_e = new int[6,2] {{1, 1}, {1, 0}, {0, -1}, {-1, 0}, {-1, 1}, {0, 1}};
    private int[,] goal2_o = new int[6,2] {{1, 0}, {1, -1}, {0, -1}, {-1, -1}, {-1, 0}, {0, 1}};

    private int[,,] goal3_e = new int[33, 2, 2] {{{1, 1},{1, 0}}, {{-1, 0},{0, -1}}, {{-1, 0},{0, 1}}, // 1
                                                 {{1, 1},{2, 0}}, {{-1, 0},{1, 0}}, {{-2, 0},{-1, 1}}, // 2
                                                 {{1, 1},{2, 1}}, {{-1, 0},{1, 1}}, {{-2, -1},{-1, 0}}, // 3
                                                 {{1, 1},{1, 2}}, {{-1, 0},{0, 1}}, {{-1, -1},{0, -1}}, // 4
                                                 {{0, -1},{1, 0}}, {{0, 1},{1, 1}}, {{-1, 1},{-1, 0}}, // 5
                                                 {{1, 0},{2, 0}}, {{-1, 1},{1, 1}}, {{-2, 0},{-1, 0}}, // 6
                                                 {{-1, 0},{-1, -1}}, {{0, -1},{1, 1}}, {{0, 1},{1, 2}}, // 7
                                                 {{1, 0},{2, -1}}, {{-1, 1},{1, 0}}, {{-2, 1},{-1, 1}}, // 8
                                                 {{1, 0},{1, -1}}, {{-1, 1},{0, -1}}, {{-1, 2},{0, 1}}, // 9
                                                 {{0, -1},{1, -1}}, {{0, 1},{1, 0}}, {{-1, 2},{-1, 1}}, // 10
                                                 {{0, -1},{0, -2}}, {{0, 1},{0, -1}}, {{0, 2},{0, 1}} // 11
                                                };
    private int[,,] goal3_o = new int[33, 2, 2] {{{1, 0},{1, -1}}, {{-1, -1},{0, -1}}, {{-1, -1},{0, 1}}, // 1
                                                 {{1, 0},{2, 0}}, {{-1, -1},{1, -1}}, {{-2, 0},{-1, 0}}, // 2
                                                 {{1, 0},{2,1}}, {{-1, -1},{1, 0}}, {{-2, -1},{-1, -1}}, // 3
                                                 {{1, 0},{1, 1}}, {{-1, -1},{0, 1}}, {{-1, -2},{0, -1}}, // 4
                                                 {{0, -1},{1, -1}}, {{0, 1},{1, 0}}, {{-1, 0},{-1, -1}}, // 5
                                                 {{1, -1},{2, 0}}, {{-1, 0},{1, 0}}, {{-2, 0},{-1, -1}}, // 6
                                                 {{-1, -1},{-1, -2}}, {{0, -1},{1, 0}}, {{0, 1},{1, 1}}, // 7
                                                 {{1, -1},{2, -1}}, {{-1, 0},{1, -1}}, {{-2, 1},{-1, 0}}, // 8
                                                 {{1, -1},{1, -2}}, {{-1, 0},{0, -1}}, {{-1, 1},{0, 1}}, // 9
                                                 {{0, -1},{1, -2}}, {{0, 1},{1, -1}}, {{-1, 1},{-1, 0}}, // 10
                                                 {{0, -1},{0, -2}}, {{0, 1},{0, -1}}, {{0, 2},{0, 1}} // 11
                                                };

    // info
    private int goalGemCnt = 2;
    private int goalIdx = 0;

    // crush gems info
    public List<List<int>> crushedGems = new List<List<int>>();

    // DEBUG PURPOSE!!!!
    public Dropdown goalOption;
    //------------------------------

    void Start(){
        board = gameObject.GetComponent<BoardManager>();
        mini = GameObject.Find("MiniManager").GetComponent<MiniManager>();
    }

    public void GoalChange(){
        // change number of goalGemCnt
        //goalGemCnt = goalOption.value+2;
        //board.goalGemCnt = goalGemCnt;
        
        SetGoal(goalGemCnt);
    }

    public int[,] GetGoal()
    {
        int[,] result = new int[2,2];
        if(goalGemCnt == 2)
        {
            result[0, 0] = goal2_e[goalIdx, 0];
            result[0, 1] = goal2_e[goalIdx, 1];
            result[1, 0] = goal2_o[goalIdx, 0];
            result[1, 1] = goal2_o[goalIdx, 1];
        }
        else if(goalGemCnt == 3)
        {
            result = new int[2, 4];
            result[0, 0] = goal3_e[goalIdx, 0, 0];
            result[0, 1] = goal3_e[goalIdx, 0, 1];
            result[0, 2] = goal3_e[goalIdx, 1, 0];
            result[0, 3] = goal3_e[goalIdx, 1, 1];
            result[1, 0] = goal3_o[goalIdx, 0, 0];
            result[1, 1] = goal3_o[goalIdx, 0, 1];
            result[1, 2] = goal3_o[goalIdx, 1, 0];
            result[1, 3] = goal3_o[goalIdx, 1, 1];
        }
        return result;
    }

    public void SetGoal(int unit){
        goalGemCnt = unit;
        if(unit == 2) {
            int prevIdx = goalIdx;
            while(goalIdx == prevIdx){
                goalIdx = Random.Range(0, goal2_e.GetLength(0));
            }
            goalSprite.sprite = goal2_sprites[goalIdx];
        }
        else if(unit == 3) {
            int prevIdx = goalIdx;
            while(goalIdx == prevIdx){
                goalIdx = Random.Range(0, goal3_e.GetLength(0));
            }
            goalSprite.sprite = goal3_sprites[goalIdx];
        }
    }

    public bool CheckGoal(int column, int row){
        // init
        crushedGems = new List<List<int>>();
        crushedGems.Add(new List<int> { column, row });

        if (goalGemCnt == 2) {
            if (column % 2 == 0) {
                int row2 = row + goal2_e[goalIdx, 1];
                int column2 = column + goal2_e[goalIdx, 0];
                if (board.GetGemColor(column, row) == board.GetGemColor(column2, row2))
                {
                    bool result = false;
                    if (mini.patternIdx == (int)PatternType.GREEN && mini.GetComponent<PatternGreen>().IsRunningGimmick(1))
                    {
                        board.BeepPlay();
                        if (mini.GetComponent<PatternGreen>().IsInArea(column, row) && mini.GetComponent<PatternGreen>().IsInArea(column2, row2))
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        result = true;
                    }
                    if (result)
                    {
                        crushedGems.Add(new List<int> { column2, row2 });
                        //if (mini.patternIdx == (int)PatternType.GREEN && mini.GetComponent<PatternGreen>().IsRunningGimmick(1))
                        //{
                        //    mini.GetComponent<PatternGreen>()?.SetAreaAgain();

                        //}
                        return true;
                    }
                }
            }
            else {
                int row2 = row + goal2_o[goalIdx, 1];
                int column2 = column + goal2_o[goalIdx, 0];
                if (board.GetGemColor(column, row) == board.GetGemColor(column2, row2))
                {
                    bool result = false;
                    if (mini.patternIdx == (int)PatternType.GREEN && mini.GetComponent<PatternGreen>().IsRunningGimmick(1))
                    {
                        board.BeepPlay();
                        if (mini.GetComponent<PatternGreen>().IsInArea(column, row) && mini.GetComponent<PatternGreen>().IsInArea(column2, row2))
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        result = true;
                    }
                    if (result)
                    {
                        crushedGems.Add(new List<int> { column2, row2 });
                        //if (mini.patternIdx == (int)PatternType.GREEN && mini.GetComponent<PatternGreen>().IsRunningGimmick(1))
                        //{
                        //    mini.GetComponent<PatternGreen>()?.SetAreaAgain();
                        //}
                        return true;
                    }
                }
            }
        }
        else if (goalGemCnt == 3) {
            bool isCrushed = true;
            bool result = true;
            if (column % 2 == 0) {
                for (int i = 0; i < goal3_e.GetLength(1); i++) {
                    int row2 = row + goal3_e[goalIdx, i, 1];
                    int column2 = column + goal3_e[goalIdx, i, 0];
                    if (mini.patternIdx == (int)PatternType.GREEN && mini.GetComponent<PatternGreen>().IsRunningGimmick(1) && !mini.GetComponent<PatternGreen>().IsInArea(column2, row2)) result = false;
                    if (board.GetGemColor(column, row) != board.GetGemColor(column2, row2))
                    {
                        result = false;
                        isCrushed = false;
                    }
                    crushedGems.Add(new List<int> { column2, row2 });
                }
            }
            else {
                for (int i = 0; i < goal3_o.GetLength(1); i++) {
                    int row2 = row + goal3_o[goalIdx, i, 1];
                    int column2 = column + goal3_o[goalIdx, i, 0];
                    if (mini.patternIdx == (int)PatternType.GREEN && mini.GetComponent<PatternGreen>().IsRunningGimmick(1) && !mini.GetComponent<PatternGreen>().IsInArea(column2, row2)) result = false;
                    if (board.GetGemColor(column, row) != board.GetGemColor(column2, row2))
                    {
                        result = false;
                        isCrushed = false;
                    }
                    crushedGems.Add(new List<int> { column2, row2 });
                }
            }
            if (mini.patternIdx == (int)PatternType.GREEN && mini.GetComponent<PatternGreen>().IsRunningGimmick(1) && !mini.GetComponent<PatternGreen>().IsInArea(column, row))
            {
                result = false;
            }
            if (isCrushed && !result)
            {
                board.BeepPlay();
            }
            //if (result && mini.patternIdx == (int)PatternType.GREEN && mini.GetComponent<PatternGreen>().IsRunningGimmick(1))
            //{
            //    mini.GetComponent<PatternGreen>()?.SetAreaAgain();
            //}
            return result;
        }
        return false;
    } 

    public void EraseGems(int column, int row, bool isCrush){
        board.DelGem(column, row);

        // if is not crush, only one gems erase
        if (!isCrush)
        {
            return;
        }

        if(goalGemCnt == 2){
            if(column%2 == 0){
                int row2 = row + goal2_e[goalIdx, 1];
                int column2 = column + goal2_e[goalIdx, 0];
                board.DelGem(column2, row2);
            }
            else{
                int row2 = row + goal2_o[goalIdx, 1];
                int column2 = column + goal2_o[goalIdx, 0];
                board.DelGem(column2, row2);
            }
        } 
        else if(goalGemCnt == 3){
            if(column%2 == 0){
                for(int i = 0; i < goal3_e.GetLength(1); i++){
                    int row2 = row + goal3_e[goalIdx, i, 1];
                    int column2 = column + goal3_e[goalIdx, i, 0];
                    board.DelGem(column2, row2);
                }
            }
            else{
                for(int i = 0; i < goal3_o.GetLength(1); i++){
                    int row2 = row + goal3_o[goalIdx, i, 1];
                    int column2 = column + goal3_o[goalIdx, i, 0];
                    board.DelGem(column2, row2);
                }
            }
        }
    }
}
