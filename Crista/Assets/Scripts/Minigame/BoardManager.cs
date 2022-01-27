using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class BoardManager : MonoBehaviour
{
    private MiniManager mini;
    public int goal_color;

    public GameObject[] org_gem;
    public GameObject tile;
    public Vector3[] drop_pos;
    public GameObject click_effect;
    private GemInfo[,] gems;
    private Vector3[,] board_tiles;


    public bool gem_movable;
    private bool gem_clicked;
    private int row, column;
    private float rotate_ang = 0;

    // fever related
    public bool fever_on;
    private int fever_cnt = 0;

    // animation times
    public float rotate_time;
    public float fall_time;
    
    // goal state
    private int[,] goal_board_o = new int[2, 2] { { -1, -1 }, { -2, 0 } };
    private int[,] goal_board_e = new int[2, 2] { { -1, 0 }, { -2, 0 } };

    void Start()
    {   
        mini = GameObject.Find("MiniManager").GetComponent<MiniManager>();
        gem_movable = true;
        fever_on = false;
        InitBoard();
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

                //GameObject b_temp = Instantiate(tile, new Vector2(-1.2f + i * diff_x, -1.7f + (i%2)*(-0.45f) + j * diff_y), Quaternion.identity, this.transform);
                //b_temp.name = "(" + i + "," + j + ")";
                board_tiles[i, j] = new Vector3(-1.2f + i * diff_x, -1.7f + (i%2)*(-0.45f) + j * diff_y, 0);

                int rand = Random.Range(0, org_gem.Length);
                // GameObject gem_temp = Instantiate(org_gem[rand], board_tiles[i,j].transform.position , Quaternion.identity, this.transform);
                GameObject gem_temp = Instantiate(org_gem[rand], board_tiles[i,j], Quaternion.identity, this.transform);
                gem_temp.GetComponent<GemInfo>().row = j;
                gem_temp.GetComponent<GemInfo>().column = i;
                gem_temp.GetComponent<GemInfo>().color = rand;
                gems[i, j] = gem_temp.GetComponent<GemInfo>();

            }

            drop_pos[i] = new Vector2(-1.1f + i * diff_x, trans_y);
        }

        //InitGem();
    }

    void InitGem(){
        // get gems inside board
        for(int i = 0; i < 11; i++){
            for(int j = 0; j < 6; j++){
                if(j == 5 && i % 2 == 0) continue; 

                int rand = Random.Range(0, org_gem.Length);
                // GameObject gem_temp = Instantiate(org_gem[rand], board_tiles[i,j].transform.position , Quaternion.identity, this.transform);
                GameObject gem_temp = Instantiate(org_gem[rand], board_tiles[i,j], Quaternion.identity, this.transform);
                gem_temp.GetComponent<GemInfo>().row = j;
                gem_temp.GetComponent<GemInfo>().column = i;
                gem_temp.GetComponent<GemInfo>().color = rand;
                gems[i, j] = gem_temp.GetComponent<GemInfo>();

            }
        }
    }

    public Vector3 GetPosition(int column_, int row_){
        return board_tiles[column_, row_];
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
            StartCoroutine(ClickEffectRotate(rotate_time));

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
            StartCoroutine(ClickEffectRotate(rotate_time));

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

    IEnumerator ClickEffectRotate(float time)
    {
        Quaternion start_rot = click_effect.transform.rotation;
        yield return new WaitForSeconds(0.05f); // used to prevent many accesses to GetPosition() at the same time
        Quaternion end_rot = Quaternion.Euler(0, 0, rotate_ang);

        for(float t = 0; t <= 1 * time; t += Time.deltaTime){
            // Dampen towards the target rotation
            click_effect.transform.rotation = Quaternion.Slerp(start_rot, end_rot, t / time);
            yield return 0;
        }
            
        click_effect.transform.rotation = end_rot;
    }

    public void GemClick(int column_, int row_)
    {
        row = row_; column = column_;

        // check if goal is met
        if(GoalCheck()) {
            gem_movable = false;
            gem_clicked = false;
            click_effect.SetActive(false);
            if(gems[column,row].color == goal_color) mini.AddScore(goal_board_e.GetLength(0)+1);
            else mini.AddFever(goal_board_e.GetLength(0)+1);

            // Delete gems
            gems[column, row].DestroyGem();
            gems[column, row] = null;
            if (column % 2 == 0)
            {
                for (int i = 0; i < 2; i++) {
                    int c = column + goal_board_e[i, 0];
                    int r = row + goal_board_e[i, 1];

                    gems[c, r].DestroyGem();
                    gems[c, r] = null;
                }
            }
            else
            {
                for (int i = 0; i < 2; i++) {
                    int c = column + goal_board_o[i, 0];
                    int r = row + goal_board_o[i, 1];

                    gems[c, r].DestroyGem();
                    gems[c, r] = null;
                }
            }

            StartCoroutine(RefillBoard());
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

    bool GoalCheck()
    {
        if (column % 2 == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                int c = column + goal_board_e[i, 0];
                int r = row + goal_board_e[i, 1];

                if (c < 0 || c > 10 || r < 0 || r > 5) return false;
                if (r == 5 && c%2 == 0) return false;

                if (gems[column, row].color != gems[c, r].color) return false;
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                int c = column + goal_board_o[i, 0];
                int r = row + goal_board_o[i, 1];

                if (c < 0 || c > 10 || r < 0 || r > 5) return false;
                if (r == 5 && c%2 == 0) return false;

                if (gems[column, row].color != gems[c, r].color) return false;
            }
        }

        return true;
    }

    bool CheckGemExist(int column_, int row_){
        if(column_ < 0 || column_ > 10 || row_ > 5 || row_ < 0) return false;
        if(column_ % 2 == 0 && row_ > 4) return false;
        if(gems[column_, row_] == null) return false;
        return true;
    }

    IEnumerator RefillBoard(){
        yield return new WaitForSeconds(0.3f);

        // fill with already existing gems
        for(int k = -1; k < goal_board_e.GetLength(0);  k++){
            int null_cnt = 0;

            int column_ = column;
            int row_ = row;
            
            if(k != -1){
                if(column_ % 2 == 0){
                    column_ += goal_board_e[k, 0];
                    row_ += goal_board_e[k, 1];
                }
                else{
                    column_ += goal_board_o[k, 0];
                    row_ += goal_board_o[k, 1];
                }
            }

            for(int i = row_; i < 6; i++){
                if(column_ % 2 == 0 && i == 5) break; 

                if(gems[column_, i] == null) {
                    if(!CheckGemExist(column_, i+1)) null_cnt++;
                    else{
                        gems[column_, i-null_cnt] = gems[column_, i+1];
                        gems[column_, i+1] = null;
                        gems[column_, i-null_cnt].MoveGem(column_, i-null_cnt, fall_time);
                    }
                }
                else null_cnt = 0;
            }

            for(int i = 0; i < null_cnt; i++){
                // yield return new WaitForSeconds(0.1f);

                // int rand = Random.Range(0, org_gem.Length);
                // row_ = (column_%2==0)?4:5;
                // row_ -= i;
                // Debug.Log("row: " + row_);
                // GameObject gem_temp = Instantiate(org_gem[rand], board_tiles[column_, row_].transform.position , Quaternion.identity, this.transform);
                // gem_temp.GetComponent<GemInfo>().row = row_;
                // gem_temp.GetComponent<GemInfo>().column = column_;
                // gem_temp.GetComponent<GemInfo>().color = rand;
                // gems[column_, row_] = gem_temp.GetComponent<GemInfo>();


                int rand = Random.Range(0, org_gem.Length);
                GameObject gem_temp = Instantiate(org_gem[rand], drop_pos[column_], Quaternion.identity, this.transform);
                gem_temp.GetComponent<GemInfo>().color = rand;
                row_ = (column_%2==0)?4:5;
                // yield return new WaitForSeconds(0.1f);
                gems[column_, row_ - i] = gem_temp.GetComponent<GemInfo>();
                gems[column_, row_ - i].MoveGem(column_, (row_-i), fall_time);
            }
        }

        gem_movable = true;
    }


    // fever
    public void StartFever(){
        fever_on = true;
        fever_cnt = 0;
        click_effect.SetActive(false);
        gem_clicked = false;
    }

    void EndFever(){
        fever_on = false;
        InitGem();
        mini.EndFever();
    }

    public void FeverClick(int column_, int row_){
        mini.AddScore(1);
        gems[column_, row_].DestroyGem();
        gems[column_, row_] = null;
        fever_cnt++;

        if(fever_cnt > 59) EndFever();
    }
}
