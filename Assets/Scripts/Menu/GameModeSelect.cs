using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameModeSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [SerializeField] private LobbyDriver lobbyDriver;
    [SerializeField] private Animator neonAnimator;
    [SerializeField] private Text gameModeDescDisplay;
    [SerializeField] private string gameMode;

    private static GameModeSelect selected;
   
    private Animator buttonAnimator;

    void Start() {
        buttonAnimator = GetComponent<Animator>();
        gameModeDescDisplay.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (selected != this) {
            neonAnimator.Update(0);
            buttonAnimator.Play("Prehighlight");
            neonAnimator.Play("Prehighlight");
            SetGameDescription(gameMode);
            if (selected != null) {
                selected.buttonAnimator.Play("Interrupted");
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (selected != this) {
            neonAnimator.Update(0);
            buttonAnimator.Play("Normal");
            neonAnimator.Play("Normal");
            SetGameDescription("");
            if (selected != null) {
                SetGameDescription(selected.gameMode);
                selected.buttonAnimator.Play("Selected");              
                neonAnimator.Play("Selected");
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (selected == this) {
            selected = null;
            buttonAnimator.Play("Highlighted");
            neonAnimator.Play("Highlighted");
            lobbyDriver.SelectGameModeByName("");
        } else {
            if (selected != null) {
                selected.buttonAnimator.Play("Normal");
            }
            selected = this;
            buttonAnimator.Play("Selected");
            neonAnimator.Play("Selected");
            lobbyDriver.SelectGameModeByName(gameMode);
        }
    }

    public void SetGameDescription(string gameMode) {
        if (!gameMode.Equals("") || gameMode != null)
            if (gameMode.Equals("Plan"))
                gameModeDescDisplay.text = ThePlanDescription();
            else if (gameMode.Equals("Lineage"))
                gameModeDescDisplay.text = LineageDescription();
            else if (gameMode.Equals("Instinct"))
                gameModeDescDisplay.text = InstinctDescription();
            else
                gameModeDescDisplay.text = "";
    }

    private string ThePlanDescription() {
        return "A gamemode that leverages the time travel feature to allow players to plan " +
               "out their turn in relative safety, protect their past selves, and attempt " +
               "to eliminate the enemy player’s past selves.";
    }

    private string LineageDescription() {
        return "A gamemode where the objective is to keep your past selves alive, while " +
               "attempting to eliminate one of your opponents past self. If your present " +
               "self dies, time warps back, and you get a chance to save yourself.";
    }

    private string InstinctDescription() {
        return "Only your current self, and that of your enemy matter. Say alive as you " +
                "both gain more and more iterations, and the field grows thick with the " +
                "projectiles of your past selves.";
    }
}
