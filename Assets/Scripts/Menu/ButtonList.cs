using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class ButtonList {
    [SerializeField]
    private Button[] buttons;

    public Button GetButton(int index)
    {
        if (index < 0)
            return buttons[0];
        else if (index > buttons.Length)
            return buttons[buttons.Length - 1];
        return buttons[index];
    }

    public int size()
    {
        return buttons.Length;
    }
}