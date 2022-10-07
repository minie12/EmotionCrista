using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ERASE!

public class GoalInfo : MonoBehaviour
{
    private BoardManager board;

    // ui, sprite
    public SpriteRenderer goal_sprite;
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
    private int goal_unit = 2;
    private int goal_num = 0;

    // crush gems info
    public List<List<int>> crushedGems = new List<List<int>>();

    // DEBUG PURPOSE!!!!
    public Dropdown goalOption;
    //------------------------------

    void Start(){
        board = gameObject.GetComponent<BoardManager>();
    }

    public void GoalChange(){
        // change number of goal_unit
        //goal_unit = goalOption.value+2;
        //board.goal_unit = goal_unit;
        
        SetGoal(goal_unit);
    }

    public void SetGoal(int unit){
        goal_unit = unit;
        if(unit == 2) {
            int prev_num = goal_num;
            while(goal_num == prev_num){
                goal_num = Random.Range(0, goal2_e.GetLength(0));
            }
            goal_sprite.sprite = goal2_sprites[goal_num];
        }
        else if(unit == 3) {
            int prev_num = goal_num;
            while(goal_num == prev_num){
                goal_num = Random.Range(0, goal3_e.GetLength(0));
            }
            goal_sprite.sprite = goal3_sprites[goal_num];
        }
    }

    public bool CheckGoal(int column, int row){
        // init
        crushedGems = new List<List<int>>();
        crushedGems.Add(new List<int> { column, row });

        if (goal_unit == 2){
            if(column%2 == 0){
                int row2 = row + goal2_e[goal_num, 1];
                int column2 = column + goal2_e[goal_num, 0];
                if (board.GetGemColor(column, row) == board.GetGemColor(column2, row2))
                {
                    crushedGems.Add(new List<int> { column2, row2 });
                    return true;
                }
            }
            else{
                int row2 = row + goal2_o[goal_num, 1];
                int column2 = column + goal2_o[goal_num, 0];
                if(board.GetGemColor(column, row) == board.GetGemColor(column2, row2))
                {
                    crushedGems.Add(new List<int> { column2, row2 });
                    return true;
                }
            }
        } 
        else if(goal_unit == 3){
            if(column%2 == 0){
                for(int i = 0; i < goal3_e.GetLength(1); i++){
                    int row2 = row + goal3_e[goal_num, i, 1];
                    int column2 = column + goal3_e[goal_num, i, 0];
                    if(board.GetGemColor(column, row) != board.GetGemColor(column2, row2)) return false;
                    crushedGems.Add(new List<int> { column2, row2 });
                }
            }
            else{
                for(int i = 0; i < goal3_o.GetLength(1); i++){
                    int row2 = row + goal3_o[goal_num, i, 1];
                    int column2 = column + goal3_o[goal_num, i, 0];
                    if(board.GetGemColor(column, row) != board.GetGemColor(column2, row2)) return false;
                    crushedGems.Add(new List<int> { column2, row2 });
                }
            }
            return true;
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

        if(goal_unit == 2){
            if(column%2 == 0){
                int row2 = row + goal2_e[goal_num, 1];
                int column2 = column + goal2_e[goal_num, 0];
                board.DelGem(column2, row2);
            }
            else{
                int row2 = row + goal2_o[goal_num, 1];
                int column2 = column + goal2_o[goal_num, 0];
                board.DelGem(column2, row2);
            }
        } 
        else if(goal_unit == 3){
            if(column%2 == 0){
                for(int i = 0; i < goal3_e.GetLength(1); i++){
                    int row2 = row + goal3_e[goal_num, i, 1];
                    int column2 = column + goal3_e[goal_num, i, 0];
                    board.DelGem(column2, row2);
                }
            }
            else{
                for(int i = 0; i < goal3_o.GetLength(1); i++){
                    int row2 = row + goal3_o[goal_num, i, 1];
                    int column2 = column + goal3_o[goal_num, i, 0];
                    board.DelGem(column2, row2);
                }
            }
        }
    }
}
