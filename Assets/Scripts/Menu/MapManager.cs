using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ELocationName
{
    ELN_FirstDayDorm,
    ELN_Dormitory,
    ELN_DormCorridor,
    ELN_Lobby,
    ELN_LabCorridor,
    ELN_Storage,
    ELN_CounselRoom,
    ELN_Garden,
    ELN_Incinerator,
    ELN_MAX
}

public class MapManager : MonoBehaviour
{
    public Dictionary<string, List<ELocationName>> availableLocationMap;

    public List<GameObject> GO_locations;

    private void Awake()
    {
        availableLocationMap = new Dictionary<string, List<ELocationName>>();

        List<ELocationName> fromDormitoryList = new List<ELocationName>();
        fromDormitoryList.Add(ELocationName.ELN_DormCorridor);

        List<ELocationName> fromDormCorridorList = new List<ELocationName>();
        fromDormCorridorList.Add(ELocationName.ELN_FirstDayDorm);
        fromDormCorridorList.Add(ELocationName.ELN_Dormitory);
        fromDormCorridorList.Add(ELocationName.ELN_Lobby);

        List<ELocationName> fromLobbyList = new List<ELocationName>();
        fromLobbyList.Add(ELocationName.ELN_DormCorridor);
        fromLobbyList.Add(ELocationName.ELN_LabCorridor);

        List<ELocationName> fromLabCorridorList = new List<ELocationName>();
        fromLobbyList.Add(ELocationName.ELN_Lobby);
        fromLobbyList.Add(ELocationName.ELN_Storage);
        fromLobbyList.Add(ELocationName.ELN_CounselRoom);

        List<ELocationName> fromStorageList = new List<ELocationName>();
        fromLobbyList.Add(ELocationName.ELN_LabCorridor);

        availableLocationMap.Add("MyFirstDorm", fromDormitoryList);
        availableLocationMap.Add("Dormitory", fromDormitoryList);
        availableLocationMap.Add("DormCorridor", fromDormCorridorList);
        availableLocationMap.Add("Lobby", fromLobbyList);
        availableLocationMap.Add("LabCorridor", fromLabCorridorList);
    }

    public void OnSceneLoaded(string inSceneName)
    {
        List<ELocationName> availableLocations;

        foreach (GameObject GO_location in GO_locations)
        {
            // GO_location lock all locations
        }

        if (availableLocationMap.TryGetValue(inSceneName, out availableLocations))
        {
            foreach (ELocationName locationName in availableLocations)
            {
                if ((int)locationName < GO_locations.Count)
                {
                    // if garden / fireplace / storage, check if it is unlocked 
                    // GO_locations[(int)locationName] unlocked
                }
            }
        }
    }
}
