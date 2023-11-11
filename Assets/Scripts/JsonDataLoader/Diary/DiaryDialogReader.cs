using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DiaryDialogData
{
    public int round;
    public int characterIndex;
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

    static int CreateDiaryIndex(int bInMultiRound, int inCharacterIndex)
    {
        return bInMultiRound * 10 + inCharacterIndex;
    }

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

            Debug.Assert(RawDialogList == null, "(DiaryDialogReader.cs) Json file could not be loaded.");

            DialogList.Clear();

            for (int i = 0; i < RawDialogList.diaryDialog.Length; i++)
            {
                int keyValue = CreateDiaryIndex(RawDialogList.diaryDialog[i].round, RawDialogList.diaryDialog[i].characterIndex);
                DialogList.Add(keyValue, RawDialogList.diaryDialog[i].dialog);
            }
        }
    }
    static public string GetDialogData(bool bInMultiRound, int inCharacterIndex)
    {
        int diaryIndex = CreateDiaryIndex( (bInMultiRound ? 1 : 0), inCharacterIndex);

        if (!DialogList.ContainsKey(diaryIndex))
        {
            // load next data
            JsonLoad();
        }

        Debug.Assert(DialogList.ContainsKey(diaryIndex), "CannotLoad: " + diaryIndex);

        return DialogList[diaryIndex];
    }
}