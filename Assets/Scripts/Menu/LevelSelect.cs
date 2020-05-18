using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [SerializeField] private Animator neonAnimator;
    [SerializeField] private Image lobbyLevelImage;
    [SerializeField] private Sprite buttonLevelImage;

    private static LevelSelect selected;
   
    private Animator buttonAnimator;

    void Start() {
        buttonAnimator = (Animator)gameObject.GetComponent("Animator");
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (selected != this) {
            neonAnimator.Update(0);
            buttonAnimator.Play("Prehighlight");
            neonAnimator.Play("Prehighlight");
            lobbyLevelImage.GetComponent<Image>().enabled = true;
            lobbyLevelImage.sprite = buttonLevelImage;
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
            lobbyLevelImage.GetComponent<Image>().enabled = false;
            if (selected != null) {
                lobbyLevelImage.GetComponent<Image>().enabled = true;
                lobbyLevelImage.sprite = selected.buttonLevelImage;
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
        } else {
            if (selected != null) {
                selected.buttonAnimator.Play("Normal");
            }
            selected = this;
            buttonAnimator.Play("Selected");
            neonAnimator.Play("Selected");
        }
    }
}
