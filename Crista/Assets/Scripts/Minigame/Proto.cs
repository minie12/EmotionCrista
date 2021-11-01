using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class Proto : MonoBehaviour
{
    const int RED = 0;
    const int YELLOW = 1;
    const int GREEN = 2;
    const int BLUE = 3;
    const int PURPLE = 4;

    private int[] jewels = new int[91];
    public GameObject[] jewels_obj;
    public GameObject jewel_clicked;
    public Text[] score;

    private Color[] jewel_color = new Color[5] { Color.red, Color.yellow, Color.green, new Color(0, 1, 1), new Color(0.64f, 0.1f, 1) };
    private int[] no_turn = new int[34] { 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 13, 21, 27, 35, 41, 49, 55, 63, 69, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89 };
    private int[,,] goal_shape = new int[3,2,5] { {{0, -14, -6,0,0 }, {0, -14, -7,0,0 }},{{-14,-6,0,7,0},{-14,-7,0,6,0}},{{0,8,13,14,21},{0,7,13,14,20}} };

    private bool clicked = false;
    private bool mutexPop = false;
    private int jewel_num = -1;

    // goal gem related
    public GameObject goalSprite;
    public Sprite[] goalSprites;
    private int goalNum = 0;


    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("goalNum")) goalNum = PlayerPrefs.GetInt("goalNum");
         
        goalSprite.GetComponent<SpriteRenderer>().sprite = goalSprites[goalNum];

        // display gems 
        for(int i = 0; i < 13; i++)
        {
            for(int j = 0; j < 7; j++)
            {
                if (j == 6 && i % 2 == 0)
                {
                    jewels[i * 7 + j] = -1;
                    break;
                }

                int color = Random.Range(0, 5);
                jewels[i*7 + j] = color;
                jewels_obj[i * 7 + j].GetComponent<SpriteRenderer>().color = jewel_color[color];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // if get mouse click
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)  // if mouse clicks on object
            {
                if(hit.collider.gameObject.tag == "Gem" && !mutexPop){
                    jewel_num = int.Parse(hit.collider.gameObject.name);
                    int row_n = (jewel_num / 7) % 2;
                    //Debug.Log("hit: " + jewel_num);

                    // check if same as goal shape
                    bool all_same = true;
                    for (int i = 1; i < goal_shape.GetLength(2); i++)
                    {
                        if (jewel_num + goal_shape[goalNum,row_n,i] < 0 || jewel_num + goal_shape[goalNum,row_n,i] > 89)
                        {
                            all_same = false;
                            break;
                        }
                        if (jewels[jewel_num] != jewels[jewel_num + goal_shape[goalNum,row_n,i]]){
                            all_same = false;
                            break;
                        }
                    }
                    if (all_same){
                        score[jewels[jewel_num]].text = (int.Parse(score[jewels[jewel_num]].text)+1).ToString();
                        StartCoroutine(pop_jewels(jewel_num));
                    }
                    else if(no_turn.Contains(jewel_num)){
                        jewel_clicked.SetActive(false);
                        clicked = false;
                    }
                    // rotate jewel
                    else if (!no_turn.Contains(jewel_num)){
                        jewel_clicked.transform.position = jewels_obj[jewel_num].transform.position;
                        jewel_clicked.SetActive(true);
                        clicked = true;
                    }
                }
                else if(hit.collider.gameObject.tag == "GemEffect" && !mutexPop){
                    // check if same as goal shape
                    int row_n = (jewel_num / 6) % 2;
                    bool all_same = true;
                    for (int i = 1; i < goal_shape.GetLength(2); i++)
                    {
                        if (jewel_num + goal_shape[goalNum,row_n,i] < 0 || jewel_num + goal_shape[goalNum,row_n,i] > 64)
                        {
                            all_same = false;
                            break;
                        }
                        if (jewels[jewel_num] != jewels[jewel_num + goal_shape[goalNum,row_n,i]]){
                            all_same = false;
                            break;
                        }
                    }
                    if (all_same){
                        score[jewels[jewel_num]].text = (int.Parse(score[jewels[jewel_num]].text)+1).ToString();
                        StartCoroutine(pop_jewels(jewel_num));
                    }
                }
            }
        }

        if(clicked){
            if (Input.GetKeyDown(KeyCode.A))
            {
                turn_jewels('a', jewel_num);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                turn_jewels('d', jewel_num);
            }
        }
    }

    IEnumerator pop_jewels(int n)
    {
        mutexPop = true;
        clicked = false;
        jewel_clicked.SetActive(false);

        int row_n = (n / 7) % 2;
        for (int i = 0; i < goal_shape.GetLength(2); i++)
        {
            Debug.Log("POP:("+n+")" + (n + goal_shape[goalNum,row_n,i]));
            jewels[n + goal_shape[goalNum,row_n,i]] = -1;
            jewels_obj[n + goal_shape[goalNum,row_n,i]].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }

        yield return new WaitForSeconds(1.0f);

        int current_idx, top_idx;
        for (int i = 0; i < goal_shape.GetLength(2); i++)
        {
            current_idx = top_idx = n + goal_shape[goalNum,row_n, i];
            
            if (jewels[current_idx] != -1) continue;
            
            while (top_idx > -1 && jewels[top_idx] == -1) top_idx -= 14;

            while (top_idx > -1)
            {
                jewels[current_idx] = jewels[top_idx];
                jewels_obj[current_idx].GetComponent<SpriteRenderer>().color = jewel_color[jewels[current_idx]];

                current_idx -= 14;
                top_idx -= 14;
            }

            while (current_idx > -1)
            {
                jewels[current_idx] = Random.Range(0, 5);
                jewels_obj[current_idx].GetComponent<SpriteRenderer>().color = jewel_color[jewels[current_idx]];

                current_idx -= 14;
            }
        }
        mutexPop = false;
    }

    void turn_jewels(char keycode, int n)
    {
        //Debug.Log("Pressed: " + keycode);

        int[,] n_arround = new int[2,6] { {-14,-6,8,14, 7,-7 }, { -14, -7, 7, 14, 6, -8 } };
        int row_n = (n / 7) % 2;

        // turn left
        if (keycode == 'a')
        {
            int temp_num = jewels[n + n_arround[row_n, 0]];

            for (int i = 1; i < 6; i++)
            {
                jewels[n + n_arround[row_n, i - 1]] = jewels[n + n_arround[row_n, i]];
                jewels_obj[n + n_arround[row_n, i - 1]].GetComponent<SpriteRenderer>().color = jewel_color[jewels[n + n_arround[row_n, i - 1]]];
            }

            jewels[n + n_arround[row_n, 5]] = temp_num;
            jewels_obj[n + n_arround[row_n, 5]].GetComponent<SpriteRenderer>().color = jewel_color[temp_num];
        }
        // turn right
        else if(keycode == 'd')
        {
            int temp_num = jewels[n + n_arround[row_n, 5]];

            for (int i = 5; i >0 ; i--)
            {
                jewels[n + n_arround[row_n, i]] = jewels[n + n_arround[row_n, i-1]];
                jewels_obj[n + n_arround[row_n, i]].GetComponent<SpriteRenderer>().color = jewel_color[jewels[n + n_arround[row_n, i]]];
            }

            jewels[n + n_arround[row_n, 0]] = temp_num;
            jewels_obj[n + n_arround[row_n, 0]].GetComponent<SpriteRenderer>().color = jewel_color[temp_num];
        }
    }
}
