using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstinctManager : MonoBehaviour, IGameMode
{
    private ILevelConfig levelConfig = null;

    [SerializeField] private int projectilesPerRound = 5;
    [SerializeField] private int equipmentPerRound = 1;
    [SerializeField] private SpawnerController spawnerController;
    [SerializeField] private AudioClip[] announcerClips;
    private PlayerCameraController cameraController = null;
    private InstinctHUDController hudController;
    

    private Dictionary<string, GameObject> loadedPlayerModels = new Dictionary<string, GameObject>();
    private InstinctPlayerManager[] playerManagers = null;

    private Dictionary<int, DeathSignature> playerDeaths = new Dictionary<int, DeathSignature>();

    private class DeathSignature
    {
        public int RoundNum { get; }
        public int StepNum { get; }
        public DeathSignature(int roundNum, int stepNum) => (RoundNum,StepNum) = (roundNum,stepNum);
    }

    //================================================Public Accessors
    [SerializeField] private float roundLengthSeconds = 20;
    private int RoundLength
    {
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
    public int RoundNumber { get; private set; } = -1;

    private InstinctGameState gameState;
    public IGameState GameState { get => gameState; }

    public PlayerManager GetPlayerManager(int playerNum)
    {
        if (playerNum < 0 || playerManagers == null || playerManagers.Length <= playerNum)
            throw new System.Exception("Illegal playernum passed to GetPlayerManager");
        return playerManagers[playerNum];
    }

    public GameObject GetPlayerObject(int playerNum, int roundNum)
    {
        if(playerNum < 0 || playerNum >= playerManagers.Length)
            throw new System.Exception("Illegal playerNum passed to GetPlayerObject");
        return playerManagers[playerNum].GetPlayerObject(roundNum);
    }

    public GameObject GameObject
    {
        get { return gameObject; }
    }

    //================================================Public Interface
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
        gameState = new StateInitializing(this);
        DontDestroyOnLoad(this);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (gameState is StateInitializing && scene.name.Equals(levelConfig.GetSceneName()))
        {
            SceneManager.SetActiveScene(scene);
            gameState.OnLeaveState();
            gameState = new StateSpawning(this);
            gameState.OnEnterState();
        }
    }

    private void FixedUpdate()
    {
        if (gameState != null)
            gameState.Tick();
    }

    //================================================Game States
    public abstract class InstinctGameState : IGameState
    {
        private protected InstinctManager manager;
        private protected InstinctGameState(InstinctManager manager) => (this.manager) = (manager);
        private protected abstract InstinctGameState NextState { get; }
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
        private protected void FinishState()
        {
            StepNumber = MaxSteps;
        }

        public virtual int StepNumber { get; private set; }
        public virtual bool GetPlayerVisible(int playerNum, int roundNum) => false;
        public virtual bool GetPlayerPositionsLocked(int playerNum, int roundNum) => true;
        public virtual bool GetPlayerLookLocked(int playerNum, int roundNum) => true;
        public virtual bool GetPlayerFireLocked(int playerNum, int roundNum) => true;
        public virtual bool GetPlayerCanTakeDamage(int playerNum, int roundNum) => roundNum == manager.RoundNumber;
        public virtual bool TimeAdvancing { get => false; }
        public virtual int MaxSteps { get; private protected set; } = 1;
        public virtual float SecondsRemaining { get => (MaxSteps - StepNumber) * Time.fixedDeltaTime; }
    }

    //active from the start of call to setup() to after the begin() call is finished executing
    internal class StateInitializing : InstinctGameState
    {
        public StateInitializing(InstinctManager manager) : base(manager) { }

        internal override void OnEnterState()
        {
            //Ensure any old data is cleared out
            manager.RoundNumber = -1;

            if (manager.playerManagers != null)
                foreach (InstinctPlayerManager manager in manager.playerManagers)
                    manager.DestroyAll();

            SceneManager.sceneLoaded -= manager.OnSceneLoaded;

            manager.cameraController?.Stop();

            //Begin initialization
            manager.playerManagers = new InstinctPlayerManager[manager.NumPlayers];
            for (int i = 0; i < manager.NumPlayers; i++)
            {
                manager.playerManagers[i] = new InstinctPlayerManager(i, manager.projectilesPerRound, manager.equipmentPerRound);
                manager.playerManagers[i].PlayerKilledListener = manager.OnPlayerKilled;
            }
        }
        private protected override InstinctGameState NextState { get => new StateSpawning(manager); }
    }

    //active between rounds while spawn pillars are rising, before players are visible
    internal class StateSpawning : InstinctGameState
    {
        private static readonly int STATE_LENGTH = 2 * 50;


        private readonly StateSpawned nextState;

        internal StateSpawning(InstinctManager manager) : base(manager)
        {
            MaxSteps = STATE_LENGTH;
            nextState = new StateSpawned(manager);
        }

        internal override void OnEnterState()
        {
            manager.RoundNumber++;
            manager.LoadNewPlayers();

            manager.spawnerController.BeginSpawnSequence(STATE_LENGTH, STATE_LENGTH + nextState.MaxSteps, manager.levelConfig.GetAllSpawnPositions());

            foreach (InstinctPlayerManager manager in manager.playerManagers)
                manager.ResetAll();

            //TODO when we have a hud controller for this
            //if (manager.hudController == null)
            //    manager.LoadHUD();
            //else
            //    manager.hudController.ReloadAll();
            //manager.hudController.gameObject.SetActive(true);

            PlayerController[] mainPlayers = new PlayerController[manager.NumPlayers];
            for (int i = 0; i < mainPlayers.Length; i++)
                mainPlayers[i] = manager.playerManagers[i].MainPlayer;

            if (manager.cameraController == null)
            {
                GameObject cameraControllerObject = GameObject.FindWithTag("SplitscreenController");
                if (cameraControllerObject != null)
                    manager.cameraController = cameraControllerObject.GetComponent<PlayerCameraController>();
            }

            if (manager.cameraController != null)
                manager.cameraController.Setup(manager, mainPlayers);
        }

        public override bool TimeAdvancing { get => true; }
        private protected override InstinctGameState NextState { get => nextState; }
        public override float SecondsRemaining { get => (MaxSteps - StepNumber + nextState.MaxSteps) * Time.fixedDeltaTime; }
    }

    //active from when players are spawned in to the point the match starts
    public class StateSpawned : InstinctGameState
    {
        private static readonly int STATE_LENGTH = 2 * 50;

        public StateSpawned(InstinctManager manager) : base(manager) { MaxSteps = STATE_LENGTH; }

        internal override void OnEnterState()
        {
            //TODO when we have sounds implemented 
            //manager.PlayAnnouncerRound();
        }

        internal override void OnLeaveState()
        {
            //TODO when we have sounds implemented 
            //manager.PlayAnnouncerFight();
        }

        private protected override InstinctGameState NextState { get => new StatePlaying(manager); }
        public override bool GetPlayerVisible(int playerNum, int roundNum) => 
            !manager.playerDeaths.ContainsKey(playerNum) || manager.playerDeaths[playerNum].RoundNum >= roundNum;
        public override bool GetPlayerLookLocked(int playerNum, int roundNum) => false;
        public override bool TimeAdvancing { get => true; }
    }

    internal class StatePlaying : InstinctGameState
    {
        public StatePlaying(InstinctManager manager) : base(manager)
        {
            MaxSteps = manager.RoundLength;
        }

        private protected override void OnTick()
        {
            foreach (InstinctPlayerManager player in manager.playerManagers)
                player.Step(StepNumber);
            if(manager.NumPlayers - manager.playerDeaths.Count <= 1)
                FinishState();
        }

        internal override void OnLeaveState()
        {
            foreach (InstinctPlayerManager player in manager.playerManagers)
            {
                player.FinishSequence();
                player.Step(0); //frontloads first frame of recorded data. Cuts down on visual glitches
            }
        }

        private protected override InstinctGameState NextState
        {
            get
            {
                if (manager.NumPlayers - manager.playerDeaths.Count <= 1)
                    return new StateFinished(manager);
                else
                    return new StateSpawning(manager);

            }
        }

        public override bool GetPlayerVisible(int playerNum, int roundNum) =>
            !manager.playerDeaths.ContainsKey(playerNum) || 
            manager.playerDeaths[playerNum].RoundNum > roundNum ||
            (manager.playerDeaths[playerNum].RoundNum == roundNum && manager.playerDeaths[playerNum].StepNum >= StepNumber);
        public override bool GetPlayerPositionsLocked(int playerNum, int roundNum) => !GetPlayerVisible(playerNum, roundNum);
        public override bool GetPlayerLookLocked(int playerNum, int roundNum) => !GetPlayerVisible(playerNum, roundNum);
        public override bool GetPlayerFireLocked(int playerNum, int roundNum) => !GetPlayerVisible(playerNum, roundNum);
        public override bool TimeAdvancing { get => true; }
    }

    internal class StateFinished : InstinctGameState
    {
        public StateFinished(InstinctManager manager) : base(manager) { }

        internal override void OnEnterState()
        {
            //manager.hudController.gameObject.SetActive(false);
            //manager.LoadScoreScreen();
        }

        private protected override InstinctGameState NextState { get => new StateInitializing(manager); }
    }

    //================================================Private Utilities
    private void LoadNewPlayers()
    {
        for (int curPlayer = 0; curPlayer < NumPlayers; curPlayer++)
        {
            InstinctPlayerManager manager = playerManagers[curPlayer];

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
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.SetPlayerInformation(
                curPlayer,
                RoundNumber,
                levelConfig.GetPlayerSpawnPosition(curPlayer, RoundNumber),
                this
            );

            if (!manager.RecordExistsForMatch(RoundNumber))
                manager.AppendNewRecording(playerController);

            if (playerDeaths.ContainsKey(curPlayer))
                player.SetActive(false);
        }
    }

    private void LoadHUD()
    {
        //TODO implement this
    }

    private void OnPlayerKilled(int playerNum)
    {
        playerDeaths.Add(playerNum, new DeathSignature(RoundNumber, GameState.StepNumber));
    }
}
