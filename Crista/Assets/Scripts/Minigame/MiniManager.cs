using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MiniManager : MonoBehaviour
{
    private int score;

    public Text score_txt;
    public GameObject fever_btn;

    private BoardManager board;
    private int goal_num = 0;

    // fever
    private int fever_num = 1;
    private bool fever_on = false;

    void Start(){
        score = 0;
        board = GameObject.Find("Board").GetComponent<BoardManager>();
        board.goal_color = 0;

        if(PlayerPrefs.HasKey("goalNum")) goal_num = PlayerPrefs.GetInt("goalNum");
    }

    public void AddScore(int n){
        score += n;
        score_txt.text = score.ToString();

        if(score > fever_num && !fever_on) fever_btn.SetActive(true);
    }

    public void StartFever(){
        fever_on = true;
        board.StartFever();
        fever_btn.SetActive(false);
    }

    public void EndFever(){
        fever_on = false;
    }

    // used to move to another scene
    public void SceneTransfer()
    {
        string nowbutton = EventSystem.current.currentSelectedGameObject.name;
        SceneManager.LoadScene(nowbutton, LoadSceneMode.Single);
    }
}
