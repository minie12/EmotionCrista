using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    string directoryName = "Saves";
    string fileIndex = "0";

    bool bSave = false;
    
    // SaveLoad Data Menu UI
    public GameObject SaveLoadMenu;
    public Text SaveLoadMenuTitle;
    public Text MenuDayText;
    public Text MenuPlayerText;
    public Text MenuPlaceText;
    public Text MenuModeText;

    SaveLoadData gameData;

    public void SetActiveSaveLoadMenu(bool bSave_)
    {
        bSave = bSave_;
        if (bSave)
        {
            SaveLoadMenuTitle.text = "저 장 하 기";
        }
        else
        {
            SaveLoadMenuTitle.text = "불 러 오 기";
        }
        SaveLoadMenu.SetActive(true);
    }

    public void SelectSaveLoadData()
    {
        if (bSave)
        {
            SetSaveData();
        }
        else // load
        {
            StartLoadData();
        }
    }

    public void SetSaveData() {
        gameData.Initialize();

        Fungus.Flowchart flowchart = GameObject.Find("Flowchart").GetComponent<Fungus.Flowchart>();

        gameData.SceneName = SceneManager.GetActiveScene().name;
        gameData.PlayerName = flowchart.GetVariable("PlayerName").GetValue() as string;
        gameData.StoryNumb = flowchart.GetVariable("StoryNumb").GetValue() as string;

        if (gameData.SceneName == "" || gameData.PlayerName == "" || gameData.StoryNumb == "")
        {
            Debug.Log("ERROR(SaveLoadManager): Save data info is missing (" + gameData.SceneName + ", " + gameData.PlayerName + ", " + gameData.StoryNumb + ")");
            return;
        }

        Debug.Log(gameData.StoryNumb.Substring(1));
        string[] temp = gameData.StoryNumb.Substring(1).Split('_');
        foreach(string a in temp){
            Debug.Log(a);
        }
        MenuDayText.text = temp[0];
        MenuPlayerText.text = gameData.PlayerName;
        MenuPlaceText.text = gameData.SceneName;

        SaveToFile_();
    }

    private void SaveToFile_(){
        if(!Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);

        BinaryFormatter binFormatter = new BinaryFormatter();
        FileStream saveStream = File.Create(directoryName + "/" + fileIndex + ".bin");

        binFormatter.Serialize(saveStream, gameData);

        Debug.Log("Saved Data at: " + directoryName + "/" + fileIndex + ".bin");

        saveStream.Close();
    }

    public void StartLoadData(){
        bSave = false;

        gameData.Initialize();
        //LoadFromFile();
        LoadTesting();

        // check if all is loaded well
        if(gameData.SceneName == "" || gameData.PlayerName == "" || gameData.StoryNumb == "")
        {
            Debug.Log("ERROR(SaveLoadManager): Load data info is missing (" + gameData.SceneName + ", " + gameData.PlayerName + ", " + gameData.StoryNumb + ")");
            return;
        }

        //string prefsKey = Fungus.SetSaveProfile.SaveProfile + "_" + Fungus.GetFlowchart().SubstituteVariables("player_name");

        PlayerPrefs.SetInt("LoadData", 1); // flag for checking if scene is loaded from load menu
        PlayerPrefs.SetString("PlayerName", gameData.PlayerName);
        PlayerPrefs.SetString("StoryNumb", gameData.StoryNumb);

        SceneManager.LoadScene(gameData.SceneName, LoadSceneMode.Single);
    }

    private void LoadFromFile(){
        BinaryFormatter binFormatter = new BinaryFormatter();
        string path = directoryName + "/" + fileIndex + ".bin";
        if (!Directory.Exists(path))
        {
            Debug.Log("ERROR(SaveLoadManager): Load file path not found. ");
            return; 
        }

        FileStream load_stream = File.Open(path, FileMode.Open);

        gameData = (SaveLoadData)binFormatter.Deserialize(load_stream);

        load_stream.Close();
    }
    private void LoadTesting()
    {
        gameData.SceneName = "Garden";
        gameData.PlayerName = "SaRangHe";
        gameData.StoryNumb = "D02_RazEnding_1";
    }
}

[System.Serializable]
public struct SaveLoadData
{
    public string SceneName;
    public string PlayerName;
    public string StoryNumb;

    public SaveLoadData(string sname_ = "", string pname_ = "", string story_ = ""){
        SceneName = sname_;
        PlayerName = pname_;
        StoryNumb = story_;
    }

    public void Initialize()
    {
        SceneName = "";
        PlayerName = "";
        StoryNumb = "";
    }
}