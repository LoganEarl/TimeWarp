using System.Collections;
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
    public Animator animator;
    public Button startButton;
    public PlanManager planGameMode;

    // Added Start() in to make start button uninteractable at the beginning
    public void Start()
    {
        startButton.interactable = false;
    }
    
    public void SelectTestLevel()
    {
        selectedLevel = new LevelOneConfig();
        CheckLoadButtonAvailability();
    }

    public void SelectPlanGameMode()
    {
        selectedGameMode = planGameMode;
        CheckLoadButtonAvailability();
    }

    // Added for levels and game modes not created yet
    public void SelectTestLevelWithIndex(int level) // maybe pass in levels by an index? or by name
    {
        if (level == 1)
            selectedLevel = new TestLevelConfig();
        else if(level == 2 || level == 3)
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
    // End added stuff

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
