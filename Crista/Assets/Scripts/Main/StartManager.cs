using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartManager : MonoBehaviour
{

    public void SceneTransfer(){
        string nowbutton = "Tutorial";
        SceneManager.LoadScene(nowbutton, LoadSceneMode.Single);
    }
}
