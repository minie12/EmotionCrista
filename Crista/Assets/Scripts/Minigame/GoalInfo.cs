using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //private int[,,] goal3_e = new int[33, 2, 2] {{{1, 1},{1, 0}}, {{-1, 0},{0, -1}}, {{-1, 0},{0, 1}},
                                                //  {{1, 1},{2, 0}}, {{-1, 0,},{1, 0}}, {{-2, 0},{-1, 1}},
                                                //  {{1, 1},{2,1}}, {{-1, 0},{1, 1}}, {{-2, -1},{-1, 0}}
                                                // };
    //private int[,,] goal3_o = new int[] {};

    // info
    private int goal_unit = 2;
    private int goal_num = 0;

    // DEBUG PURPOSE!!!!
    public void GoalChange(){
        SetGoal(goal_unit);
    }



    void Start(){
        board = GameObject.Find("Board").GetComponent<BoardManager>();
    }

    public void SetGoal(int unit){
        goal_unit = unit;
        if(unit == 2) {
            goal_num = Random.Range(0, goal2_e.GetLength(0));
            goal_sprite.sprite = goal2_sprites[goal_num];
        }
        // else if(unit == 3) {
        //     goal_num = Random.Range(0, goal3_e.GetLength(0));
        //     goal_sprite.sprite = goal3_sprites[goal_num];
        // }

    }

    public bool CheckGoal(int column, int row){
        if(goal_unit == 2){
            if(column%2 == 0){
                int row2 = row + goal2_e[goal_num, 1];
                int column2 = column + goal2_e[goal_num, 0];
                if(board.GetGemColor(column, row) == board.GetGemColor(column2, row2)) return true;
            }
            else{
                int row2 = row + goal2_o[goal_num, 1];
                int column2 = column + goal2_o[goal_num, 0];
                if(board.GetGemColor(column, row) == board.GetGemColor(column2, row2)) return true;
            }
        } 
        // else if(goal_unit == 3){
        //     if(column%2 == 0){
        //         for(int i = 0; i < goal3_e.GetLength(1); i++){
        //             int row2 = row + goal3_e[goal_num][i][1];
        //             int column2 = column + goal3_e[goal_num][i][0];
        //             if(board.GetGemColor(column, row) == board.GetGemColor(column2, row2)) return true;
        //         }
        //     }
        //     else{
        //         for(int i = 0; i < goal3_o.GetLength(1); i++){
        //             int row2 = row + goal3_o[goal_num][i][1];
        //             int column2 = column + goal3_o[goal_num][i][0];
        //             if(board.GetGemColor(column, row) == board.GetGemColor(column2, row2)) return true;
        //         }
        //     }
        // }

        return false;
    } 

    public void EraseGems(int column, int row){
        board.DelGem(column, row);

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
    }
}
