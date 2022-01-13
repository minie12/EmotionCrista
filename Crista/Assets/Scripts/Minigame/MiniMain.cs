using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MiniMain : MonoBehaviour
{
    public GameObject[] org_gem;
    public GameObject tile;
    public Transform[] init_pos;
    public Vector3[] drop_pos;
    public Transform parent_gem;
    public GameObject[,] board_tiles;
    public GemInfo[,] gems;
    public GameObject gem_shine;

    public bool gem_movable;
    private bool gem_clicked;
    private int row, column;

    // animation times
    public float rotate_time;
    public float fall_time;
    
    // goal state
    private int[,] goal_board_o = new int[2, 2] { { -1, -1 }, { -2, 0 } };
    private int[,] goal_board_e = new int[2, 2] { { -1, 0 }, { -2, 0 } };

    void Start()
    {   
        gem_movable = true;
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
        board_tiles = new GameObject[11, 6];
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

                GameObject b_temp = Instantiate(tile, new Vector2(-1.1f + i * diff_x, -1.4f + (i%2)*(-0.45f) + j * diff_y), Quaternion.identity, parent_gem);
                b_temp.name = "(" + i + "," + j + ")";
                board_tiles[i, j] = b_temp;

            }

            drop_pos[i] = new Vector2(-1.1f + i * diff_x, trans_y);
        }

        // get gems inside board
        for(int i = 0; i < 11; i++){
            for(int j = 0; j < 6; j++){
                if(j == 5 && i % 2 == 0) continue; 

                int rand = Random.Range(0, org_gem.Length);
                GameObject gem_temp = Instantiate(org_gem[rand], board_tiles[i,j].transform.position , Quaternion.identity, parent_gem);
                gem_temp.GetComponent<GemInfo>().row = j;
                gem_temp.GetComponent<GemInfo>().column = i;
                gem_temp.GetComponent<GemInfo>().color = rand;
                gems[i, j] = gem_temp.GetComponent<GemInfo>();

            }

            drop_pos[i] = new Vector2(-1.1f + i * diff_x, trans_y);
        }
    }

    void RotateGem(char key)
    {
        int eo = (column%2 == 0)?1:0;

        // turn CCW
        if(key == 'a'){
            gems[column-1, row+eo].MoveGem(column-1, row-1+eo, rotate_time);
            gems[column-1, row-1+eo].MoveGem(column, row-1, rotate_time);
            gems[column, row-1].MoveGem(column+1, row-1+eo, rotate_time);
            gems[column+1, row-1+eo].MoveGem(column+1, row+eo, rotate_time);
            gems[column+1, row+eo].MoveGem(column, row+1, rotate_time);
            gems[column, row+1].MoveGem(column-1, row+eo, rotate_time);

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
            gems[column-1, row+eo].MoveGem(column, row+1, rotate_time);
            gems[column, row+1].MoveGem(column+1, row+eo, rotate_time);
            gems[column+1, row+eo].MoveGem(column+1, row-1+eo, rotate_time);
            gems[column+1, row-1+eo].MoveGem(column, row-1, rotate_time);
            gems[column, row-1].MoveGem(column-1, row-1+eo, rotate_time);
            gems[column-1, row-1+eo].MoveGem(column-1, row+eo, rotate_time);

            GemInfo g_temp = gems[column-1, row+eo];
            gems[column-1, row+eo] = gems[column-1, row-1+eo];
            gems[column-1, row-1+eo] = gems[column, row-1];
            gems[column, row-1] = gems[column+1, row-1+eo];
            gems[column+1, row-1+eo] = gems[column+1, row+eo];
            gems[column+1, row+eo] = gems[column, row+1];
            gems[column, row+1] = g_temp;
        }
    }

    public void GemClick(int column_, int row_, Transform pos)
    {
        row = row_; column = column_;

        // check if goal is met
        if(GoalCheck()) {
            StartCoroutine(RefillBoard());
            return;
        }

        // do not enable click when user clicks the boundary of board
        if(column_ == 10 || column_ == 0 || row_ == 5 || row_ == 0) return;
        if(row_ == 4 && column_ % 2 == 0) return;

        // show that gem has been clicked
        gem_clicked = true;
        gem_shine.transform.position = pos.position;
        gem_shine.SetActive(true);

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

        // satisfies the goal!!
        gem_movable = false;
        gem_clicked = false;
        gem_shine.SetActive(false);

        Debug.Log("POP");
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

        return true;
    }

    IEnumerator RefillBoard(){
        yield return new WaitForSeconds(0.3f);

        int null_cnt = 0;
        bool check_pop = true;
        // fill with already existing gems
        for(int i = row; i < 6; i++){
            if(column % 2 == 0 && i == 5) break; 

            if(gems[column, i] == null && check_pop) {
                null_cnt++;
            }
            else{
                check_pop = false;
                gems[column, i-null_cnt] = gems[column, i];
                gems[column, i-null_cnt].MoveGem(column, i-null_cnt, fall_time);
            }
        }

        for(int i = 0; i < null_cnt; i++){
            int rand = Random.Range(0, org_gem.Length);
            GameObject gem_temp = Instantiate(org_gem[rand], drop_pos[column], Quaternion.identity, parent_gem);
            gem_temp.GetComponent<GemInfo>().color = rand;
            int row_ = (column%2==0)?4:5;
            yield return new WaitForSeconds(0.1f);
            gems[column, row_ - i] = gem_temp.GetComponent<GemInfo>();
            gems[column, row_ - i].MoveGem(column, (row_-i), fall_time);
        }

        gem_movable = true;
    }

    // used to move to another scene
    public void SceneTransfer()
    {
        string nowbutton = EventSystem.current.currentSelectedGameObject.name;
        SceneManager.LoadScene(nowbutton, LoadSceneMode.Single);
    }
}
