using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/*
 * Interface for selecting multiple buttons. The selected button will stay highlighted.
 */

interface ButtonInteract
{
    void ButtonClicked(Button clickedButton);
    void SetButtonsInteractable(Button clickedButton);
}
