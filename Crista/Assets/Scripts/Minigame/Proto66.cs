using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class Proto66 : MonoBehaviour
{
    const int RED = 0;
    const int YELLOW = 1;
    const int GREEN = 2;
    const int BLUE = 3;
    const int PURPLE = 4;

    private int[] gems = new int[66];
    public GameObject[] gems_obj;
    public GameObject gem_clicked;
    public Text[] score;

    private Color[] gem_color = new Color[5] { Color.red, Color.yellow, Color.green, new Color(0, 1, 1), new Color(0.64f, 0.1f, 1) };
    private int[] no_turn = new int[29] { 0, 1, 2, 3, 4, 5, 6, 7,8,9,10,11, 18, 23, 30, 35, 42, 47, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64};
    private int[,,] goal_shape3 = new int[3,2,3] {{{0, -12, -5,},{0, -12, -6,}}, {{-1, 6, 0},{-1,5,0}}, {{-12,0,6},{-12,0,5}}};
    private int[,,] goal_shape4 = new int[3,2,4] {{{-12,-5,0,6},{-12,-6,0,5}}, {{12,0,-5,1},{12,0,-6,1}}, {{5,-1,6,0},{4,-1,5,0}}};
    private int[,,] goal_shape5 = new int[3,2,5] {{{0,7,11,12,18},{0,6,11,12,17}}, {{-7,-13,-18,-12,0},{-8,-13,-19,-12,0}}, {{12,6,0,-5,1},{12,5,0,-6,1}}};

    private int[,,] goal_shape;

    private bool clicked = false;
    private bool mutex_pop = false;
    private int gem_num = -1;

    // goal gem related
    public GameObject goal_sprite;
    public Sprite[] goalSprites;
    private int goal_num = 0;

    private int timer = 1;
    private int goal_choice = 0;
    private int fever_count = 0;
    private bool fever = false;


    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("goalNum")) goal_num = PlayerPrefs.GetInt("goalNum");

        switch(goal_num){
            case 0:
                goal_shape = goal_shape3;
                break;
            case 1:
                goal_shape = goal_shape4;
                break;
            case 2:
                goal_shape = goal_shape5;
                break;
        }
         
        // set goal_shape
        goal_choice = Random.Range(0, 3);
        goal_sprite.GetComponent<SpriteRenderer>().sprite = goalSprites[3*goal_num+goal_choice];

        // display gems 
        for(int i = 0; i < 11; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                if (j == 5 && i % 2 == 0)
                {
                    gems[i * 6 + j] = -1;
                    break;
                }

                int color = Random.Range(0, 5);
                gems[i*6 + j] = color;
                gems_obj[i * 6 + j].GetComponent<SpriteRenderer>().color = gem_color[color];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(timer % 10 == 0){
            goal_choice = Random.Range(0, 3);
            goal_sprite.GetComponent<SpriteRenderer>().sprite = goalSprites[3*goal_num+goal_choice];
            timer++;
        }

        if (Input.GetMouseButtonDown(0))  // if get mouse click
        {
            timer++;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)  // if mouse clicks on object
            {
                if(fever){
                    if(hit.collider.gameObject.tag == "Gem"){
                        gem_num = int.Parse(hit.collider.gameObject.name);
                        StartCoroutine(pop_gems(gem_num));
                    }
                }
                else if(hit.collider.gameObject.tag == "Gem" && !mutex_pop){
                    gem_num = int.Parse(hit.collider.gameObject.name);
                    int row_n = (gem_num / 6) % 2;
                    //Debug.Log("hit: " + gem_num);

                    // check if same as goal shape
                    bool all_same = true;
                    for (int i = 1; i < goal_shape.GetLength(2); i++)
                    {
                        if (gem_num + goal_shape[goal_choice,row_n,i] < 0 || gem_num + goal_shape[goal_choice,row_n,i] > 64)
                        {
                            all_same = false;
                            break;
                        }
                        if (gems[gem_num] != gems[gem_num + goal_shape[goal_choice,row_n,i]]){
                            all_same = false;
                            break;
                        }
                    }
                    if (all_same){
                        if(gems[gem_num] != RED) fever_count++;

                        score[gems[gem_num]].text = (int.Parse(score[gems[gem_num]].text)+3+goal_num).ToString();
                        StartCoroutine(pop_gems(gem_num));

                        if(fever_count == 5){
                            fever = true;
                            fever_on();
                        }
                    }
                    else if(no_turn.Contains(gem_num)){
                        gem_clicked.SetActive(false);
                        clicked = false;
                    }
                    // rotate gem
                    else if (!no_turn.Contains(gem_num)){
                        gem_clicked.transform.position = gems_obj[gem_num].transform.position;
                        gem_clicked.SetActive(true);
                        clicked = true;
                    }
                }
                else if(hit.collider.gameObject.tag == "GemEffect" && !mutex_pop){
                    // check if same as goal shape
                    int row_n = (gem_num / 6) % 2;
                    bool all_same = true;
                    for (int i = 1; i < goal_shape.GetLength(2); i++)
                    {
                        if (gem_num + goal_shape[goal_choice,row_n,i] < 0 || gem_num + goal_shape[goal_choice,row_n,i] > 64)
                        {
                            all_same = false;
                            break;
                        }
                        if (gems[gem_num] != gems[gem_num + goal_shape[goal_choice,row_n,i]]){
                            all_same = false;
                            break;
                        }
                    }
                    // pop gems as it is equal to goal shape
                    if (all_same){
                        if(gems[gem_num] != RED) fever_count++;

                        score[gems[gem_num]].text = (int.Parse(score[gems[gem_num]].text)+1).ToString();
                        StartCoroutine(pop_gems(gem_num));

                        if(fever_count == 5){
                            fever = true;
                            fever_on();
                        }
                    }
                }
            }
        }

        if(clicked){
            if (Input.GetKeyDown(KeyCode.A))
            {
                turn_gems('a', gem_num);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                turn_gems('d', gem_num);
            }
        }
    }

    void fever_on(){
        // change gem colors to RED
        for(int i = 0; i < 11; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                if (j == 5 && i % 2 == 0) break;

                gems[i*6 + j] = RED;
                gems_obj[i * 6 + j].GetComponent<SpriteRenderer>().color = gem_color[RED];
            }
        }
    }

    IEnumerator pop_gems(int n)
    {
        mutex_pop = true;
        clicked = false;
        gem_clicked.SetActive(false);

        int row_n = (n / 6) % 2;
        for (int i = 0; i < goal_shape.GetLength(2); i++)
        {
            //Debug.Log("POP:("+n+")" + (n + goal_shape[goal_choice,row_n,i]));
            gems[n + goal_shape[goal_choice,row_n,i]] = -1;
            gems_obj[n + goal_shape[goal_choice,row_n,i]].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }

        yield return new WaitForSeconds(1.0f);

        int current_idx, top_idx;
        for (int i = 0; i < goal_shape.GetLength(2); i++)
        {
            current_idx = top_idx = n + goal_shape[goal_choice,row_n, i];
            
            if (gems[current_idx] != -1) continue;
            
            while (top_idx > -1 && gems[top_idx] == -1) top_idx -= 12;

            while (top_idx > -1)
            {
                gems[current_idx] = gems[top_idx];
                gems_obj[current_idx].GetComponent<SpriteRenderer>().color = gem_color[gems[current_idx]];

                current_idx -= 12;
                top_idx -= 12;
            }

            while (current_idx > -1)
            {
                gems[current_idx] = Random.Range(0, 5);
                gems_obj[current_idx].GetComponent<SpriteRenderer>().color = gem_color[gems[current_idx]];

                current_idx -= 12;
            }
        }

        mutex_pop = false;
    }

    void turn_gems(char keycode, int n)
    {
        //Debug.Log("Pressed: " + keycode);

        int[,] n_arround = new int[2,6] { {-12,-5,7,12, 6,-6 }, { -12, -6, 6, 12, 5, -7 } };
        int row_n = (n / 6) % 2;

        // turn left
        if (keycode == 'a')
        {
            int temp_num = gems[n + n_arround[row_n, 0]];

            for (int i = 1; i < 6; i++)
            {
                gems[n + n_arround[row_n, i - 1]] = gems[n + n_arround[row_n, i]];
                gems_obj[n + n_arround[row_n, i - 1]].GetComponent<SpriteRenderer>().color = gem_color[gems[n + n_arround[row_n, i - 1]]];
            }

            gems[n + n_arround[row_n, 5]] = temp_num;
            gems_obj[n + n_arround[row_n, 5]].GetComponent<SpriteRenderer>().color = gem_color[temp_num];
        }
        // turn right
        else if(keycode == 'd')
        {
            int temp_num = gems[n + n_arround[row_n, 5]];

            for (int i = 5; i >0 ; i--)
            {
                gems[n + n_arround[row_n, i]] = gems[n + n_arround[row_n, i-1]];
                gems_obj[n + n_arround[row_n, i]].GetComponent<SpriteRenderer>().color = gem_color[gems[n + n_arround[row_n, i]]];
            }

            gems[n + n_arround[row_n, 0]] = temp_num;
            gems_obj[n + n_arround[row_n, 0]].GetComponent<SpriteRenderer>().color = gem_color[temp_num];
        }
    }
}
