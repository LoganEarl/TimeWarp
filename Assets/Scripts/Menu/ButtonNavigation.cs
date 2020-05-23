using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonNavigation : MonoBehaviour {

    private static readonly float POSITIVE_AXIS = 0.5f;
    private static readonly float NEGATIVE_AXIS = -0.5f;

    [SerializeField]
    private ButtonList[] panels;
    [SerializeField]
    Button firstButtonSelected;
    [SerializeField]
    private int currentSelection = 0;

    private int currentPanel = 0;
    Button previousSelectedBeforeOptions;
    Button selectedButton;

    private bool isNavigatable = true;
    private bool cooldown = false;
    private int panelArraySize;

    void Start()
    {
        Invoke("StartFirstSelect", 1.0f);
        panelArraySize = panels.Length;
    }

    void Update()
    {
        if (!cooldown && isNavigatable)
        {
            CheckUpAxis();
            CheckDownAxis();
            CheckLeftAxis();
            CheckRightAxis();
        }
    }

    private void StartFirstSelect()
    {
        firstButtonSelected.Select();
    }

    private void CheckUpAxis()
    {
        if (Input.GetAxis("Vertical0") > POSITIVE_AXIS)
        { 
            NavigateUp();
            StartCoroutine(InputCooldown());
        }
    }

    private void CheckDownAxis()
    {
        if (Input.GetAxis("Vertical0") < NEGATIVE_AXIS)
        {
            NavigateDown();
            StartCoroutine(InputCooldown());
        }
    }

    private void CheckLeftAxis()
    {
        if (Input.GetAxis("Horizontal0") < NEGATIVE_AXIS)
        {
            NavigateLeft();
            StartCoroutine(InputCooldown());
        }
    }

    private void CheckRightAxis()
    {
        if (Input.GetAxis("Horizontal0") > POSITIVE_AXIS)
        {
            NavigateRight();
            StartCoroutine(InputCooldown());
        }
    }
    
    IEnumerator InputCooldown()
    {
        cooldown = true;
        yield return new WaitForSeconds(.2f);
        cooldown = false;
    }

    public void SetPreviousSelection(Button prevButton)
    {
        this.previousSelectedBeforeOptions = prevButton;
    }

    private void Navigate()
    {
        Button temp = panels[currentPanel].GetButton(currentSelection);
        if (!temp.IsInteractable())
            temp = panels[currentPanel].GetButton((currentSelection + 1) % panelArraySize);
        temp.Select();
    }

    private void NavigateDown()
    {
        int buttonArraySize = panels[currentPanel].size();
        this.currentSelection = (this.currentSelection + 1) % buttonArraySize;
        Navigate();
    }

    private void NavigateUp()
    { 
        int buttonArraySize = panels[currentPanel].size();
        this.currentSelection = (this.currentSelection - 1);
        if (this.currentSelection < 0)
            this.currentSelection = buttonArraySize - 1;
        else         
            this.currentSelection = this.currentSelection % buttonArraySize;
        Navigate();
    }

    private void NavigateRight()
    {
        if (this.currentPanel > panelArraySize)
            this.currentPanel = panelArraySize - 1;
        else if (currentPanel != panelArraySize - 1)
        {
            this.currentPanel = (this.currentPanel + 1) % panelArraySize;
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
            this.currentSelection = 0;
        }
        Navigate();
    }

    public void OnOptions()
    {
        Button prevButton = panels[currentPanel].GetButton(currentSelection);
        SetPreviousSelection(prevButton);
        SetNavigatable(false);
        FindObjectOfType<AudioManager>().GetComponentInChildren<OptionsNavigation>().OnOpen();
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
}