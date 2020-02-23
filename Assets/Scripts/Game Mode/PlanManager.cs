using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanManager : MonoBehaviour, IGameMode
{
    private bool begun = false;
    private ILevelConfig levelConfig = null;
    
    private Dictionary<string, GameObject> loadedPlayerModels = new Dictionary<string, GameObject>();
    private PlanPlayerManager[] playerManagers = null;
    private List<GameObject> playerObjects = new List<GameObject>();
    private PlanHUDController hudController;

    //================================================Public Accessors

    public float SecondsRemaining
    {
        get
        {
            return (MaxSteps - StepNumber) * Time.fixedDeltaTime;
        }
    }

    public int StepNumber { get; private set; } = -1;
    public int MaxSteps { get; private set; } = 1000;
    public int NumPlayers { get; private set; }
    public int MaxRounds { get; private set; } = 1;
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
        return playerManagers[playerNumber].ProjectedProjectilesRemaining(StepNumber);
    }

    public int MaxShots(int playerNumber)
    {
        if (playerNumber < 0 || playerNumber >= playerManagers.Length)
            return 0;
        return playerManagers[playerNumber].MaxProjectiles;
    }

    public int EquipmentRemaining(int playerNumber)
    {
        if (playerNumber < 0 || playerNumber >= playerManagers.Length)
            return 0;
        return playerManagers[playerNumber].AvailableEquipment;
    }

    public int ProjectedEquipmentRemaining(int playerNumber)
    {
        if (playerNumber < 0 || playerNumber >= playerManagers.Length)
            return 0;
        return playerManagers[playerNumber].ProjectedEquipmentRemaining(StepNumber);
    }

    public int MaxEquipment(int playerNumber)
    {
        if (playerNumber < 0 || playerNumber >= playerManagers.Length)
            return 0;
        return playerManagers[playerNumber].MaxEquipment;
    }

    public GameObject GetPlayerObject(int playerNum, int roundNum)
    {
        if (playerNum < 0 || playerNum >= NumPlayers || roundNum < 0 || roundNum > RoundNumber)
            throw new System.Exception("Passed illegal playernum or roundNumber to getPlayerObject");
        return playerManagers[playerNum].GetPlayerObject(roundNum);
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
            if (StepNumber > MaxSteps)
                NextMatch();

            StepNumber++;

            foreach (PlanPlayerManager player in playerManagers)
                player.Step(StepNumber);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (levelConfig != null && scene.name.Equals(levelConfig.GetSceneName()))
        {
            begun = true;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelConfig.GetSceneName()));
            LoadHUD();
            NextMatch();
           
        }
    }

    //================================================Public Interface
    public void Setup(int numPlayers, ILevelConfig levelConfig)
    {
        this.levelConfig = levelConfig;
        this.NumPlayers = numPlayers;
        this.MaxRounds = levelConfig.GetMaxRounds();

        playerManagers = new PlanPlayerManager[numPlayers];
        for (int i = 0; i < numPlayers; i++)
            playerManagers[i] = new PlanPlayerManager(18,2);
    }

    public void Begin()
    {
        if (levelConfig != null)
            SceneManager.LoadScene(levelConfig.GetSceneName(), LoadSceneMode.Single);
    }

    //================================================Private Utilities
    private void NextMatch()
    {
        StepNumber = -1;
        RoundNumber++;

        if (RoundNumber < levelConfig.GetMaxRounds())
        {
            foreach (PlanPlayerManager player in playerManagers)
                player.FinishSequence();

            foreach (GameObject playerObject in playerObjects)
                playerObject.GetComponent<PlayerController>().OnReset();

            LoadNewPlayers();
            hudController.ReloadAll();   
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

    private void LoadHUD()
    {
        string canvasPath = "Prefabs/HUD/UIFrame";
        GameObject loaded = Resources.Load(canvasPath) as GameObject;
        loaded = Instantiate(loaded);
        hudController = loaded.GetComponent<PlanHUDController>();
        hudController.Setup(this);
    }
}

