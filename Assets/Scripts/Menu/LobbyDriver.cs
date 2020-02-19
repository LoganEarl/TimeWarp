﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* 
 * This class focuses on transitioning between scenes of the game.
 */

public class LobbyDriver : MonoBehaviour {
    private ILevelConfig selectedLevel = null;
    private IGameMode selectedGameMode = null;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private PlanManager planGameMode;

    // Added Start() in to make start button uninteractable at the beginning
    public void Start()
    {
        startButton.interactable = false;

        //Remove all code underneath here to have lobby interactivity again.
        planGameMode.Setup(2, new LevelOneConfig());
        planGameMode.Begin();
    }
    
    public void SelectTestLevel()
    {
        selectedLevel = new TestLevelConfig();
        CheckLoadButtonAvailability();
    }

    public void SelectPlanGameMode()
    {
        selectedGameMode = planGameMode;
        CheckLoadButtonAvailability();
    }

    public void SelectTestLevelWithIndex(int level) // maybe pass in levels by an index? or by name
    {
        if (level == 1)
            selectedLevel = new TestLevelConfig();
        else if(level == 2)
            selectedLevel = new LevelOneConfig();
        else if(level == 3)
            selectedLevel = null;

        CheckLoadButtonAvailability();
    }

    public void SelectGameMode(string gameModeName)
    {
        if (gameModeName.Equals("Plan"))
            selectedGameMode = planGameMode;
        else
            selectedGameMode = null;

        CheckLoadButtonAvailability();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void CheckLoadButtonAvailability()
    {
        startButton.interactable = (selectedLevel != null && selectedGameMode != null);
    }

    public void LoadSelectedGame()
    {
        if(selectedLevel != null && selectedGameMode != null)
        {
            selectedGameMode.Setup(2, selectedLevel);
            selectedGameMode.Begin();
        }
    }
}
