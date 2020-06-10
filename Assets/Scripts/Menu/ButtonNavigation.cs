using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonNavigation : MonoBehaviour {

    private static readonly float POSITIVE_AXIS = 1f; //0.5f;
    private static readonly float NEGATIVE_AXIS = -1f;//-0.5f;

    [SerializeField]
    private ButtonList[] panels;
    [SerializeField]
    Button firstButtonSelected;
    [SerializeField]
    private int currentSelection = 0;

    private int currentPanel = 0;
    private Button previousSelectedBeforeOptions;
    private Button selectedButton;

    private bool isNavigatable;
    private bool cooldown = false;
    private int panelArraySize;

    OptionsNavigation optionsNavigator;

    void Start()
    {
        EnableNavigatable();
        optionsNavigator = FindObjectOfType<AudioManager>().GetComponentInChildren<OptionsNavigation>();
        Invoke("StartFirstSelect", 1.0f);
        panelArraySize = panels.Length;
    }

    void Update()
    {
        if (isNavigatable)
        {
            if (!cooldown)
            {
                CheckUpAxis();
                CheckDownAxis();
                CheckLeftAxis();
                CheckRightAxis();
            }

            if (Input.GetButtonDown("Submit0"))
                FindObjectOfType<AudioManager>().PlaySFX("OnButtonClick");
        }
    }

    private void StartFirstSelect()
    {
        FindObjectOfType<AudioManager>().PlaySFX("OnButtonHover");
        firstButtonSelected.Select();
        this.selectedButton = firstButtonSelected;
    }

    #region CheckAxis
    private void CheckUpAxis()
    {
        if (Input.GetAxisRaw("Vertical0") >= POSITIVE_AXIS)
        {
            NavigateUp();
            StartCoroutine(InputCooldown());
        }
    }

    private void CheckDownAxis()
    {
        if (Input.GetAxisRaw("Vertical0") <= NEGATIVE_AXIS)
        {
            NavigateDown();
            StartCoroutine(InputCooldown());
        }
    }

    private void CheckLeftAxis()
    {
        if (Input.GetAxisRaw("Horizontal0") <= NEGATIVE_AXIS)
        {
            NavigateLeft();
            StartCoroutine(InputCooldown());
        }
    }

    private void CheckRightAxis()
    {
        if (Input.GetAxisRaw("Horizontal0") >= POSITIVE_AXIS)
        {
            NavigateRight();
            StartCoroutine(InputCooldown());
        }
    }
    #endregion

    #region Navigate
    private void Navigate()
    {
        Button temp = panels[currentPanel].GetButton(this.currentSelection);
        if (!temp.IsInteractable())
        {
            this.currentSelection = (this.currentSelection + 1) % panels[currentPanel].size();
            temp = panels[currentPanel].GetButton(this.currentSelection);
        }
        this.selectedButton = temp;
        selectedButton.Select();
    }

    private void NavigateDown()
    {
        int buttonArraySize = panels[currentPanel].size();
        this.currentSelection = (this.currentSelection + 1) % buttonArraySize;
        FindObjectOfType<AudioManager>().PlaySFX("OnButtonHover");
        Navigate();
    }

    private void NavigateUp()
    {
        int buttonArraySize = panels[currentPanel].size();
        this.currentSelection = (this.currentSelection - 1);
        if (this.currentSelection < 0)
        {
            this.currentSelection = buttonArraySize - 1;
            int i = 1;
            while (!this.panels[currentPanel].GetButton(this.currentSelection).IsInteractable())
            {
                this.currentSelection = buttonArraySize - i;
                i++;
            }
        } 
        else
            this.currentSelection = this.currentSelection % buttonArraySize;
        FindObjectOfType<AudioManager>().PlaySFX("OnButtonHover");
        Navigate();
    }

    private void NavigateRight()
    {
        if (this.currentPanel > panelArraySize)
            this.currentPanel = panelArraySize - 1;
        else if (currentPanel != panelArraySize - 1)
        {
            this.currentPanel = (this.currentPanel + 1) % panelArraySize;
            FindObjectOfType<AudioManager>().PlaySFX("OnButtonHover");
            this.currentSelection = 0;
        }
        Navigate();
    }

    private void NavigateLeft()
    {
        if (this.currentPanel < 0)
            this.currentPanel = 0;
        else if (this.currentPanel != 0)
        {
            this.currentPanel = (this.currentPanel - 1) % panelArraySize;
            FindObjectOfType<AudioManager>().PlaySFX("OnButtonHover");
            this.currentSelection = 0;
        }
        Navigate();
    }
    #endregion

    IEnumerator InputCooldown()
    {
        cooldown = true;
        yield return new WaitForSeconds(0.1f);
        cooldown = false;
    }

    public void SetPreviousSelection(Button prevButton)
    {
        this.previousSelectedBeforeOptions = prevButton;
    }
    public void OnOptions()
    {
        Button prevButton = panels[currentPanel].GetButton(currentSelection);
        SetPreviousSelection(prevButton);
        DisableNavigation();
        if (optionsNavigator == null)
            optionsNavigator = FindObjectOfType<AudioManager>().GetComponentInChildren<OptionsNavigation>();
        FindObjectOfType<AudioManager>().PlaySFX("OnButtonClick");
        optionsNavigator.OnOpen();
    }

    public void OnOptionsExit()
    {
        EnableNavigatable();
        previousSelectedBeforeOptions.Select();
    }

    public void EnableNavigatable()
    {
        this.isNavigatable = true;
    }

    public void DisableNavigation()
    {
        this.isNavigatable = false;
    }
}