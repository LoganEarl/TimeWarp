using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanManager : MonoBehaviour, IGameMode
{
    private bool begun = false;
    private ILevelConfig levelConfig = null;
    public int numPlayers;
    private int stepNumber = -1;
    private int roundNumber = -1; //starts at -1, but first match is 0. This is so NextMatch() doesnt need edge case checks

    private Dictionary<string, GameObject> loadedPlayerModels = new Dictionary<string, GameObject>();
    private List<GameObject> currentPlayers = new List<GameObject>();

    private List<PlayerController>[] playerControllers;
    //TODO: This is temporary. I want to redo this with actual replay objects later
    private List<List<PlayerSnapshot>>[] playerRecordings; //player,match index,current step state

    /*
    Player num Array[
        Round Number List[
            R1 Frames[
                Frame Snapshot,
                Frame Snapshot
            ]
            R2 Frames[
                Frame Snapshot,
                Frame Snapshot
            ]
        ]
    ]


     */


    private static readonly int MAX_STEPS = 360;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void FixedUpdate()
    {
        if (begun)
        {
            if (stepNumber > MAX_STEPS)
                NextMatch();
            stepNumber++;
            for (int curPlayer = 0; curPlayer < numPlayers; curPlayer++)
            {
                for(int roundCounter = 0; roundCounter <= roundNumber; roundCounter++)
                {
                    List<PlayerSnapshot> currentReplay = playerRecordings[curPlayer][roundCounter];
                    PlayerController currentController = playerControllers[curPlayer][roundCounter];

                    if (roundCounter == roundNumber)
                        currentReplay.Add(currentController.GetSnapshot());
                    else
                        currentController.SetSnapshot(currentReplay[stepNumber]);
                }
            }
        }
    }

    private void NextMatch()
    {
        stepNumber = -1;
        roundNumber++;
        UnloadPlayers();
        LoadPlayers();   //reloads the scene with the new data
    }

    public void Setup(int numPlayers, ILevelConfig levelConfig)
    {
        this.levelConfig = levelConfig;
        this.numPlayers = numPlayers;
        playerControllers = new List<PlayerController>[numPlayers];
        playerRecordings = new List<List<PlayerSnapshot>>[numPlayers];
    }

    public void Begin()
    {
        if (levelConfig != null)
            SceneManager.LoadScene(levelConfig.GetSceneName(), LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(levelConfig != null && scene.name.Equals(levelConfig.GetSceneName()))
        {
            begun = true;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelConfig.GetSceneName()));
            NextMatch();
        }   
    }

    private void UnloadPlayers()
    {
        foreach(GameObject obj in currentPlayers)
        {
            Destroy(obj);
        }
        currentPlayers.Clear();
    }

    private void LoadPlayers()
    {
        //instantiate matchNumber * numPlayers player prefabs in the current scene
        for(int curPlayer = 0; curPlayer < numPlayers; curPlayer++)
        {
            playerControllers[curPlayer] = new List<PlayerController>();
            if(playerRecordings[curPlayer] == null)
                playerRecordings[curPlayer] = new List<List<PlayerSnapshot>>();

            for(int roundCounter = 0; roundCounter <= roundNumber; roundCounter++)
            {
                string assetName = levelConfig.GetPlayerModelName(curPlayer, roundCounter);
                GameObject playerPrefab;
                if (loadedPlayerModels.ContainsKey(assetName))
                    playerPrefab = loadedPlayerModels[assetName];
                else
                {
                    playerPrefab = Resources.Load(assetName) as GameObject;
                    loadedPlayerModels[assetName] = playerPrefab;
                }

                GameObject player = Instantiate(playerPrefab, levelConfig.GetPlayerSpawnPosition(curPlayer, roundCounter), Quaternion.identity);
                currentPlayers.Add(player);
                PlayerController playerController = player.GetComponent<PlayerController>();
                playerController.SetPlayerInformation(curPlayer, roundCounter);

                playerController.SetUseSnapshots(roundCounter != roundNumber);
                playerControllers[curPlayer].Add(playerController);

                if(roundCounter == roundNumber)
                    playerRecordings[curPlayer].Add(new List<PlayerSnapshot>());
            }
        }
    }
}