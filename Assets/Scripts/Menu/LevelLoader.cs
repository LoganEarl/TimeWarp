using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 *  This class now loads the next scene by name and provides an animation transition.
 *  Set the sorting order in accordance to the project parameters.
 */

public class LevelLoader : MonoBehaviour {
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private Animator transition;
    [SerializeField]
    private int sortOrder;

    private float transitionTime = 1f;

    public void LoadNextLevel(string sceneName)
    {
        if (sceneName.Equals("Quit"))
            Application.Quit();
        StartCoroutine(LoadLevel(sceneName));
    }

    private IEnumerator LoadLevel(string sceneName)
    {
        canvas.GetComponent<Canvas>().sortingOrder = sortOrder;
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        yield return new WaitForSeconds(transitionTime);
        canvas.sortingOrder = 0;
    }
}
    