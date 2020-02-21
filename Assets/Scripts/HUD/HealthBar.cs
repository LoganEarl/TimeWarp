using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class maintains the health bar of a player. Taking damage will cause the
 * slider to change in value in accordance to how much health that player has left.
 * Other classes may reference this to control the UI of the player health bar.
 */

public class HealthBar : MonoBehaviour
{
    private IGameMode attachedGameMode;
    private int playerNumber;
    private int roundNumber;
    private bool setup = false;
    private PlayerHealth playerHealth;

    private Image fillImage;
    private Image borderImage;
    private Color defaultColor;

    public void Setup(IGameMode attachedGameMode, GameObject attachedPlayer, int playerNumber, int roundNumber)
    {
        this.attachedGameMode = attachedGameMode;
        this.playerNumber = playerNumber;
        this.roundNumber = roundNumber;
        this.playerHealth = attachedPlayer.GetComponent<PlayerHealth>();
        Image[] childImages = gameObject.GetComponentsInChildren<Image>();
        if (childImages.Length != 2)
            throw new System.Exception("HUD health component images not found. Make sure to update the Healthbar script if you adjust these assets");
        borderImage = childImages[0];
        fillImage = childImages[1];
        defaultColor = fillImage.color;
        setup = true;
    }

    public void Update()
    {
        if (setup)
        {
            Color targetColor;
            if (playerHealth.Dead)
                targetColor = Color.black;
            else
            {
                int maxSteps = attachedGameMode.MaxSteps;
                int curStep = attachedGameMode.StepNumber;
                float scale = curStep / (float)maxSteps * 0.8f + 0.2f;
                scale = -1 * (scale - 1) * (scale - 1) + 1; //correct for the bad way humans see color
                gameObject.transform.localScale = new Vector2(scale, 1);

                float healthScale = playerHealth.Health / (float)playerHealth.MaxHealth;
                targetColor = defaultColor * healthScale;
            }
            if (!targetColor.Equals(fillImage.color))
                fillImage.color = targetColor;
        }

    }
}
