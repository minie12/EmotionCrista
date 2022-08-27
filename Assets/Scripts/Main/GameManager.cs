using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterChat{
    public string name;
    public string chat;
}
[System.Serializable]
public class myChatList{
    public CharacterChat[] characterChat;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState state;

    public TextAsset textJSON;
    public myChatList myChatList = new myChatList();

    // Start is called before the first frame update
    void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        if(Instance == null) // If there is no instance already
        {
            Debug.Log("inst");
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
            Instance = this;  
            myChatList = JsonUtility.FromJson<myChatList>(textJSON.text);
        } else if(Instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }
}

public enum GameState{
    StartMenu,
    StoryMode,
    MiniGameMode
}