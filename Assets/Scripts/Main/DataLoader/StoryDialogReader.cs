using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class RawDialogData
{
    public string id;
    public string character;
    public string spriteName;
    public string spritePosition;
    public string dialog;
    public string desc;
}
[System.Serializable]
public class RawDialogDataList
{
    public RawDialogData[] data;

    public static RawDialogDataList CreateFromJSON(string jsonData)
    {
        return JsonUtility.FromJson<RawDialogDataList>("{\"data\":" + jsonData + "}");
    }
}

public class DialogData
{
    public string character;
    public string[] spriteName;
    public string[] spritePosition;
    public string dialog;

    public DialogData(string textID, string inCharacter, string inSprite, string inspritePosition, string inDialog)
    {
        character = inCharacter.Replace(" ", "");
        dialog = inDialog;

        if (inSprite == "")
            return;

        inSprite = inSprite.Replace(" ", "");
        inspritePosition = inspritePosition.Replace(" ", "");
        spriteName = inSprite.Split(',');
        spritePosition = inspritePosition.Split(',');

        if (spriteName.Length != spritePosition.Length)
        {
            Debug.LogError("(DialogDataManager.cs) " + textID + ": number of sprite and position not matching");
            return;
        }
    }
}

public class StoryDialogReader : MonoBehaviour
{
    static private Dictionary<string, DialogData> DialogList = new Dictionary<string, DialogData>();

    static void JsonLoad(string loadID)
    {
        string path = Path.Combine(Application.dataPath, "JSON/FullDialog.json");

        if (!File.Exists(path))
        {
            Debug.LogError("(DialogDataManger.cs) DialogData json file could not be found.");
            return;
        }
        else
        {
            // List<PreDialogData> PreDialogList = new List<PreDialogData>();
            string loadJson = File.ReadAllText(path);

            // Do not load all data at once
            RawDialogDataList RawDialogList = RawDialogDataList.CreateFromJSON(loadJson);

            if (RawDialogList == null)
            {
                Debug.LogError("(DialogDataManger.cs) DialogData json file could not be loaded.");
            }

            if (DialogList.Count != 0)
            {
                DialogList.Clear();
            }

            for (int i = 0; i < RawDialogList.data.Length; i++)
            {
                DialogList.Add(RawDialogList.data[i].id.Replace(" ", ""), new DialogData(RawDialogList.data[i].id, RawDialogList.data[i].character, RawDialogList.data[i].spriteName, RawDialogList.data[i].spritePosition, RawDialogList.data[i].dialog));
            }
        }
    }
    static public DialogData GetDialogData(string id)
    {
        id = id.Replace(" ", "");

        if (!DialogList.ContainsKey(id))
        {
            // load next data
            JsonLoad(id);
        }

        if (!DialogList.ContainsKey(id))
        {
            Debug.LogError("Cannot load: " + id);
        }

        return DialogList[id];
    }
}
