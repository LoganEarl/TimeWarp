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
        gameIsPaused = false;
        StartCoroutine(UnPause());
    }

    IEnumerator UnPause()
    {
        yield return new WaitForSecondsRealtime(.7f);
        pauseOverlay.SetActive(false);
        Time.timeScale = 1f;
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

    IEnumerator HighlightButton()
    {
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
    }
}
