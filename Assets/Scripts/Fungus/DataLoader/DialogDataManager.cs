using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class RawDialogData
{
    public string Id;
    public string Character;
    public string SpriteName;
    public string SpritePosition;
    public string Dialog;
    public string Desc;
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

    public DialogData(string inCharacter, string inSprite, string inspritePosition, string inDialog)
    {
        character = inCharacter;
        inSprite = inSprite.Replace(" ", "");
        inspritePosition = inspritePosition.Replace(" ", "");
        spriteName = inSprite.Split(',');
        spritePosition = inspritePosition.Split(',');
        dialog = inDialog;
    }
}


public class DialogDataManager : MonoBehaviour
{
    static private Dictionary<string, DialogData> DialogList = new Dictionary<string, DialogData>();

    static void JsonLoad(string loadID)
    {
        string path = Path.Combine(Application.dataPath, "JSON/FullDialog.json");

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
            RawDialogDataList RawDialogList = RawDialogDataList.CreateFromJSON(loadJson);

            if (RawDialogList == null)
            {
                Debug.Log("ERROR(DialogDataManger.cs): DialogData json file could not be loaded.");
            }

            if (DialogList.Count != 0)
            {
                DialogList.Clear();
            }

            for (int i = 0; i < RawDialogList.data.Length; i++)
            {
                DialogList.Add(RawDialogList.data[i].Id, new DialogData(RawDialogList.data[i].Character, RawDialogList.data[i].SpriteName, RawDialogList.data[i].SpritePosition, RawDialogList.data[i].Dialog));
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

        return DialogList[id];
    }
}
