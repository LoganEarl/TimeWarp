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
    private bool runningRecordingRound = false;
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
    public int MaxSteps { get; private set; } = 15 * 50;    //15 seconds
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
        if (betweenRounds)
            return playerManagers[playerNumber].ProjectedProjectilesRemaining(0);
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
        if(betweenRounds)
            return playerManagers[playerNumber].ProjectedEquipmentRemaining(0);
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

    public GameObject GameObject
    {
        get { return gameObject; }
    }

    //================================================Public Interface
    //both resets the current state of things and sets up for a new game. To start, follow with a call to Begin()
    public void Setup(int numPlayers, ILevelConfig levelConfig)
    {
        //Ensure any old data is cleared out
        this.levelConfig = levelConfig;
        this.NumPlayers = numPlayers;
        this.MaxRounds = levelConfig.GetMaxRounds();
        this.runningRecordingRound = false;
        RoundNumber = -1;
        StepNumber = -1;
        betweenRounds = true;
        GameEnabled = false;
        begun = false;

        if (hudController != null) Destroy(hudController.gameObject);

        if(playerManagers != null)
            foreach(PlanPlayerManager manager in playerManagers)
                manager.DestroyAll();

        playerObjects = new List<GameObject>();

        SceneManager.sceneLoaded -= OnSceneLoaded;

        //Begin initialization
        playerManagers = new PlanPlayerManager[numPlayers];
        for (int i = 0; i < numPlayers; i++)
            playerManagers[i] = new PlanPlayerManager(18, 1);
    }

    public void Begin()
    {
        if (levelConfig != null)
        {
            string sceneName = levelConfig.GetSceneName();
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene == null || !scene.isLoaded)
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            else
                OnSceneLoaded(scene, LoadSceneMode.Single);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }    
    }

    public void Reset()
    {
        Setup(NumPlayers, levelConfig);
    }

    //================================================Unity Callback Methods
    private void Awake()
    {
        DontDestroyOnLoad(this);
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
            SceneManager.SetActiveScene(scene);
            LoadHUD();
            NextMatch();
            begun = true;
        }
    }

    //================================================Private Utilities
    private void NextMatch()
    {
        StepNumber = -1;

        RoundNumber++;
        if (RoundNumber == levelConfig.GetMaxRounds() && !runningRecordingRound)
        {
            runningRecordingRound = true;
            RoundNumber--;
        }
            
        betweenRounds = true;

        if (RoundNumber < levelConfig.GetMaxRounds())
        {
            foreach (PlanPlayerManager player in playerManagers)
                player.FinishSequence();

            foreach (GameObject playerObject in playerObjects)
                playerObject.GetComponent<PlayerController>().OnReset();

            foreach (PlanPlayerManager player in playerManagers)    //frontload the first frame of recorded data
                player.Step(0);

            if(!runningRecordingRound)
                LoadNewPlayers();
            hudController.ReloadAll();

        }
        else
        {
            GameEnabled = false;
            begun = false;
            hudController.gameObject.SetActive(false);
            DoScoreScreen();
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
        if (hudController != null)
            Destroy(hudController.gameObject);

        string canvasPath = "Prefabs/HUD/HUDFrame";
        GameObject loaded = Resources.Load(canvasPath) as GameObject;
        if (loaded == null)
            throw new System.Exception("Unable to find HUDFrame component. Have you renamed/moved it?");
        loaded = Instantiate(loaded);
        hudController = loaded.GetComponent<PlanHUDController>();
        hudController.Setup(this);
    }

    private void DoScoreScreen()
    {
        string[] scoreKeyOrder = { "Unused Ammo", "Unused Sheilds", "Damage Taken", "Time Alive", "Survival" };
        int[] scores = new int[NumPlayers];
        Dictionary<string, int>[] listings = new Dictionary<string, int>[NumPlayers];

        for(int i = 0; i < NumPlayers; i++)
        {
            int remainingAmmo = playerManagers[i].AvailableProjectiles;
            int remainingEquip = playerManagers[i].AvailableEquipment;
            int remainingPlayers = playerManagers[i].NumberRecordingsAlive;
            int remainingHealth = playerManagers[i].TotalHealthRemaining;
            int totalTimeAlive = playerManagers[i].TotalTimeAlive; 

            Dictionary<string, int> stats = new Dictionary<string, int>();
            stats["Unused Ammo"] = remainingAmmo * 100;
            stats["Unused Sheilds"] = remainingEquip * 200;
            stats["Survival"] = remainingPlayers * 10000;
            stats["Damage Taken"] = remainingHealth * 500;
            stats["Time Alive"] = totalTimeAlive;

            int total = 0;
            foreach (int value in stats.Values)
                total += value;

            scores[i] = total;
            listings[i] = stats;
        }

        ScoreOverlay.ScoreList scoreListings = new ScoreOverlay.ScoreList(scores, listings, scoreKeyOrder);
        string canvasPath = "Prefabs/Overlay/ScoreOverlay";
        GameObject loaded = Resources.Load(canvasPath) as GameObject;
        if (loaded == null)
            throw new System.Exception("Unable to find ScoreOverlay component. Have you renamed/moved it?");
        loaded = Instantiate(loaded);
        loaded.GetComponent<ScoreOverlay>().Setup(scoreListings, this);
    }
}

