using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class focuses on toggling the buttons for game mode.
 */

public class GameModeButtonInteract : MonoBehaviour, ButtonInteract {
    [SerializeField]
    private Button[] gameModeButtons;

    public void ButtonClicked(Button clickedButton)
    {
        //int buttonIndex = System.Array.IndexOf(gameModeButtons, clickedButton);

        //if (buttonIndex == -1)
        //    return;

        //SetButtonsInteractable(clickedButton);

        //clickedButton.interactable = false;
    }

    public void SetButtonsInteractable(Button clickedButton)
    {
        //foreach (Button button in gameModeButtons)
        //    button.interactable = true;
    }
}
