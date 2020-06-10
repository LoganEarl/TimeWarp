using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler {
    [SerializeField] private LobbyDriver lobbyDriver;
    [SerializeField] private Animator neonAnimator;
    [SerializeField] private Image levelImageDisplay;
    [SerializeField] private Sprite levelImage;
    [SerializeField] private string levelName;

    private static LevelSelect selected;

    private Button button;
    private Animator buttonAnimator;

    void Start() {
        button = (Button)gameObject.GetComponent("Button");
        buttonAnimator = (Animator)gameObject.GetComponent("Animator");
        levelImageDisplay.enabled = false;
        button.onClick.AddListener(OnClick);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        OnSelect(eventData);
    }

    public void OnPointerExit(PointerEventData eventData) {
        OnDeselect(eventData);
    }

    public void OnSelect(BaseEventData eventData) {
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

    public void OnDeselect(BaseEventData eventData) {
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

    public void OnClick() {
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
