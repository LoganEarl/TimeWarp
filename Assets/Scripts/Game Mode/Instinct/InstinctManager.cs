using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstinctManager : MonoBehaviour, IGameMode
{
    private ILevelConfig levelConfig = null;

    [SerializeField] private SpawnerController spawnerController;
    //TODO [SerializeField] private AudioClip[] announcerClips;

    private Dictionary<string, GameObject> loadedPlayerModels = new Dictionary<string, GameObject>();


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

    private InstictGameState gameState;
    public IGameState GameState { get => gameState; }

    public int MaxShots(int playerNumber)
    {
        //TODO
        return 0;
    }

    public int MaxEquipment(int playerNumber)
    {
        //TODO
        return 0;
    }

    public int ShotsRemaining(int playerNumber)
    {
        //TODO
        return 0;
    }

    public int ProjectedShotsRemaining(int playerNumber)
    {
        //TODO
        return 0;
    }

    public int EquipmentRemaining(int playerNumber)
    {
        //TODO
        return 0;
    }

    public int ProjectedEquipmentRemaining(int playerNumber)
    {
        //TODO
        return 0;
    }

    public GameObject GetPlayerObject(int playerNum, int roundNum)
    {
        //TODO
        return null;
    }

    public GameObject GameObject
    {
        get { return gameObject; }
    }

    //================================================Public Interface
    public void Setup(int numPlayers, ILevelConfig levelConfig)
    {

    }

    public void Begin()
    {

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    private void FixedUpdate()
    {

    }

    //================================================Game States
    public abstract class InstictGameState : IGameState
    {
        private protected InstinctManager manager;
        private protected InstictGameState(InstinctManager manager) => (this.manager) = (manager);
        private protected abstract InstictGameState NextState { get; }
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
}
