using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * This class focuses on toggling the buttons for level selection.
 */

public class LevelSelectorButtonInteract : MonoBehaviour, ButtonInteract {
    [SerializeField]
    private Button[] levelSelectorButtons;

    [SerializeField]
    private Animator animator;

    public void ButtonClicked(Button clickedButton)
    {
        //int buttonIndex = System.Array.IndexOf(levelSelectorButtons, clickedButton);

        //if (buttonIndex == -1)
        //    return;

        //SetButtonsInteractable(clickedButton);

        //clickedButton.interactable = false;
    }

    public void SetButtonsInteractable(Button clickedButton)
    {
        //foreach (Button button in levelSelectorButtons)
        //    button.interactable = true;
    }
}
