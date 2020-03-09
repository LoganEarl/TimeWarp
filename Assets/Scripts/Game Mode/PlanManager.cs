using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanManager : MonoBehaviour, IGameMode
{
    private ILevelConfig levelConfig = null;

    [SerializeField] private SpawnerController spawnerController;

    private Dictionary<string, GameObject> loadedPlayerModels = new Dictionary<string, GameObject>();
    private PlanPlayerManager[] playerManagers = null;
    private List<GameObject> playerObjects = new List<GameObject>();
    private PlanHUDController hudController;

    private bool runningRecordingRound = false;

    //================================================Public Accessors
    [SerializeField] private float roundLengthSeconds = 20;
    private int RoundLength {
        get
        {
            return (int)(roundLengthSeconds * 50);
        }
        set
        {
            roundLengthSeconds = value / 50.0f;
        }
    }
    public int NumPlayers { get; private set; }
    public int MaxRounds { get; private set; } = 1;
    public int RoundNumber { get; private set; } = -1;      //starts at -1, but first match is 0.
                                                            //This is so NextMatch() doesnt need edge case checks
    private PlanGameState gameState;
    public IGameState GameState { get => gameState; }

    public int MaxShots(int playerNumber)
    {
        if (playerNumber < 0 || playerNumber >= playerManagers.Length)
            return 0;
        return playerManagers[playerNumber].MaxProjectiles;
    }

    public int MaxEquipment(int playerNumber)
    {
        if (playerNumber < 0 || playerNumber >= playerManagers.Length)
            return 0;
        return playerManagers[playerNumber].MaxEquipment;
    }

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
        return playerManagers[playerNumber].ProjectedProjectilesRemaining(GameState.StepNumber);
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
        return playerManagers[playerNumber].ProjectedEquipmentRemaining(GameState.StepNumber);
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
        this.levelConfig = levelConfig;
        this.NumPlayers = numPlayers;
        this.MaxRounds = levelConfig.GetMaxRounds();
        gameState = new StateInitializing(this);
        gameState.OnEnterState();
    }

    public void Begin()
    {
        if (gameState is StateInitializing)
        {
            string sceneName = levelConfig.GetSceneName();
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene == null || !scene.isLoaded)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
            else
                OnSceneLoaded(scene, LoadSceneMode.Single);
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
        gameState = new StateInitializing(this);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (gameState is StateInitializing && scene.name.Equals(levelConfig.GetSceneName()))
        {
            SceneManager.SetActiveScene(scene);
            gameState.OnLeaveState();
            gameState = new StateSpawning(true, this);
            gameState.OnEnterState();
        }
    }

    private void FixedUpdate()
    {
        if (gameState != null)
            gameState.Tick();
    }

    //================================================Game States
    public abstract class PlanGameState : IGameState
    {
        private protected PlanManager manager;
        private protected PlanGameState(PlanManager manager)
        {
            this.manager = manager;
        }

        private protected abstract PlanGameState NextState { get; }
        internal virtual void OnEnterState() { }
        internal virtual void OnLeaveState() { }
        internal void Tick()
        {
            if (TimeAdvancing) StepNumber++;
            if (StepNumber >= MaxSteps)
            {
                manager.gameState = NextState;
                OnLeaveState();
                manager.gameState.OnEnterState();
            }
            else if (TimeAdvancing)
                OnTick();
        }
        private protected virtual void OnTick() { }

        public virtual int StepNumber { get; private set; }
        public virtual bool PlayersVisible { get => false; }
        public virtual bool PlayersPositionsLocked { get => true; }
        public virtual bool PlayersLookLocked { get => true; }
        public virtual bool PlayersFireLocked { get => true; }
        public virtual bool TimeAdvancing { get => false; }
        public virtual int MaxSteps { get; private protected set; } = 1;
        public virtual float SecondsRemaining { get => (MaxSteps - StepNumber) * Time.fixedDeltaTime; }
    }

    //active from the start of call to setup() to after the begin() call is finished executing
    internal class StateInitializing : PlanGameState
    {
        public StateInitializing(PlanManager manager) : base(manager) { }

        internal override void OnEnterState()
        {
            //Ensure any old data is cleared out
            manager.runningRecordingRound = false;
            manager.RoundNumber = -1;

            if (manager.playerManagers != null)
                foreach (PlanPlayerManager manager in manager.playerManagers)
                    manager.DestroyAll();

            manager.playerObjects = new List<GameObject>();

            SceneManager.sceneLoaded -= manager.OnSceneLoaded;

            //Begin initialization
            manager.playerManagers = new PlanPlayerManager[manager.NumPlayers];
            for (int i = 0; i < manager.NumPlayers; i++)
                manager.playerManagers[i] = new PlanPlayerManager(18, 1);
        }
        private protected override PlanGameState NextState { get => new StateSpawning(true, manager); }
    }

    //active between rounds while spawn pillars are rising, before players are visible
    internal class StateSpawning : PlanGameState
    {
        private static readonly int STATE_LENGTH = 2 * 50;
        private readonly bool spawnNewPlayers;

        private readonly StateSpanwed nextState;

        internal StateSpawning(bool spawnNewPlayers, PlanManager manager) : base(manager)
        {
            this.spawnNewPlayers = spawnNewPlayers;
            MaxSteps = STATE_LENGTH;
            nextState = new StateSpanwed(manager);
        }

        internal override void OnEnterState()
        {
            if (spawnNewPlayers)
            {
                manager.RoundNumber++;
                manager.LoadNewPlayers();
            }

            manager.spawnerController.BeginSpawnSequence(STATE_LENGTH, STATE_LENGTH + nextState.MaxSteps, manager.levelConfig.GetAllSpawnPositions());

            foreach (PlanPlayerManager manager in manager.playerManagers)
                manager.ResetAll();

            if (manager.hudController == null)
                manager.LoadHUD();
            else
                manager.hudController.ReloadAll();
            manager.hudController.gameObject.SetActive(true);
        }

        public override bool TimeAdvancing { get => true; }
        private protected override PlanGameState NextState { get => new StateSpanwed(manager); }
        public override float SecondsRemaining { get => (MaxSteps - StepNumber + nextState.MaxSteps) * Time.fixedDeltaTime; }
    }

    //active from when players are spawned in to the point the match starts
    public class StateSpanwed : PlanGameState
    {
        private static readonly int STATE_LENGTH = 2 * 50;

        public StateSpanwed(PlanManager manager) : base(manager) { MaxSteps = STATE_LENGTH; }


        private protected override PlanGameState NextState { get => new StatePlaying(manager); }
        public override bool PlayersVisible { get => true; }
        public override bool PlayersLookLocked { get => false; }
        public override bool TimeAdvancing { get => true; }
    }

    internal class StatePlaying : PlanGameState
    {
        public StatePlaying(PlanManager manager) : base(manager)
        {
            MaxSteps = manager.RoundLength;
        }

        private protected override void OnTick()
        {
            foreach (PlanPlayerManager player in manager.playerManagers)
                player.Step(StepNumber);
        }

        internal override void OnLeaveState()
        {
            if (!manager.runningRecordingRound)
                foreach (PlanPlayerManager player in manager.playerManagers)
                {
                    player.FinishSequence();
                    player.Step(0); //frontloads first frame of recorded data. Cuts down on visual glitches
                }

            manager.runningRecordingRound = manager.RoundNumber >= manager.MaxRounds - 1;
        }

        private protected override PlanGameState NextState
        {
            get
            {
                if (manager.runningRecordingRound)
                    return new StateFinished(manager);
                else
                    return new StateSpawning(manager.RoundNumber < manager.MaxRounds - 1, manager);

            }
        }

        public override bool PlayersVisible { get => true; }
        public override bool PlayersPositionsLocked { get => false; }
        public override bool PlayersLookLocked { get => false; }
        public override bool PlayersFireLocked { get => false; }
        public override bool TimeAdvancing { get => true; }
    }

    internal class StateFinished : PlanGameState
    {
        public StateFinished(PlanManager manager) : base(manager) { }

        internal override void OnEnterState()
        {
            manager.hudController.gameObject.SetActive(false);
            manager.LoadScoreScreen();
        }

        private protected override PlanGameState NextState { get => new StateInitializing(manager); }
    }


    //================================================Private Utilities
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

    private void LoadScoreScreen()
    {
        string[] scoreKeyOrder = { "Unused Ammo", "Unused Sheilds", "Damage Taken", "Time Alive", "Survival" };
        int[] scores = new int[NumPlayers];
        Dictionary<string, int>[] listings = new Dictionary<string, int>[NumPlayers];

        for (int i = 0; i < NumPlayers; i++)
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

