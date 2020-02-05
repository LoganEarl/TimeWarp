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
