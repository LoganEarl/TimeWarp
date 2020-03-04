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
    [SerializeField] private Animator animator;
    [SerializeField] private Button startButton;
    [SerializeField] private PlanManager planGameMode;

    private ILevelConfig selectedLevel = null;
    private IGameMode selectedGameMode = null;

    // Added Start() in to make start button uninteractable at the beginning
    public void Start()
    {
        startButton.interactable = false;

        /*---------- For Testing Purposes to skip main menu ---- 
                            //just remove the '*' at the end of ^ line to disable this block and '/' after '*' here.
        SelectPlanGameMode();
        SelectTestLevelWithIndex(2);
        LoadSelectedGame();
        /*---------- End Testing Purposes block ----*/
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
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
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

    /*Extra part for demonstration purposes*/
    private void FixedUpdate()
    {
        if (Input.GetButtonDown("Reset"))
        {
            //Destroy(this.gameObject, 1f);
            selectedGameMode.Reset();
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }
    /*-------------*/
}
