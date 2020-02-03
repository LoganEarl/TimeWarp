using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanManager : MonoBehaviour, IGameMode
{
    private bool begun = false;
    private ILevelConfig levelConfig = null;
    public int numPlayers;
    private int stepNumber = 0;
    private int matchNumber = 0; //starts at 0, but first match is 1. This is so NextMatch() doesnt need edge case checks

    private Dictionary<string, GameObject> loadedPlayerModels = new Dictionary<string, GameObject>();
    private List<GameObject> currentPlayers = new List<GameObject>();

    private static readonly int MAX_STEPS = 360;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void FixedUpdate()
    {
        if (begun)
        {
            stepNumber++;
            if (stepNumber > MAX_STEPS)
                NextMatch();
            //TODO: record player data
        }
    }

    private void NextMatch()
    {
        stepNumber = 0;
        matchNumber++;
        UnloadPlayers();
        LoadPlayers();   //reloads the scene with the new data
    }

    public void Setup(int numPlayers, ILevelConfig levelConfig)
    {
        this.levelConfig = levelConfig;
        this.numPlayers = numPlayers;
        SceneManager.LoadScene(levelConfig.GetSceneName(), LoadSceneMode.Single);
    }

    public void Begin()
    {
        if (levelConfig != null)
        {
            begun = true;
            NextMatch();
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelConfig.GetSceneName()));
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
            for(int matchCounter = 1; matchCounter <= matchNumber; matchCounter++)
            {
                string assetName = levelConfig.GetPlayerModelName(curPlayer, matchCounter);
                GameObject playerPrefab;
                if (loadedPlayerModels.ContainsKey(assetName))
                    playerPrefab = loadedPlayerModels[assetName];
                else
                {
                    playerPrefab = Resources.Load(assetName) as GameObject;
                    loadedPlayerModels[assetName] = playerPrefab;
                }
                //TODO: instantiate a game player. If matchCounter < matchNumber - 1, attach it to replay data
                currentPlayers.Add(Instantiate(playerPrefab, levelConfig.GetPlayerSpawnPosition(curPlayer, matchCounter), Quaternion.identity));
                //TODO: make sure that when matchCounter = matchNumber -1 that it gets attached to a controller, however that will work
            }
        }
    }
}