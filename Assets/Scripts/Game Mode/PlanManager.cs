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

    private bool betweenRounds = true;
    private readonly int betweenRoundsTime = 3 * 50;        //3 seconds

    //================================================Public Accessors

    public float SecondsRemaining
    {
        get
        {
            if (betweenRounds)
                return (betweenRoundsTime - StepNumber) * Time.fixedDeltaTime;
            return (MaxSteps - StepNumber) * Time.fixedDeltaTime;
        }
    }

    public int StepNumber { get; private set; } = -1;
    public int MaxSteps { get; private set; } = 5 * 50;    //20 seconds
    public int NumPlayers { get; private set; }
    public int MaxRounds { get; private set; } = 1;
    public bool GameEnabled { private set; get; } = false;
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

    //================================================Public Interface
    public void Setup(int numPlayers, ILevelConfig levelConfig)
    {
        //Ensure old data is cleared out
        this.levelConfig = levelConfig;
        this.NumPlayers = numPlayers;
        this.MaxRounds = levelConfig.GetMaxRounds();
        RoundNumber = -1;
        StepNumber = -1;
        betweenRounds = true;
        GameEnabled = false;

        if (hudController != null) Destroy(hudController);

        if(playerManagers != null)
            foreach(PlanPlayerManager manager in playerManagers)
                manager.DestroyAll();

        playerObjects = new List<GameObject>();

        //Begin initialization
        playerManagers = new PlanPlayerManager[numPlayers];
        for (int i = 0; i < numPlayers; i++)
            playerManagers[i] = new PlanPlayerManager(18, 2);
    }

    public void Begin()
    {
        if (levelConfig != null)
            SceneManager.LoadScene(levelConfig.GetSceneName(), LoadSceneMode.Single);
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
            if (!betweenRounds && StepNumber > MaxSteps)
                NextMatch();
            if(betweenRounds && StepNumber > betweenRoundsTime)
            {
                betweenRounds = false;
                StepNumber = -1;
            }

            GameEnabled = !betweenRounds;

            StepNumber++;

            if (!betweenRounds)
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

    //================================================Private Utilities
    private void NextMatch()
    {
        StepNumber = -1;
        RoundNumber++;
        betweenRounds = true;

        if (RoundNumber < levelConfig.GetMaxRounds())
        {
            foreach (PlanPlayerManager player in playerManagers)
                player.FinishSequence();

            foreach (GameObject playerObject in playerObjects)
                playerObject.GetComponent<PlayerController>().OnReset();

            foreach (PlanPlayerManager player in playerManagers)    //frontload the first frame of recorded data
                player.Step(0);
            LoadNewPlayers();
            hudController.ReloadAll();

        }
        else
        {
            Setup(NumPlayers, levelConfig);
            Begin();
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
                levelConfig.GetPlayerSpawnPosition(curPlayer, RoundNumber),
                this
            );

            if (!manager.RecordExistsForMatch(RoundNumber))
                manager.AppendNewRecording(playerController);
        }
    }

    private void LoadHUD()
    {
        string canvasPath = "Prefabs/HUD/HUDFrame";
        GameObject loaded = Resources.Load(canvasPath) as GameObject;
        if (loaded == null)
            throw new System.Exception("Unable to find HUDFrame component. Have you renamed/moved it?");
        loaded = Instantiate(loaded);
        hudController = loaded.GetComponent<PlanHUDController>();
        hudController.Setup(this);
    }
}

