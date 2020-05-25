using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* 
 * This class focuses on transitioning between scenes of the game.
 */

public class LobbyDriver : MonoBehaviour
{
    [SerializeField] private Animator neonAnimator;
    [SerializeField] private Button startButton;
    [SerializeField] private PlanManager planGameMode;
    [SerializeField] private InstinctManager instinctGameMode;

    private Animator startButtonAnimator;

    private ILevelConfig selectedLevel = null;
    private IGameMode selectedGameMode = null;

    // Added Start() in to make start button uninteractable at the beginning
    public void Start()
    {
        startButtonAnimator = startButton.GetComponent<Animator>();
        startButtonAnimator.Play("Disabled");
        startButton.interactable = false;
    }
   
    public void SelectLevelByName(string levelName)
    {
        if (levelName == "Bounce")
            selectedLevel = new BounceLevelConfig();
        else if (levelName == "Hex")
            selectedLevel = new HexLevelConfig();
        else if (levelName == "Lava")
            selectedLevel = new LavaLevelConfig();
        else
            selectedLevel = null;

        CheckLoadButtonAvailability();
    }

    public void SelectGameModeByName(string gameMode) {
        if (gameMode == "Plan")
            selectedGameMode = planGameMode;
        else if (gameMode == "Instinct")
            selectedGameMode = instinctGameMode;
        else
            selectedGameMode = null;

        CheckLoadButtonAvailability();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    private void CheckLoadButtonAvailability()
    {
        if (selectedLevel != null && selectedGameMode != null) {
            startButton.interactable = true;
            startButtonAnimator.Play("Normal");
            neonAnimator.Play("Selected");
        } else {
            startButton.interactable = false;
            startButtonAnimator.Play("Disabled");
            neonAnimator.Play("Normal");
        }
    }

    public void LoadSelectedGame()
    {
        if(selectedLevel != null && selectedGameMode != null)
        {
            selectedGameMode.Setup(2, selectedLevel);
            selectedGameMode.Begin();
        }
    }

    /*Extra part for demonstration purposes*/
    private void FixedUpdate()
    {
        if (Input.GetButtonDown("Reset"))
        {
            selectedGameMode.Reset();
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }
    /*-------------*/
}
