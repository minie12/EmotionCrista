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

    public void SetSaveData(List<Fungus.Variable> variables){
        //var flowchart = GameObject.FindObjectOfType<Fungus.Flowchart>();
        //List<Fungus.Variable> variables = flowchart.Variables;

        game_data.scene_name = SceneManager.GetActiveScene().name;

        for (int i = 0; i < variables.Count; i++)
        {
            var variable = variables[i];
            if (variable == null) continue;
            else if (variable.Key == "PlayerName")
            {
                Fungus.StringVariable v = variable as Fungus.StringVariable;
                game_data.player_name = v.Value;
            }
            else if(variable.Key == "StoryNumb")
            {
                Fungus.StringVariable v = variable as Fungus.StringVariable;
                game_data.story_numb = v.Value;
            }
        }

        SaveToFile();
    }

    private void SaveToFile(){
        if(!Directory.Exists(directory_name))
            Directory.CreateDirectory(directory_name);

        BinaryFormatter bin_formatter = new BinaryFormatter();
        FileStream save_stream = File.Create(directory_name + "/" + file_index + ".bin");

        bin_formatter.Serialize(save_stream, game_data);

        Debug.Log("Saved Data at: " + directory_name + "/" + file_index + ".bin");

        save_stream.Close();
    }

    public void StartLoadData(){
        LoadFromFile();


    }

    private void LoadFromFile(){
        BinaryFormatter bin_formatter = new BinaryFormatter();
        FileStream load_stream = File.Open(directory_name + "/" + file_index + ".bin", FileMode.Open);

        game_data = (SaveLoadData)bin_formatter.Deserialize(load_stream);

        Debug.Log(game_data.scene_name);
        Debug.Log(game_data.player_name);
        Debug.Log(game_data.story_numb);

        load_stream.Close();
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