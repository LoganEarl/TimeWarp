using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuDriver: MonoBehaviour
{
    public void GoToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void CancelGame()
    {
        Application.Quit();
    }
}
