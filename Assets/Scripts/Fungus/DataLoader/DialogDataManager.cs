using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PreDialogData
{
    public string id;
    public string desc;
    public string character;
    public string dialog;
    public string spriteRenderer;
    public string sprite;
}
[System.Serializable]
public class PreDialogDataList
{
    public PreDialogData[] data;
}

public class DialogData
{
    public string character;
    public string dialog;
    public string spriteRenderer;
    public string sprite;

    public DialogData(string inCharacter, string inDialog, string inSpriteRenderer, string inSprite)
    {
        character = inCharacter;
        dialog = inDialog;
        spriteRenderer = inSpriteRenderer;
        sprite = inSprite;
    }
}


public class DialogDataManager : MonoBehaviour
{
    static private Dictionary<string, DialogData> DialogList = new Dictionary<string, DialogData>();

    static void JsonLoad(string loadID)
    {
        string path = Path.Combine(Application.dataPath, "JSON/DialogData.json");

        if (!File.Exists(path))
        {
            Debug.Log("ERROR(DialogDataManger.cs): DialogData json file could not be found.");
            return;
        }
        else
        {
            // List<PreDialogData> PreDialogList = new List<PreDialogData>();
            string loadJson = File.ReadAllText(path);
            Debug.Log(loadJson);

            // Do not load all data at once
            PreDialogDataList PreDialogList = new PreDialogDataList();
            PreDialogList = JsonUtility.FromJson<PreDialogDataList>("{\"data\":" + loadJson + "}");


            if (PreDialogList == null)
            {
                Debug.Log("ERROR(DialogDataManger.cs): DialogData json file could not be loaded.");
            }

            if (DialogList.Count != 0)
            {
                DialogList.Clear();
            }

            for (int i = 0; i < PreDialogList.data.Length; i++)
            {
                DialogList.Add(PreDialogList.data[i].id, new DialogData(PreDialogList.data[i].character, PreDialogList.data[i].dialog, PreDialogList.data[i].spriteRenderer, PreDialogList.data[i].sprite));
                Debug.Log(PreDialogList.data[i].id + " " + DialogList[PreDialogList.data[i].id].character + " " + DialogList[PreDialogList.data[i].id].sprite);

            }
        }
    }

    static public DialogData GetDialogData(string id)
    {
        if(!DialogList.ContainsKey(id))
        {
            // load next data
            JsonLoad(id);
        }

        if (!DialogList.ContainsKey(id))
        {
            Debug.Log("Cannot load: " + id);
        }

        Debug.Log(id + " " + DialogList[id].character + " " + DialogList[id].sprite);

        return DialogList[id];
    }
}
