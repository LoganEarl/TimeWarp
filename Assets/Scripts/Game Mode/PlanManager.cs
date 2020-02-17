using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanManager : MonoBehaviour, IGameMode
{
    private bool begun = false;
    private ILevelConfig levelConfig = null;
    private int stepNumber = -1;
    
    private Dictionary<string, GameObject> loadedPlayerModels = new Dictionary<string, GameObject>();
    private PlanPlayerManager[] playerManagers = null;
    private List<GameObject> playerObjects = new List<GameObject>();

    private static readonly int MAX_STEPS = 1000;

    //================================================Public Accessors

    public float SecondsRemaining
    {
        get
        {
            return (MAX_STEPS - stepNumber) * Time.fixedDeltaTime;
        }
    }

    public int NumPlayers { get; private set; }
    public int RoundNumber { get; private set; } = -1;      //starts at -1, but first match is 0.
                                                            //This is so NextMatch() doesnt need edge case checks
    public int ShotsRemaining(int playerNumber)
    {
        if (playerNumber < 0 || playerNumber >= playerManagers.Length)
            return 0;
        return playerManagers[playerNumber].AvailableProjectiles;
    }                                                

    public int ProjectedShotsRemaining(int playerNumber)
    {
        if (playerNumber < 0 || playerNumber >= playerManagers.Length)
            return 0;
        return playerManagers[playerNumber].ProjectedProjectilesRemaining(stepNumber);
    }

    //================================================Unity Callback Methods
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

            foreach (PlanPlayerManager player in playerManagers)
                player.Step(stepNumber);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (levelConfig != null && scene.name.Equals(levelConfig.GetSceneName()))
        {
            begun = true;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelConfig.GetSceneName()));
            NextMatch();
        }
    }

    //================================================Public Interface
    public void Setup(int numPlayers, ILevelConfig levelConfig)
    {
        this.levelConfig = levelConfig;
        this.NumPlayers = numPlayers;

        playerManagers = new PlanPlayerManager[numPlayers];
        for (int i = 0; i < numPlayers; i++)
            playerManagers[i] = new PlanPlayerManager(10);
    }

    public void Begin()
    {
        if (levelConfig != null)
            SceneManager.LoadScene(levelConfig.GetSceneName(), LoadSceneMode.Single);
    }

    //================================================Private Utilities
    private void NextMatch()
    {
        stepNumber = -1;
        RoundNumber++;

        if (RoundNumber < levelConfig.GetMaxRounds())
        {
            foreach (PlanPlayerManager player in playerManagers)
                player.FinishSequence();

            foreach (GameObject playerObject in playerObjects)
                playerObject.GetComponent<PlayerController>().OnReset();

            LoadNewPlayers();
        }
    }

    private void LoadNewPlayers()
    {
        for (int curPlayer = 0; curPlayer < NumPlayers; curPlayer++)
        {
            PlanPlayerManager manager = playerManagers[curPlayer];

            string assetName = levelConfig.GetPlayerModelName(curPlayer, RoundNumber);
            GameObject playerPrefab;

            if (loadedPlayerModels.ContainsKey(assetName))
                playerPrefab = loadedPlayerModels[assetName];
            else
            {
                playerPrefab = Resources.Load(assetName) as GameObject;
                loadedPlayerModels[assetName] = playerPrefab;
            }

            GameObject player = Instantiate(playerPrefab);
            playerObjects.Add(player);
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.SetPlayerInformation(
                curPlayer,
                RoundNumber,
                levelConfig.GetPlayerSpawnPosition(curPlayer, RoundNumber)
            );

            if (!manager.RecordExistsForMatch(RoundNumber))
                manager.AppendNewRecording(playerController);
        }
    }
}

