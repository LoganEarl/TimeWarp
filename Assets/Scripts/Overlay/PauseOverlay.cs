using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseOverlay : MonoBehaviour {

    [SerializeField]
    private GameObject pauseOverlay;
    [SerializeField]
    private EventSystem eventSystem;
    [SerializeField]
    private Button selectButton;

    private IGameMode sourceGameMode;
    private bool gameIsPaused;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            if (!gameIsPaused)
                PauseGame();
            else
                ResumeGame();
    }

    public void Setup(IGameMode iGameMode)
    {
        this.sourceGameMode = iGameMode;
    }

    public bool GameIsPaused()
    {
        return gameIsPaused;
    }

    public void ResumeGame()
    {
        selectButton.Select();
        StartCoroutine(UnPause());
    }

    IEnumerator UnPause()
    {
        yield return new WaitForSecondsRealtime(.7f);
        gameIsPaused = false;
        Time.timeScale = 1f;
        pauseOverlay.SetActive(false);
        
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        gameIsPaused = true;
        pauseOverlay.SetActive(true);
        StartCoroutine(HighlightButton());
    }

    public void RestartGame()
    {
        Destroy(gameObject);
        ResumeGame();
        sourceGameMode.Reset();
        Time.timeScale = 1f;
        sourceGameMode.Begin();
    }

    public void QuitGame()
    {
        ResumeGame();
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        Destroy(sourceGameMode.GameObject);
        Time.timeScale = 1f;
    }

    IEnumerator HighlightButton()
    {
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
    }
}
