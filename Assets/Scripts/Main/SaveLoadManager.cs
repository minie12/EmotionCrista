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
            StartSaveData();
        }
        else // load
        {
            StartLoadData();
        }
    }

    public void StartSaveData() {
        EmoSaveData gameData = EmoSaveData.CreateSaveData();

        MenuDayText.text = gameData.StoryRound.ToString();
        MenuPlayerText.text = gameData.PlayerName;
        MenuPlaceText.text = gameData.SceneName;

        // Save To File
        BinaryFormatter formatter = new BinaryFormatter();
        string savePath = Application.persistentDataPath + "/" + directoryName + ".bin";

        FileStream stream = new FileStream(savePath, FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();

        Debug.Log("Saved Data at: " + directoryName + "/" + fileIndex + ".bin");
    }

    public void StartLoadData(){
        bSave = false;

        EmoSaveData loadedData = LoadFromFile();
        if (false == loadedData.ValidateData())
        {
            Debug.Log("ERROR(SaveLoadManager): Load data info is missing");
            return;
        }

        //string prefsKey = Fungus.SetSaveProfile.SaveProfile + "_" + Fungus.GetFlowchart().SubstituteVariables("player_name");

        GameObject flowchartObj = GameObject.Find("Flowchart");
        if (null != flowchartObj)
        {
            Fungus.Flowchart flowchart = flowchartObj.GetComponent<Fungus.Flowchart>();

        }

        SceneManager.LoadScene(loadedData.SceneName, LoadSceneMode.Single);
    }

    private EmoSaveData LoadFromFile(){
        string loadPath = Application.persistentDataPath + "/" + directoryName + ".bin";

        if (File.Exists(loadPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(loadPath, FileMode.Open);

            EmoSaveData data = formatter.Deserialize(stream) as EmoSaveData;

            stream.Close();

            return data;
        }

        return null;
    }
}