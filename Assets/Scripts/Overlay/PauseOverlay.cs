using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseOverlay : MonoBehaviour {

    [SerializeField]
    private GameObject pauseOverlay;

    private Button selectButton;

    private IGameMode sourceGameMode;
    public static bool GameIsPaused;
    private EventSystem es;

    void Start()
    {
        es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }
    
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

    public void ResumeGame()
    {
        //Reset button animators back to normal state, otherwise buttons get stuck in the middle of the highlighted animation
        Animator[] animators = pauseOverlay.GetComponentsInChildren<Animator>(true);
        foreach (Animator animator in animators) {
            animator.Play("Normal");
            animator.Update(0);
        }

        StartCoroutine(UnPause());
        Canvas drawCanvas = gameObject.GetComponentInChildren<Canvas>();
        drawCanvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        drawCanvas.planeDistance = 1;
    }

    IEnumerator UnPause()
    {
        if (Input.GetButtonDown("Submit0"))
            FindObjectOfType<AudioManager>().PlaySFX("OnButtonClick");
        this.selectButton.GetComponent<Animator>().Play("Normal");
        es.SetSelectedGameObject(null);
        yield return null;
        GameIsPaused = false;
        Time.timeScale = 1f;
        pauseOverlay.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
        pauseOverlay.SetActive(true);
        StartCoroutine(HighlightButton());
    }

    public void RestartGame()
    {
        if (Input.GetButtonDown("Submit0"))
            FindObjectOfType<AudioManager>().PlaySFX("OnButtonClick");
        Destroy(gameObject);
        ResumeGame();
        sourceGameMode.Reset();
        Time.timeScale = 1f;
        sourceGameMode.Begin();
    }

    public void QuitGame()
    {
        if (Input.GetButtonDown("Submit0"))
            FindObjectOfType<AudioManager>().PlaySFX("OnButtonClick");
        ResumeGame();
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        Destroy(sourceGameMode.GameObject);
        Time.timeScale = 1f;
    }

    IEnumerator HighlightButton()
    {
        if(es == null)
            es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        es.SetSelectedGameObject(null);
        yield return null;
        selectButton = es.firstSelectedGameObject.GetComponent<Button>();
        selectButton.Select();
    }
}
