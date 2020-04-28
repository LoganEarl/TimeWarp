using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonNavigation : MonoBehaviour, ISelectHandler, IDeselectHandler, ICancelHandler {

    [SerializeField]
    Button[] buttons;

    EventSystem firstSelectedButton;
    EventSystem currentSelectedButton;

    [SerializeField]
    private Button selectOnTop, selectOnBottom, selectOnLeft, selectOnRight;
    [SerializeField]
    private Slider bgmSlider, sfxSlider, voiceSlider;

    void Start()
    {
        selectOnTop = GetComponent<Button>();
        selectOnBottom = GetComponent<Button>();
        selectOnLeft = GetComponent<Button>();
        selectOnRight = GetComponent<Button>();
    }

    public void OnCancel(BaseEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnSelect(BaseEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
