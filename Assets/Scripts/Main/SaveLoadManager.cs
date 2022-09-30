using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    string directory_name = "Saves";
    string file_index = "0";

    bool bSave = false;
    public GameObject SaveLoadMenu;
    public Text SaveLoadMenuTitle;

    SaveLoadData game_data;

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

        }
        else // load
        {
            
        }
    }

    public void SetSaveData() {
        game_data.Initialize();

        Fungus.Flowchart flowchart = GameObject.Find("Flowchart").GetComponent<Fungus.Flowchart>();

        game_data.SceneName = SceneManager.GetActiveScene().name;
        game_data.PlayerName = flowchart.GetVariable("PlayerName").GetValue() as string;
        game_data.StoryNumb = flowchart.GetVariable("StoryNumb").GetValue() as string;

        if(game_data.SceneName == "" || game_data.PlayerName == "" || game_data.StoryNumb == "")
        {
            Debug.Log("ERROR(SaveLoadManager): Save data info is missing (" + game_data.SceneName + ", " + game_data.PlayerName + ", " + game_data.StoryNumb+")");
            return;
        }

        SaveToFile_();
    }

    private void SaveToFile_(){
        if(!Directory.Exists(directory_name))
            Directory.CreateDirectory(directory_name);

        BinaryFormatter bin_formatter = new BinaryFormatter();
        FileStream save_stream = File.Create(directory_name + "/" + file_index + ".bin");

        bin_formatter.Serialize(save_stream, game_data);

        Debug.Log("Saved Data at: " + directory_name + "/" + file_index + ".bin");

        save_stream.Close();
    }

    public void StartLoadData(){
        bSave_flag = false;

        game_data.Initialize();
        //LoadFromFile();
        LoadTesting();

        // check if all is loaded well
        if(game_data.SceneName == "" || game_data.PlayerName == "" || game_data.StoryNumb == "")
        {
            Debug.Log("ERROR(SaveLoadManager): Load data info is missing (" + game_data.SceneName + ", " + game_data.PlayerName + ", " + game_data.StoryNumb + ")");
            return;
        }

        //string prefsKey = Fungus.SetSaveProfile.SaveProfile + "_" + Fungus.GetFlowchart().SubstituteVariables("player_name");

        PlayerPrefs.SetInt("LoadData", 1); // flag for checking if scene is loaded from load menu
        PlayerPrefs.SetString("PlayerName", game_data.PlayerName);
        PlayerPrefs.SetString("StoryNumb", game_data.StoryNumb);

        SceneManager.LoadScene(game_data.SceneName, LoadSceneMode.Single);
    }

    private void LoadFromFile(){
        BinaryFormatter bin_formatter = new BinaryFormatter();
        string path = directory_name + "/" + file_index + ".bin";
        if (!Directory.Exists(path))
        {
            Debug.Log("ERROR(SaveLoadManager): Load file path not found. ");
            return; 
        }

        FileStream load_stream = File.Open(path, FileMode.Open);

        game_data = (SaveLoadData)bin_formatter.Deserialize(load_stream);

        load_stream.Close();
    }
    private void LoadTesting()
    {
        game_data.SceneName = "Garden";
        game_data.PlayerName = "SaRangHe";
        game_data.StoryNumb = "D02_RazEnding_1";
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