using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [SerializeField] private LobbyDriver lobbyDriver;
    [SerializeField] private Animator neonAnimator;
    [SerializeField] private Image levelImageDisplay;
    [SerializeField] private Sprite levelImage;
    [SerializeField] private string levelName;

    private static LevelSelect selected;
   
    private Animator buttonAnimator;

    void Start() {
        buttonAnimator = (Animator)gameObject.GetComponent("Animator");
        levelImageDisplay.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (selected != this) {
            neonAnimator.Update(0);
            buttonAnimator.Play("Prehighlight");
            neonAnimator.Play("Prehighlight");
            levelImageDisplay.GetComponent<Image>().enabled = true;
            levelImageDisplay.sprite = levelImage;
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
            levelImageDisplay.GetComponent<Image>().enabled = false;
            if (selected != null) {
                levelImageDisplay.GetComponent<Image>().enabled = true;
                levelImageDisplay.sprite = selected.levelImage;
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
            lobbyDriver.SelectLevelByName("");
        } else {
            if (selected != null) {
                selected.buttonAnimator.Play("Normal");
            }
            selected = this;
            buttonAnimator.Play("Selected");
            neonAnimator.Play("Selected");
            lobbyDriver.SelectLevelByName(levelName);
        }
    }
}
