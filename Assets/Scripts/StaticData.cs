using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAMEMODE
{
    SinglePlayer = 0,
    Multiplayer = 1,
    PreAssessment = 3,
    PostAssessment = 4
}

public enum TOPIC
{
    Computer = 0,
    Networking = 1,
    Software = 2
}

public enum DIFFICULTY
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}

public class StaticData : MonoBehaviour
{
    public GameObject sceneLoader;

    // These variables are exposed to all Scenes to pass data between them by DontDestroyOnLoad
    public GAMEMODE SelectedGameMode;

    public TOPIC SelectedTopic;

    public DIFFICULTY SelectedDifficulty;

    public bool IsPostAssessment;

    public Stack<int> SceneIndexHistory = new Stack<int>();

    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("UI Manager").Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
