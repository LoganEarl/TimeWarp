using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler {

    private Button button { get { return GetComponent<Button>(); } }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(button.IsInteractable() && button.IsActive())
            FindObjectOfType<AudioManager>().PlaySFX("OnButtonHover");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.IsInteractable() && button.IsActive())
            FindObjectOfType<AudioManager>().PlaySFX("OnButtonClick");
    }

    public void PlayOnSelect()
    {
        FindObjectOfType<AudioManager>().PlaySFX("OnButtonHover");
    }

    public void PlayOnClick()
    {
        FindObjectOfType<AudioManager>().PlaySFX("OnButtonClick");
    }
 }
