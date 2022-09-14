using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    string directory_name = "Saves";
    string file_index = "0";

    SaveLoadData game_data = new SaveLoadData();

    Dictionary<string, string> save_data = new Dictionary<string, string>();

    public void SetSaveData() {
        Fungus.Flowchart flowchart = GameObject.Find("Flowchart").GetComponent<Fungus.Flowchart>();

        game_data.scene_name = SceneManager.GetActiveScene().name;

        string[] keys = flowchart.GetVariableNames();
        for (int i = 0; i < keys.GetLength(0); i++)
        {
            var var = flowchart.GetVariable(keys[i]);
            save_data[keys[i]] = var.GetValue() as string;

            Debug.Log(keys[i] + " " + var.GetValue());
        }

        foreach (KeyValuePair<string, string> kvp in save_data)
            Debug.Log("Key = " + kvp.Key+ ", Value = "+ kvp.Value);

        SaveToFile();
    }

    private void SaveToFile(){
        if(!Directory.Exists(directory_name))
            Directory.CreateDirectory(directory_name);

        BinaryFormatter bin_formatter = new BinaryFormatter();
        FileStream save_stream = File.Create(directory_name + "/" + file_index + ".bin");

        bin_formatter.Serialize(save_stream, save_data);

        Debug.Log("Saved Data at: " + directory_name + "/" + file_index + ".bin");

        save_stream.Close();
    }

    public void StartLoadData(){
        LoadFromFile();
        //LoadTesting();

        //string prefsKey = Fungus.SetSaveProfile.SaveProfile + "_" + Fungus.GetFlowchart().SubstituteVariables("player_name");

        PlayerPrefs.SetInt("LoadData", 1); // flag for checking if scene is loaded from load menu
        //PlayerPrefs.SetString("player_name", game_data.player_name);
        //PlayerPrefs.SetString("story_numb", game_data.story_numb);

        foreach (KeyValuePair<string, string> kvp in save_data)
            PlayerPrefs.SetString(kvp.Key, kvp.Value);

        string scene_name = "";
        save_data.TryGetValue("SceneName", out scene_name) ;
        if (scene_name != "")
            SceneManager.LoadScene(scene_name, LoadSceneMode.Single);
        else
            Debug.Log("ERROR(SaveLoadManager): No scene name found! ");
    }

    private void LoadFromFile(){
        BinaryFormatter bin_formatter = new BinaryFormatter();
        FileStream load_stream = File.Open(directory_name + "/" + file_index + ".bin", FileMode.Open);

        save_data = (Dictionary<string,string>)bin_formatter.Deserialize(load_stream);

        load_stream.Close();
    }
    private void LoadTesting()
    {
        game_data.scene_name = "Garden";
        game_data.player_name = "SaRangHe";
        game_data.story_numb = "D02_RazEnding_1";

        save_data["SceneName"] = "Garden";
        save_data["PlayerName"] = "SaRangHe";
        save_data["StoryNumb"] = "D02_RazEnding_1";
    }
}

[System.Serializable]
public struct SaveLoadData
{
    public string scene_name;
    public string player_name;
    public string story_numb;

    public SaveLoadData(string sname_ = "LabCorridor", string pname_ = " ", string story_ = "D01_NariaAfter"){
        scene_name = sname_;
        player_name = pname_;
        story_numb = story_;
    }
}