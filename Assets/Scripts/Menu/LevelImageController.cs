using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelImageController : MonoBehaviour
{
    [SerializeField]
    private Sprite[] levelImg;
    [SerializeField]
    private Image imageToDisplay;

    public void Start()
    {
        DisableImage();
    }

    public void DisplayImage(int index)
    {
        if (index < 0 || index > levelImg.Length)
            DisableImage();
        else
        {
            imageToDisplay.GetComponent<Image>().enabled = true;
            imageToDisplay.sprite = levelImg[index];
        }
    }

    private void DisableImage()
    {
        imageToDisplay.GetComponent<Image>().enabled = false;
    }
}
