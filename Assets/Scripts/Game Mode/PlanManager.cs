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
    private int roundNumber = -1; //starts at -1, but first match is 0.
                                  //This is so NextMatch() doesnt need edge case checks

    private Dictionary<string, GameObject> loadedPlayerModels = new Dictionary<string, GameObject>();
    private PlanPlayerManager[] playerManagers = null;
    private List<GameObject> playerObjects = new List<GameObject>();

    private static readonly int MAX_STEPS = 200;

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

    private void NextMatch()
    {
        stepNumber = -1;
        roundNumber++;

        if (roundNumber < levelConfig.GetMaxRounds())
        {
            foreach (PlanPlayerManager player in playerManagers)
                player.FinishSequence();

            foreach (GameObject playerObject in playerObjects)
                playerObject.GetComponent<PlayerController>().OnReset();

            LoadNewPlayers();
        }
    }

    public void Setup(int numPlayers, ILevelConfig levelConfig)
    {
        this.levelConfig = levelConfig;
        this.numPlayers = numPlayers;

        playerManagers = new PlanPlayerManager[numPlayers];
        for (int i = 0; i < numPlayers; i++)
            playerManagers[i] = new PlanPlayerManager(10);
    }

    public void Begin()
    {
        if (levelConfig != null)
            SceneManager.LoadScene(levelConfig.GetSceneName(), LoadSceneMode.Single);
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

    private void LoadNewPlayers()
    {
        for (int curPlayer = 0; curPlayer < numPlayers; curPlayer++)
        {
            PlanPlayerManager manager = playerManagers[curPlayer];

            string assetName = levelConfig.GetPlayerModelName(curPlayer, roundNumber);
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
                roundNumber, 
                levelConfig.GetPlayerSpawnPosition(curPlayer,roundNumber)
            );

            if (!manager.RecordExistsForMatch(roundNumber))
                manager.AppendNewRecording(playerController);
        }
    }

    private class PlanPlayerManager
    {
        private List<MatchRecordingManager> playerRecordings = new List<MatchRecordingManager>();
        public int AvailableProjectiles { internal set; get; }

        internal PlanPlayerManager(int availableProjectiles)
        {
            this.AvailableProjectiles = availableProjectiles;
        }

        internal void AppendNewRecording(PlayerController controller)
        {
            playerRecordings.Add(new MatchRecordingManager(controller));
        }

        internal void Step(int stepNumber)
        {
            foreach (MatchRecordingManager recording in playerRecordings)
            {
                recording.AppendNextSnapshot();
                recording.UtilizeFrame(stepNumber);
            }
        }

        internal void FinishSequence()
        {
            foreach (MatchRecordingManager recording in playerRecordings)
                recording.Finish();
        }

        internal bool RecordExistsForMatch(int matchNum)
        {
            return playerRecordings.Count > matchNum && matchNum >= 0;
        }
    }

    private class MatchRecordingManager
    {
        private bool recordingComplete = false;
        private PlayerController playerController;
        private List<PlayerSnapshot> snapshots = new List<PlayerSnapshot>();

        internal MatchRecordingManager(PlayerController playerController)
        {
            this.playerController = playerController;
            playerController.SetUseSnapshots(false);
        }

        internal void AppendNextSnapshot()
        {
            if (!recordingComplete)
                snapshots.Add(playerController.GetSnapshot());
        }

        internal PlayerSnapshot UtilizeFrame(int frameNum)
        {
            if (recordingComplete && snapshots.Count > 0)
            {
                if (frameNum >= 0 && frameNum < snapshots.Count)
                    playerController.SetSnapshot(snapshots[frameNum]);
                else if (frameNum < 0)
                    playerController.SetSnapshot(snapshots[0]);
                else
                    playerController.SetSnapshot(snapshots[snapshots.Count - 1]);
            }
            return null;
        }

        internal void Finish()
        {
            recordingComplete = true;
            playerController.SetUseSnapshots(true);
        }
    }
}

