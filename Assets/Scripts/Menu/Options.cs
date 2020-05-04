using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Options {
    public Button[] buttons;
    public Slider[] sliders;
    public Toggle[] toggles;

    public Button GetButton(int index)
    {
        if (index < 0)
            return buttons[0];
        else if (index > buttons.Length)
            return buttons[buttons.Length - 1];
        return buttons[index];
    }

    public Slider GetSlider(int index)
    {
        if (index < 0)
            return sliders[0];
        else if (index > sliders.Length)
            return sliders[sliders.Length - 1];
        return sliders[index];
    }

    public Toggle GetToggle(int index)
    {
        if (index < 0)
            return toggles[0];
        else if (index > toggles.Length)
            return toggles[toggles.Length - 1];
        return toggles[index];
    }

    public int ButtonSize()
    {
        return buttons.Length;
    }

    public int SliderSize()
    {
        return sliders.Length;
    }

    public int ToggleSize()
    {
        return toggles.Length;
    }
}
