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

            Debug.Assert(RawDialogList != null, "(DiaryDialogReader.cs) Json file could not be loaded.");

            DialogList.Clear();

            for (int i = 0; i < RawDialogList.diaryDialog.Length; i++)
            {
                int keyValue = CreateDiaryIndex(RawDialogList.diaryDialog[i].round, RawDialogList.diaryDialog[i].characterIndex);
                DialogList.Add(keyValue, RawDialogList.diaryDialog[i].dialog);
            }
        }
    }
    static public string GetDialogData(bool bInMultiRound, int inCharacterIndex, bool bInRedButtonPressed)
    {
        int diaryIndex = CreateDiaryIndex( (bInMultiRound ? 1 : 0), inCharacterIndex);

        if (bInRedButtonPressed)
        {
            // Only red has extra dialog
            // change json file when more data is added
            if (true == bInMultiRound && 3 == inCharacterIndex)
            {
                return "출근 셋째 날, \r\r일이 생겼다.\r\r위험부담이 가장 큰 붉은색 감정의\r내담자분께서 감정 격화로 인한 변이로 상담이 중지 되었다.\r\r상담사로 그 자리에 있었음에도 불구하고\r제련에 실패했다.\r\r이번 일로 인해서야 내담자분의 기분을 이해할 수 있게 됐다 생각하니 아이러니하다.\r\r지금이라면… 상황이 달라질까?하는 생각이 끝없이 밀려든다.\r\r오늘은 잠이 안 올 거 같으니 관련 서적들을 한줄이라도 더 읽어봐야겠다.\r\r더 이상 내가 부족해서 이런 일을 겪는 내담자가 나오지 않기를 바란다.";
            }
        }
        else
        {
            if (!DialogList.ContainsKey(diaryIndex))
            {
                // load next data
                JsonLoad();
            }

            if(DialogList.ContainsKey(diaryIndex))
            {
                return DialogList[diaryIndex];
            }
        }

        Debug.LogError("[DiaryDialogReader] Wrong play Info passed: " + bInMultiRound + " " + inCharacterIndex + " " + bInRedButtonPressed);
        return "";
    }
}