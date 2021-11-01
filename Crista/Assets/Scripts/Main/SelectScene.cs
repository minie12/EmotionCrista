using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectScene : MonoBehaviour
{
    public int goalNum = 0;
    public Dropdown goalOption;

    public void SceneTransferMain(){
        goalNum = goalOption.value;
        Debug.Log(goalOption.value+3);

        PlayerPrefs.SetInt("goalNum", goalNum);

        string nowbutton = EventSystem.current.currentSelectedGameObject.name;
        SceneManager.LoadScene(nowbutton, LoadSceneMode.Single);
    }

    public void SceneTransfer(){
        string nowbutton = EventSystem.current.currentSelectedGameObject.name;
        SceneManager.LoadScene(nowbutton, LoadSceneMode.Single);
    }
}
