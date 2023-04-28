using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class MinigameDialog
{
    public string name;
    public string chat;
}
[System.Serializable]
public class MinigameDialogList
{
    public MinigameDialog[] characterChat;

    public static MinigameDialogList CreateFromJSON(string jsonData)
    {
        return JsonUtility.FromJson<MinigameDialogList>(jsonData);
    }
}

public class MinigameDialogReader : MonoBehaviour
{
    static private Dictionary<string, string[]> DialogList = new Dictionary<string, string[]>();

    static void JsonLoad(string loadID)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "JSON/MinigameText.json");

        if (!File.Exists(path))
        {
            Debug.LogError("(MinigameDialogReader.cs) Json file could not be found.");
            return;
        }
        else
        {
            // List<PreDialogData> PreDialogList = new List<PreDialogData>();
            string loadJson = File.ReadAllText(path);

            // Do not load all data at once
            MinigameDialogList RawDialogList = MinigameDialogList.CreateFromJSON(loadJson);

            if (RawDialogList == null)
            {
                Debug.LogError("(MinigameDialogReader.cs) Json file could not be loaded.");
            }

            DialogList.Clear();

            for (int i = 0; i < RawDialogList.characterChat.Length; i++)
            {
                DialogList.Add(RawDialogList.characterChat[i].name, RawDialogList.characterChat[i].chat.Split('\r'));
            }
        }
    }
    static public string[] GetDialogData(string id)
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
