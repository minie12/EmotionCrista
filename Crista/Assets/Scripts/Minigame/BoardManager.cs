using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BoardManager : MonoBehaviour
{
    public MiniManager mini;
    public int goal_color; // 0 yellow, 1 blue, 2 red, 3 green, 4 purple
    private int gem_numb = 5; // need to change if gem color numb changes

    public GameObject gemPF;
    // public GameObject[] gemPF;
    public Vector3[] drop_pos;
    public GameObject click_effect;
    private GemInfo[,] gems;
    private Vector3[,] board_tiles;


    public bool gem_movable;
    private bool gem_clicked;
    private int row, column;
    private float rotate_ang = 0;

    // fever related
    private int fever_cnt = 0;

    // animation times
    public float rotate_time;
    public float fall_time;
    
    // goal state
    public GoalInfo goal_info;
    public int goal_unit = 2;
    

    void Start()
    {   
        // mini = GameObject.Find("MiniManager").GetComponent<MiniManager>();
        // goal_info = gameObject.GetComponent<GoalInfo>();
        gem_movable = true;
        InitBoard();
        // for testing purpose ERASE BELOW
        goal_info.SetGoal(goal_unit);
    }

    void Update()
    {
        if (gem_clicked)
        {
            if (Input.GetKeyDown(KeyCode.A)) RotateGem('a');
            else if (Input.GetKeyDown(KeyCode.D)) RotateGem('d');
        }
    }

    void InitBoard()
    {
        board_tiles = new Vector3[11, 6];
        gems = new GemInfo[11, 6];
        drop_pos = new Vector3[11];

        // create 66 gems on correct location
        float diff_x = 0.75f;
        float diff_y = 0.9f;
        float trans_y = 3.35f;

        // set board tile (hexagon tile)
        for(int i = 0; i < 11; i++){
            for(int j = 0; j < 6; j++){
                if(j == 5 && i % 2 == 0) continue; 

                board_tiles[i, j] = new Vector3(-1.2f + i * diff_x, -1.7f + (i%2)*(-0.45f) + j * diff_y, 0);

                int color = Random.Range(0, gem_numb);
                GameObject gem_temp = Instantiate(gemPF, board_tiles[i,j], Quaternion.identity, this.transform);
                gem_temp.GetComponent<GemInfo>().InitGem(i, j, color);
                gems[i, j] = gem_temp.GetComponent<GemInfo>();

            }

            drop_pos[i] = new Vector2(-1.1f + i * diff_x, trans_y);
        }
    }

    void RefillGem(){ // NOT USED!!!!!
        // get gems inside board
        for(int i = 0; i < 11; i++){
            for(int j = 0; j < 6; j++){
                if(j == 5 && i % 2 == 0) continue; 

                int color = Random.Range(0, gem_numb);
                GameObject gem_temp = Instantiate(gemPF, board_tiles[i,j], Quaternion.identity, this.transform);
                gem_temp.GetComponent<GemInfo>().InitGem(i, j, color);
                gems[i, j] = gem_temp.GetComponent<GemInfo>();
            }
        }
    }


    void RotateGem(char key)
    {
        int eo = (column%2 == 0)?1:0;

        // turn CCW
        if(key == 'a'){
            // rotate gameobjects
            gems[column-1, row+eo].MoveGem(column-1, row-1+eo, rotate_time);
            gems[column-1, row-1+eo].MoveGem(column, row-1, rotate_time);
            gems[column, row-1].MoveGem(column+1, row-1+eo, rotate_time);
            gems[column+1, row-1+eo].MoveGem(column+1, row+eo, rotate_time);
            gems[column+1, row+eo].MoveGem(column, row+1, rotate_time);
            gems[column, row+1].MoveGem(column-1, row+eo, rotate_time);

            rotate_ang += 60;
            StartCoroutine(ClickEffectRotateC(rotate_time));

            // update gems array
            GemInfo g_temp = gems[column-1, row+eo];
            gems[column-1, row+eo] = gems[column, row+1];
            gems[column, row+1] = gems[column+1, row+eo];
            gems[column+1, row+eo] = gems[column+1, row-1+eo];
            gems[column+1, row-1+eo] = gems[column, row-1];
            gems[column, row-1] = gems[column-1, row-1+eo];
            gems[column-1, row-1+eo] = g_temp;
        } 
        
        // turn CW
        else{
            // rotate gameobjects
            gems[column-1, row+eo].MoveGem(column, row+1, rotate_time);
            gems[column, row+1].MoveGem(column+1, row+eo, rotate_time);
            gems[column+1, row+eo].MoveGem(column+1, row-1+eo, rotate_time);
            gems[column+1, row-1+eo].MoveGem(column, row-1, rotate_time);
            gems[column, row-1].MoveGem(column-1, row-1+eo, rotate_time);
            gems[column-1, row-1+eo].MoveGem(column-1, row+eo, rotate_time);

            rotate_ang -= 60;
            StartCoroutine(ClickEffectRotateC(rotate_time));

            // update gems array
            GemInfo g_temp = gems[column-1, row+eo];
            gems[column-1, row+eo] = gems[column-1, row-1+eo];
            gems[column-1, row-1+eo] = gems[column, row-1];
            gems[column, row-1] = gems[column+1, row-1+eo];
            gems[column+1, row-1+eo] = gems[column+1, row+eo];
            gems[column+1, row+eo] = gems[column, row+1];
            gems[column, row+1] = g_temp;
        }
    }

    IEnumerator ClickEffectRotateC(float time)
    {
        gem_movable = false;

        Quaternion start_rot = click_effect.transform.rotation;
        yield return new WaitForSeconds(0.05f); // used to prevent many accesses to GetPosition() at the same time
        Quaternion end_rot = Quaternion.Euler(0, 0, rotate_ang);

        for(float t = 0; t <= 1 * time; t += Time.deltaTime){
            // Dampen towards the target rotation
            click_effect.transform.rotation = Quaternion.Slerp(start_rot, end_rot, t / time);
            yield return 0;
        }
            
        click_effect.transform.rotation = end_rot;

        gem_movable = true;
    }

    public void GemClick(int column_, int row_)
    {
        row = row_; column = column_;

        // check if goal is met
        if(goal_info.CheckGoal(column, row)){
            gem_clicked = false;
            click_effect.SetActive(false);

            // add score if gem color is the goal color
            if(gems[column,row].GetColor() == goal_color) mini.AddScore(goal_unit);
            else mini.AddFever(goal_unit);

            // Delete gems
            goal_info.EraseGems(column, row);

            StartCoroutine("RefillBoard");
            // Invoke("RefillBoard", 0.3f);
        }
        else{
            // do not enable click when user clicks the boundary of board
            if(column_ == 10 || column_ == 0 || row_ == 5 || row_ == 0) return;
            if(row_ == 4 && column_ % 2 == 0) return;

            // show that gem has been clicked
            gem_clicked = true;
            click_effect.transform.position = board_tiles[column, row];
            click_effect.SetActive(true);
        }
    }

    bool CheckGemExist(int column_, int row_){
        if(column_ < 0 || column_ > 10 || row_ > 5 || row_ < 0) return false;
        if(column_ % 2 == 0 && row_ > 4) return false;
        if(gems[column_, row_] == null) return false;
        return true;
    }

    IEnumerator RefillBoard(){
        yield return new WaitForSeconds(0.3f); // wait for gem crush

        gem_movable = false;
        
        int start = column-goal_unit > 0? column-goal_unit : 0;
        int end = column+goal_unit < 11? column+goal_unit : 11;
        for(int i = start; i < end; i++){
            for(int j = (row-goal_unit > 0? row-goal_unit : 0); j < 6; j++){
                if(i % 2 == 0 && j == 5) break; 
                
                if(!CheckGemExist(i, j)){
                    bool filled = false;
                    // check if there is gem on top
                    for(int k = j; k < 6; k++){
                        if(i % 2 == 0 && k == 5) break; 

                        if(CheckGemExist(i, k)){
                            // drop the gem on top to bottom
                            gems[i, j] = gems[i,k];
                            gems[i, k] = null;
                            gems[i, j].MoveGem(i, j, fall_time);
                            filled = true;
                            break;
                        }
                    }

                    // // if there was no gem on top
                    if(!filled){
                        // fill with new gem
                        int color = Random.Range(0, gem_numb);
                        GameObject gem_temp = Instantiate(gemPF, drop_pos[i], Quaternion.identity, this.transform);
                        gem_temp.GetComponent<GemInfo>().InitGem(i, j, color);
                        // yield return new WaitForSeconds(0.1f);
                        gems[i, j] = gem_temp.GetComponent<GemInfo>();
                        gems[i, j].MoveGem(i, j, fall_time);
                    }
                }
            }
        }

        // wait for gems to fall down then allow clicks
        yield return new WaitForSeconds(fall_time);
        gem_movable = true;
    }

 // used to communicate with other classes ---------------------------------------------------
    public void SetGoal(int unit){
        goal_unit = unit;
        goal_info.SetGoal(unit);
    }
    public void DelGem(int column_, int row_){
        gems[column_, row_].DestroyGem();
        gems[column_, row_] = null;
    }
    public Vector3 GetGemPosition(int column_, int row_){
        return board_tiles[column_, row_];
    }
    public int GetGemColor(int column_, int row_){
        if(!CheckGemExist(column_, row_)) return -1;
        return gems[column_, row_].GetColor();
    }

    public bool CheckFever(){
        return mini.fever_on;
    }

// FEVER ---------------------------------------------------------------------------
    public void StartFever(){
        fever_cnt = 0;
        click_effect.SetActive(false);
        gem_clicked = false;
    }

    IEnumerator RefillBoardFever(){
        yield return new WaitForSeconds(0.3f); // wait for gem crush
        
        gem_movable = false;
        
        for(int i = 0; i < 11; i++){
            for(int j = 0; j < 6; j++){
                if(i % 2 == 0 && j == 5) break; 
                
                if(!CheckGemExist(i, j)){
                    bool filled = false;
                    // check if there is gem on top
                    for(int k = j; k < 6; k++){
                        if(i % 2 == 0 && k == 5) break; 

                        if(CheckGemExist(i, k)){
                            // drop the gem on top to bottom
                            gems[i, j] = gems[i,k];
                            gems[i, k] = null;
                            gems[i, j].MoveGem(i, j, fall_time);
                            filled = true;
                            break;
                        }
                    }

                    // // if there was no gem on top
                    if(!filled){
                        // fill with new gem
                        int color = Random.Range(0, gem_numb);
                        GameObject gem_temp = Instantiate(gemPF, drop_pos[i], Quaternion.identity, this.transform);
                        gem_temp.GetComponent<GemInfo>().InitGem(i, j, color);
                        // yield return new WaitForSeconds(0.1f);
                        gems[i, j] = gem_temp.GetComponent<GemInfo>();
                        gems[i, j].MoveGem(i, j, fall_time);
                    }
                }
            }
        }

        // wait for gems to fall down then allow clicks
        yield return new WaitForSeconds(fall_time);
        gem_movable = true;
    }

    public void EndFever(){
        StartCoroutine("RefillBoardFever");
        // Invoke("RefillBoardFever",0.5f);
    }

    public void FeverClick(int column_, int row_){
        mini.AddScore(1);
        DelGem(column_, row_);

        // in case player clicks all gem before Fever ends
        fever_cnt++;
        if(fever_cnt > 59) mini.EndFever();
    }
}
