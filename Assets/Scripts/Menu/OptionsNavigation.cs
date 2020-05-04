using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsNavigation : MonoBehaviour {

    private static readonly float POSITIVE_AXIS = 0.5f;
    private static readonly float NEGATIVE_AXIS = -0.5f;
    private static readonly int BUTTON = 0;
    private static readonly int SLIDER = 1;
    private static readonly int TOGGLE = 2;

    [SerializeField]
    private Options panels;

    private int currentType = SLIDER;
    private int currentButton = 0;
    private int currentSlider = 0;
    private int currentToggle = 0;
    private bool isOnBackButton = false;
    private bool cooldown = false;
    private bool isNavigatable = true;
    GameObject buttonNavigation;


    void Start()
    {
        buttonNavigation = GameObject.Find("Navigator");
        Navigate();
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

    private void Navigate()
    {
        Button buttonTemp;
        Slider sliderTemp;
        Toggle toggleTemp;

        if (currentType == BUTTON)
        {
            buttonTemp = panels.GetButton(this.currentButton);
            buttonTemp.Select();
        }
        else if (currentType == SLIDER)
        {
            sliderTemp = panels.GetSlider(this.currentSlider);
            sliderTemp.Select();
        }
        else
        {
            toggleTemp = panels.GetToggle(this.currentToggle);
            toggleTemp.Select();
        }
    }

    private void NavigateDown()
    {
        if (this.currentType == BUTTON)
        {
            this.currentButton = (this.currentButton + 1) % this.panels.ButtonSize();
            if (this.currentButton == 0)
            {
                this.currentType = SLIDER;
                this.currentSlider = 0;
            }
        }
        else if(this.currentType == SLIDER)
        {
            this.currentSlider = (this.currentSlider + 1) % this.panels.SliderSize();
            if (this.currentSlider == 0)
            {
                this.currentType = TOGGLE;
                this.currentToggle = 0;
            }
        }
        else
        {
            this.currentToggle = (this.currentToggle + 1) % this.panels.ToggleSize();
            if(this.currentToggle == 0)
            {
                this.currentType = BUTTON;
                this.currentButton = 0;
            }
        }
        Navigate();
    }

    private void NavigateUp()
    {
        if (this.currentType == BUTTON)
        {
            if (this.currentButton == 0)
            {
                this.currentType = TOGGLE;
                this.currentToggle = this.panels.ToggleSize() - 1;
            }
            else
                this.currentButton = (this.currentButton - 1) % this.panels.ButtonSize();
        }
        else if (this.currentType == SLIDER)
        {
            if (this.currentSlider == 0)
            {
                this.currentType = BUTTON;
                this.currentButton = this.panels.ButtonSize() - 1;
            }
            else
                this.currentSlider = (this.currentSlider - 1) % this.panels.SliderSize();
        }
        else
        {
            if (this.currentToggle == 0)
            {
                this.currentType = SLIDER;
                this.currentSlider = this.panels.SliderSize() - 1;
            }
            else
                this.currentToggle = (this.currentToggle - 1) % this.panels.ToggleSize();
        }

        Navigate();
    }

    private void NavigateRight()
    {
        if (currentType == SLIDER)
            this.panels.GetSlider(currentSlider).value += 0.1f;
        else if (currentType == BUTTON)
            this.panels.GetButton(this.currentButton).Select();
    }

    private void NavigateLeft()
    {
        if (currentType == SLIDER)
            this.panels.GetSlider(currentSlider).value -= 0.1f;
        else if (currentType == BUTTON)
            this.panels.GetButton(this.currentButton).Select();
    }

    public void OnBack()
    {
        isNavigatable = false;
        if(buttonNavigation == null)
            buttonNavigation = GameObject.Find("Navigator");
        buttonNavigation.GetComponent<ButtonNavigation>().OnOptionsExit();
    }

    public void OnOpen()
    {
        isNavigatable = true;
    }
}
