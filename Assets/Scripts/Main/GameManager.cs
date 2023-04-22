using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
//public class CharacterChat{
//    public string name;
//    public string chat;
//}
//[System.Serializable]
//public class myChatList{
//    public CharacterChat[] characterChat;
//}
public enum GameState
{
    StartMenu,
    StoryMode,
    MiniGameMode
}

public enum StoryRound
{
    None,
    First,
    Second,
    Error
}
public enum CharacterName
{
    Naria,
    Lulian,
    Russel,
    Nish,
    Ilac,
    Max
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState state;

    private StoryRound currentRound = StoryRound.First;
    private CharacterName currentCharacter = CharacterName.Naria;

    private GameObject fungusManager;
    
    // sound
    public float soundVolumeBGM;
    public float soundVolumeSFX;

    //public TextAsset textJSON;
    //public myChatList myChatList = new myChatList();

    // Start is called before the first frame update
    void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        if(instance == null) // If there is no instance already
        {
            Debug.Log("inst");
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
            instance = this;  
            //myChatList = JsonUtility.FromJson<myChatList>(textJSON.text);
            fungusManager = GameObject.Find("FungusManager");

        } else if(instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }

    #region StoryIndex
    int CreateStoryIndexInt(int inRound, int inPatient)
    {
        return inRound * 100 + inPatient;
    }
    public int CreateStoryIndex(int inRound, string inCharacter)
    {
        CharacterName characterIndex = CharacterName.Naria;
        for(; characterIndex < CharacterName.Max; characterIndex++)
        {
            if (inCharacter == characterIndex.ToString())
            {
                break;
            }
        }

        Debug.Assert(0 < inRound && inRound < 3 && characterIndex != CharacterName.Max, "Wrong story round or character name in json file");

        return CreateStoryIndexInt((int)inRound, (int)characterIndex);
    }

    public int GetStoryIndex()
    {
        return CreateStoryIndexInt((int)currentRound, (int)currentCharacter);
    }

    #endregion

    //------------------ Sound Setting ------------------------------
    void SetSoundVolumeBGM() { }

}