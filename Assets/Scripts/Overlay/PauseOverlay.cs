using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseOverlay : MonoBehaviour {

    [SerializeField]
    private GameObject pauseOverlay;

    private IGameMode sourceGameMode;
    private bool GameIsPaused;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            if (!GameIsPaused)
                PauseGame();
            else
                ResumeGame();
    }

    public void Setup(IGameMode iGameMode)
    {
        this.sourceGameMode = iGameMode;
    }

    public bool GetPausedState()
    {
        return GameIsPaused;
    }

    public void ResumeGame()
    {
        //Reset button animators back to normal state, otherwise buttons get stuck in the middle of the highlighted animation
        Animator[] animators = pauseOverlay.GetComponentsInChildren<Animator>(true);
        foreach (Animator animator in animators) {
            animator.Play("Normal");
            animator.Update(0);
        }
        pauseOverlay.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void PauseGame()
    {
        pauseOverlay.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void RestartGame()
    {
        Destroy(gameObject);
        sourceGameMode.Reset();
        ResumeGame();
        sourceGameMode.Begin();
    }

    public void QuitGame()
    {
        Destroy(sourceGameMode.GameObject);
        ResumeGame();
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
