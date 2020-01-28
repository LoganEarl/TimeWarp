using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanManager : MonoBehaviour, IGameMode
{
    private ILevel level;
    private readonly int numPlayers;
    private int stepNumber = 0;
    private int matchNumber = 0; //starts at 0, but first match is 1. This is so RollMatchNumber() doesnt need edge cases

    private static readonly int MAX_STEPS = 1000;

    private PlanManager(ILevel level, int numPlayers)
    {
        this.level = level;
        this.numPlayers = numPlayers;
    }

    void FixedUpdate()
    {
        stepNumber++;
        if (stepNumber > MAX_STEPS)
            RollMatchNumber();
    }

    private void RollMatchNumber()
    {
        stepNumber = 0;
        matchNumber++;
        LoadAssets();   //reloads the scene with the new data
    }
    
    public void LoadAssets()
    {
        //first load the bound level's scene
        SceneManager.LoadScene(level.GetSceneName());
        //instantiate matchNumber * numPlayers player prefabs in the current scene
        for(int curPlayer = 0; curPlayer < numPlayers; curPlayer++)
        {
            for(int matchCounter = 1; matchCounter <= matchNumber; matchCounter++)
            {
                //TODO: instantiate a game player. If matchCounter < matchNumber - 1, attach it to replay data
                //TODO: make sure that when matchCounter = matchNumber -1 that it gets attached to a controller, however that will work
            }
        }
    }

    public class PlanBuilder
    {
        private ILevel level;
        private int numPlayers = 2;

        public PlanBuilder SetLevel(ILevel level)
        {
            this.level = level;
            return this;
        }

        public PlanBuilder SetNumPlayers(int numPlayers)
        {
            this.numPlayers = numPlayers;
            return this;
        }

        public PlanManager Build()
        {
            return new PlanManager(level, numPlayers);
        }
    }
}