using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuDriver: MonoBehaviour
{
    /*Remove the Start Function if you want to test the menus.
    private void Start()
    {
        GoToLobby();
    }
    
    /*Ending extraneous start function*/
    public void GoToLobby()
    {
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void CancelGame()
    {
        Application.Quit();
    }
}
