using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveLoadMenuManager : MonoBehaviour
{
    bool bSave = false;
    
    // SaveLoad Data Menu UI
    public GameObject SaveLoadMenu;
    public Image SaveLoadMenuTitle;

    public GameObject[] Slots;

    public Sprite EmptySlotImage;
    public Sprite ActiveSlotImage;

    public Sprite LoadTitleImage;
    public Sprite SaveTitleImage;

    public Sprite[] GemIcons;

    public void Start()
    {
        for (int index = 0; index < Slots.Length; ++index)
        {
            string loadPath = GetSaveDataPath(index);

            if (true == System.IO.File.Exists(loadPath))
            {
                GameObject loadSlot = Slots[index];
                EmoSaveData saveData = LoadFromFile(loadPath);

                if (null != loadSlot && null != saveData)
                {
                    FillSaveMenu(saveData, loadSlot);
                }
            }
        }
    }

    public void SetActiveSaveLoadMenu(bool bSave_)
    {
        bSave = bSave_;
        if (bSave)
        {
            SaveLoadMenuTitle.sprite = SaveTitleImage;
        }
        else
        {
            SaveLoadMenuTitle.sprite = LoadTitleImage;
        }
        SaveLoadMenu.SetActive(true);
    }

    public void SelectSaveLoadData(int index)
    {
        if (bSave)
        {
            StartSaveData(index);
        }
        else // load
        {
            StartLoadData(index);
        }
    }

    protected void StartSaveData(int _index) {
        string savePath = GetSaveDataPath(_index);

        if (true == System.IO.File.Exists(savePath))
        {
            Debug.Log("File Exists. Continue? NOTIFY");
            System.IO.File.Delete(savePath);
        }    
        if (Slots.Length <= 0)
        {
            Debug.LogError("Slot is not added. Check SaveLoadManager");
            return;
        }
        
        EmoSaveData gameData = EmoSaveData.CreateSaveData();
        GameObject saveSlot = Slots[_index];

        if (null != gameData && null != saveSlot)
        {
            if (false == System.IO.File.Exists(savePath))
            {
                if (false == System.IO.Directory.Exists(GetSaveDataDirectory()))
                {
                    System.IO.Directory.CreateDirectory(GetSaveDataDirectory());
                }

                //System.IO.File.Create(savePath);
            }

            // Save To File

            FileStream stream = new FileStream(savePath, FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, gameData);

            stream.Close();

            Debug.Log("Saved Data at: " + savePath);
        }
        else
        {
            Debug.LogError("Failed to save data in slot " + _index + ": " + savePath);
        }

        // Show Save data is succeeded in saving
        if (true == System.IO.File.Exists(savePath))
        {
            FillSaveMenu(gameData, saveSlot);
        }
    }

    private bool FillSaveMenu(EmoSaveData inSaveData, GameObject inSaveSlot)
    {
        if (null == inSaveSlot)
        {
            Debug.LogError("Save Slot is not set. Check SaveLoadManager.");
            return false;
        }

        GameObject ActiveSlotObj = inSaveSlot.transform.Find("Active").gameObject;
        if (null == ActiveSlotObj)
        {
            return false;
        }

        ActiveSlotObj.SetActive(true);

        bool bDay, bPlayer, bPlace, bGem;
        bDay = bPlayer = bPlace = bGem = false;
        {
            GameObject MenuDay = ActiveSlotObj.transform.Find("Day").gameObject;
            if (null != MenuDay)
            {
                GameObject MenuDayTextObj = MenuDay.transform.Find("Number").gameObject;
                if (null != MenuDayTextObj)
                {
                    Text MenuDayText = MenuDayTextObj.GetComponent<Text>();
                    if (null != MenuDayText)
                    {
                        MenuDayText.text = inSaveData.playInfo.dayCount.ToString();
                        bDay = true;
                    }
                }
            }
        }

        {
            GameObject MenuPlayer = ActiveSlotObj.transform.Find("PlayerName").gameObject;
            if (null != MenuPlayer)
            {
                Text MenuPlayerText = MenuPlayer.GetComponent<Text>();
                if (null != MenuPlayerText)
                {
                    MenuPlayerText.text = inSaveData.playInfo.playerName.ToString();
                    bPlayer = true;
                }
            }
        }

        {
            GameObject MenuPlace = ActiveSlotObj.transform.Find("PlaceInfo").gameObject;
            if (null != MenuPlace)
            {
                GameObject MenuPlaceTextObj = MenuPlace.transform.Find("Scene").gameObject;
                if (null != MenuPlaceTextObj)
                {
                    Text MenuPlaceText = MenuPlaceTextObj.GetComponent<Text>();
                    if (null != MenuPlaceText)
                    {
                        MenuPlaceText.text = UICanvasManager.GetLocationName(inSaveData.SceneName);
                        
                        bPlace = true;
                    }
                }
            }
        }

        {
            GameObject MenuGem = ActiveSlotObj.transform.Find("Gem").gameObject;
            if (null != MenuGem)
            {
                Image GemImage = MenuGem.GetComponent<Image>();
                if (null != GemImage)
                {
                    int ColorIndex = inSaveData.playInfo.dayCount - 1;
                    if (0 <= ColorIndex && ColorIndex < GemIcons.Length)
                    {
                        GemImage.sprite = GemIcons[ColorIndex];

                        bGem = true;
                    }
                }
            }
        }


        if (bDay && bPlayer && bPlace && bGem)
        {
            Image SlotImage = inSaveSlot.GetComponent<Image>();
            if (null != SlotImage)
            {
                SlotImage.sprite = ActiveSlotImage;
            }

            return true;
        }

        ActiveSlotObj.SetActive(false);

        return false;
    }

    protected void StartLoadData(int _index)
    {
        bSave = false;

        string loadPath = GetSaveDataPath(_index);

        if (true == System.IO.File.Exists(loadPath))
        {
            EmoSaveData loadedData = LoadFromFile(loadPath);
            if (null == loadedData)
            {
                Debug.LogError("ERROR(SaveLoadManager): Load data info is missing");
            }
            if (false == loadedData.ValidateData())
            {
                Debug.Log("ERROR(SaveLoadManager): Load data info is missing");
                return;
            }

            GameManager gameManager = GameManager.Get();
            if (null != gameManager)
            {
                gameManager.SetLoadData(loadedData);
            }

            SceneManager.LoadScene(loadedData.SceneName, LoadSceneMode.Single);
        }
    }

    private EmoSaveData LoadFromFile(string loadPath)
    {
        if (File.Exists(loadPath))
        {
            FileStream stream = new FileStream(loadPath, FileMode.Open);

            BinaryFormatter formatter = new BinaryFormatter();
            EmoSaveData data = formatter.Deserialize(stream) as EmoSaveData;

            stream.Close();

            return data;
        }

        return null;
    }

    private string GetSaveDataPath(int index)
    {
        return string.Format("{0}{1}.bin", GetSaveDataDirectory(), index);
    }

    private string GetSaveDataDirectory()
    {
        return string.Format("{0}/{1}/", Application.persistentDataPath, "Saves");
    }

    // DEBUG
    static public void EraseData()
    {
        string directoryPath = string.Format("{0}/{1}/", Application.persistentDataPath, "Saves");

        if (Directory.Exists(directoryPath))
        {
            Directory.Delete(directoryPath, true);
        }
    }
}