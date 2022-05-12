using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MiniManager : MonoBehaviour
{
    // UI
    public GameObject UI_Canvas;
    public Image timer_fill;
    public Image score_fill;
    public Text score_txt;

    // timer
    public int full_time;
    public float full_fever_time = 10f;
    private float fever_time; // 10f
    private float time;

    // board related
    private BoardManager board;
    public SpriteRenderer board_img;
    public Sprite[] puzzle_board_sp;
    private int goal_unit = 2;

    // score
    public float full_score;
    private int score;


    // fever
    public int full_fever;
    private int fever;
    public Image fever_fill_img;
    public Image fever_img;
    public Button fever_btn;
    public bool fever_on = false;
    public Sprite[] fever_sp;
    public Sprite[] fever_fill_sp;
    public Animator animator;


    // YELLOW Pattern
    public GameObject bubblePF;
    private GameObject[] bubbles;
    private int y_index;
    private int bubble_numb;


    // public Texture2D cursor;
    // Vector2 mc = new Vector2(0, 0.1f); 


    void Start(){
        time = full_time; score = 0; fever = 0;
        board = GameObject.Find("Board").GetComponent<BoardManager>();
        board.goal_color = 0;

        // ui
        score_fill.fillAmount = 0;
        timer_fill.fillAmount = 0;
        fever_fill_img.fillAmount = 0;

        if(PlayerPrefs.HasKey("goalUnit")) goal_unit = PlayerPrefs.GetInt("goalUnit");
        board.SetGoal(goal_unit);

        // PATTERN YELLOW
        Y_Start();
    }

    void Update(){
        // timer_fill.fillAmount = Mathf.InverseLerp(0, full_time, time);
        timer_fill.fillAmount = time/full_time;
        time -= Time.deltaTime;

        // used to manipulate mouse cursor shown on screen (not the actual HW)
        // mc = new Vector2(0, mc.y + 20*Time.deltaTime); 
        // Cursor.SetCursor(cursor, mc, CursorMode.Auto);

        if(fever_on){
            fever_fill_img.fillAmount = Mathf.InverseLerp(0, full_fever_time, fever_time);
            fever_time -= Time.deltaTime;
            if(fever_time <= 0) EndFever();
        }
    }

    public void AddScore(int n){
        score += n;
        int score_pt = (int)((score/full_score)*100);
        if(score_pt > 100) score_pt = 100;
        score_txt.text = (score_pt).ToString() + " %";
        score_fill.fillAmount = Mathf.InverseLerp(0, full_score, score);

        if(!fever_on) AddFever(n);
    }

    public void AddFever(int n){
        fever += n;
        fever_fill_img.fillAmount = Mathf.InverseLerp(0, full_fever, fever);

        if(fever > full_fever && !fever_on) {
            animator.SetBool("fever_on", true);
            fever_fill_img.sprite = fever_fill_sp[1];
            fever_img.sprite = fever_sp[1];
            fever_btn.enabled = true;
        }
    }

    public void StartFever(){
        // reset
        fever_on = true;
        fever_time = full_fever_time;
        fever = 0;
        board_img.sprite = puzzle_board_sp[1];


        //reset fever gauge
        // fever_fill_img.fillAmount = Mathf.InverseLerp(0, full_fever, fever);

        Y_StartFever(); // pattern yellow
        board.StartFever();
        fever_btn.enabled = false;
    }

    public void EndFever(){
        animator.SetBool("fever_on", false);

        board_img.sprite = puzzle_board_sp[0];
        fever_fill_img.sprite = fever_fill_sp[0];
        fever_img.sprite = fever_sp[0];
        fever_on = false;
        board.EndFever();
        Y_EndFever(); // pattern yellow
    }


    // used to move to another scene
    public void SceneTransfer()
    {
        string nowbutton = EventSystem.current.currentSelectedGameObject.name;
        SceneManager.LoadScene(nowbutton, LoadSceneMode.Single);
    }

    // PATTERN ----------------------------------------------------
    // Y
    void Y_Start()
    {
        // Pattern yellow
        y_index = 0; bubble_numb = 5;

        bubbles = new GameObject[bubble_numb];
        for(int i = 0; i < bubble_numb; i++){
            GameObject gem_temp = Instantiate(bubblePF, new Vector3(0,0,0), Quaternion.identity, UI_Canvas.transform);
            gem_temp.SetActive(false);
            bubbles[i] = gem_temp;
        }
        InvokeRepeating("Y_SpawnBubble", 2, 2);
    }

    void Y_StartFever(){
        CancelInvoke(); // stop spawing bubbles
        // de-activate all bubles
        for(int i = 0; i < bubble_numb; i++) bubbles[i].SetActive(false);
        y_index = 0;
    }

    void Y_EndFever(){
        InvokeRepeating("Y_SpawnBubble", 2, 2);
    }

    // PATTERN -- YELLOW
    void Y_SpawnBubble(){
        Vector3 rand_pos = new Vector3(Random.Range(800.0f, 1600.0f), Random.Range(220.0f, 850.0f), 5);
        bubbles[y_index].transform.position = Camera.main.ScreenToWorldPoint(rand_pos);
        bubbles[y_index].SetActive(true);
        y_index = (y_index+1)%bubble_numb;
    }
}
