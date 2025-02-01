using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class BoardSettings
{
    public List<float> fullPlayTime;
    public List<float> playTimeSpeed;
    public List<float> crushedGaugeTime;
    public List<float> fullScore;
    public List<float> scoreSpeed;
    public List<int> goalUnit;
}

[System.Serializable]
public class Gimmick
{
    public int cnt;
    public List<List<int>> type;
    public List<int> failGaugeMount;
}

[System.Serializable]
public class PatternConfig
{
    public string name;
    public BoardSettings boardSettings;
    public Gimmick gimmick;
}

public class PatternConfigReader : MonoBehaviour
{
    static public List<PatternConfig> LoadPatternConfigList()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "JSON/PatternConfig.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            List<PatternConfig> patternConfigList = JsonConvert.DeserializeObject<List<PatternConfig>>(json);

            return patternConfigList;
        }
        else
        {
            Debug.LogError("JSON file not found: " + filePath);
        }
        return new List<PatternConfig>(){ };
    }

    static public PatternConfig GetPatternConfig(int patternIdx)
    {
        List<PatternConfig> patternConfigList = LoadPatternConfigList();

        return patternConfigList[patternIdx];
    }
}
