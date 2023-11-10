using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DiaryDialogData
{
    public int round;
    public string characterName;
    public string dialog;
}
[System.Serializable]
public class DiaryDialogDataList
{
    public DiaryDialogData[] diaryDialog;
    public static DiaryDialogDataList CreateFromJSON(string jsonData)
    {
        return JsonUtility.FromJson<DiaryDialogDataList>("{\"diaryDialog\":" + jsonData + "}");
    }
}

public class DiaryDialogReader : MonoBehaviour
{
    static private Dictionary<int, string> DialogList = new Dictionary<int, string>();

    static void JsonLoad()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "JSON/DiaryText.json");

        if (!File.Exists(path))
        {
            Debug.LogError("(DiaryDialogReader.cs) Json file could not be found.");
            return;
        }
        else
        {
            // List<PreDialogData> PreDialogList = new List<PreDialogData>();
            string loadJson = File.ReadAllText(path);

            // Do not load all data at once
            DiaryDialogDataList RawDialogList = DiaryDialogDataList.CreateFromJSON(loadJson);

            if (RawDialogList == null)
            {
                Debug.LogError("(DiaryDialogReader.cs) Json file could not be loaded.");
            }

            DialogList.Clear();

            for (int i = 0; i < RawDialogList.diaryDialog.Length; i++)
            {
                int keyValue = OldGameManager.instance.CreateStoryIndex(RawDialogList.diaryDialog[i].round, RawDialogList.diaryDialog[i].characterName);
                DialogList.Add(keyValue, RawDialogList.diaryDialog[i].dialog);
            }
        }
    }
    static public string GetDialogData(int id)
    {
        if (!DialogList.ContainsKey(id))
        {
            // load next data
            JsonLoad();
        }

        Debug.Assert(DialogList.ContainsKey(id), "CannotLoad: " + id);

        return DialogList[id];
    }
}