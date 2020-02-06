using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* 
 * This class focuses on transitioning between scenes of the game.
 */
 
public class MenuScreens : MonoBehaviour
{
    /* @GoNextGameScene()
     * 
     * This function loads to the next game scene based on build index.
     * Only use this function if the build settings are set correctly in 
     * order.
     * 
     */ 
    public void GoNextGameScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /* @GoPreviousGameScene()
     * 
     * This function loads the previous game scene based on build index.
     * Only use this function if the build settings are set correctly in 
     * order.
     * 
     */
    public void GoPreviousGameScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    /* @GoNextGameScene(string)
     * 
     * This function loads the scene based on the passed in string (should be a scene name).
     * NOTE: The scene can only be loaded if it exists in the build settings.
     * 
     */
    public void GoToLoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /* @CancelGame()
     * 
     * This function exits the game application. Will only occur on the Main Menu.
     * 
     */
    public void CancelGame()
    {
        //Debug.Log("Quit!");
        Application.Quit();
    }
}
