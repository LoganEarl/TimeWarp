using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScoreNavigation : MonoBehaviour
{
    public EventSystem es;
    private Button selectButton;

    void Start()
    {
        //es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        StartCoroutine(HighlightButton());
    }

    IEnumerator HighlightButton()
    {
        if (es == null)
            es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        es.SetSelectedGameObject(null);
        yield return null;
        selectButton = es.firstSelectedGameObject.GetComponent<Button>();
        selectButton.Select();
    }

    public void OnPressed()
    {
        StartCoroutine(ResetAnim());
    }

    IEnumerator ResetAnim()
    {
        this.selectButton.GetComponent<Animator>().Play("Normal");
        if(Input.GetButtonDown("Submit0"))
            FindObjectOfType<AudioManager>().PlaySFX("OnButtonClick");
        es.SetSelectedGameObject(null);
        yield return null;
    }

}
