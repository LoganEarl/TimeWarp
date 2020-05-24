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
    [SerializeField]
    private bool isLobbyMenu;

    private int currentPanel = 0;
    private Button previousSelectedBeforeOptions;
    private Button selectedButton;
    private Button levelButton;

    private bool isNavigatable = true;
    private bool cooldown = false;
    private int panelArraySize;

    OptionsNavigation optionsNavigator;

    void Start()
    {
        optionsNavigator = FindObjectOfType<AudioManager>().GetComponentInChildren<OptionsNavigation>();
        Invoke("StartFirstSelect", 1.0f);
        panelArraySize = panels.Length;
    }

    void Update()
    {
        if(isNavigatable)
            CurrentButtonStaySelected();

        if (!cooldown && isNavigatable)
        {          
            CheckUpAxis();
            CheckDownAxis();
            CheckLeftAxis();
            CheckRightAxis();
        }

        if(isLobbyMenu)
            this.selectedButton.onClick.AddListener(SetLevelButton);
    }

    private void StartFirstSelect()
    {
        FindObjectOfType<AudioManager>().PlaySFX("OnButtonHover");
        firstButtonSelected.Select();
        this.selectedButton = firstButtonSelected;
        CheckButtonSelectAnimation();
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
        CheckButtonSelectAnimation();
    }

    private void NavigateDown()
    {
        int buttonArraySize = panels[currentPanel].size();
        CheckButtonExitAnimation();
        this.currentSelection = (this.currentSelection + 1) % buttonArraySize;
        FindObjectOfType<AudioManager>().PlaySFX("OnButtonHover");
        Navigate();
    }

    private void NavigateUp()
    {
        int buttonArraySize = panels[currentPanel].size();
        CheckButtonExitAnimation();
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
        CheckButtonSelectAnimation();
        ResetAnimations();
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
    private void CurrentButtonStaySelected()
    {
        this.selectedButton = panels[currentPanel].GetButton(currentSelection);
        this.selectedButton.Select();
    }

    public void OnOptions()
    {
        Button prevButton = panels[currentPanel].GetButton(currentSelection);
        SetPreviousSelection(prevButton);
        SetNavigatable(false);
        if (optionsNavigator == null)
            optionsNavigator = FindObjectOfType<AudioManager>().GetComponentInChildren<OptionsNavigation>();
        optionsNavigator.OnOpen();
    }

    public void OnOptionsExit()
    {
        SetNavigatable(true);
        previousSelectedBeforeOptions.Select();
    }

    public void SetNavigatable(bool state)
    {
        isNavigatable = state;
    }

    #region ButtonAnimation
    private void CheckButtonSelectAnimation()
    {
        if (isLobbyMenu)
            if (this.selectedButton == GameObject.Find("BounceButton").GetComponent<Button>() && levelButton != selectedButton)
                selectedButton.GetComponent<LevelSelect>().GetComponent<Animator>().Play("Prehighlight");
            else if (this.selectedButton == GameObject.Find("HexButton").GetComponent<Button>() && levelButton != selectedButton)
                selectedButton.GetComponent<LevelSelect>().GetComponent<Animator>().Play("Prehighlight");
            else if (this.selectedButton == GameObject.Find("LavaButton").GetComponent<Button>() && levelButton != selectedButton)
                selectedButton.GetComponent<LevelSelect>().GetComponent<Animator>().Play("Prehighlight");
    }

    private void CheckButtonClickAnimation()
    {
        if (isLobbyMenu)
            if (this.selectedButton == GameObject.Find("BounceButton").GetComponent<Button>())
                selectedButton.GetComponent<LevelSelect>().GetComponent<Animator>().Play("Selected");
            else if (this.selectedButton == GameObject.Find("HexButton").GetComponent<Button>())
                selectedButton.GetComponent<LevelSelect>().GetComponent<Animator>().Play("Selected");
            else if (this.selectedButton == GameObject.Find("LavaButton").GetComponent<Button>())
                selectedButton.GetComponent<LevelSelect>().GetComponent<Animator>().Play("Selected");
    }

    private void CheckButtonExitAnimation()
    {
        if (isLobbyMenu)
            if (this.selectedButton == GameObject.Find("BounceButton").GetComponent<Button>() && levelButton != selectedButton)
                selectedButton.GetComponent<LevelSelect>().GetComponent<Animator>().Play("Normal");
            else if (this.selectedButton == GameObject.Find("HexButton").GetComponent<Button>() && levelButton != selectedButton)
                selectedButton.GetComponent<LevelSelect>().GetComponent<Animator>().Play("Normal");
            else if (this.selectedButton == GameObject.Find("LavaButton").GetComponent<Button>() && levelButton != selectedButton)
                selectedButton.GetComponent<LevelSelect>().GetComponent<Animator>().Play("Normal");
    }
    #endregion

    private void SetLevelButton()
    {
        if(isLobbyMenu)
            if(this.selectedButton == GameObject.Find("BounceButton").GetComponent<Button>() ||
                this.selectedButton == GameObject.Find("HexButton").GetComponent<Button>() ||
                this.selectedButton == GameObject.Find("LavaButton").GetComponent<Button>())
                this.levelButton = this.selectedButton;

        ResetAnimations();
    }

    private void ResetAnimations()
    {
        Button temp;
        if (isLobbyMenu)
            for (int i = 0; i < 3; i++)
            {
                temp = this.panels[0].GetButton(i);
                if(this.levelButton != temp)
                    temp.GetComponent<LevelSelect>().GetComponent<Animator>().Play("Normal");
            }
    }
}