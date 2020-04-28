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
    private DisplayMode displayMode;

    public enum DisplayMode
    {
        SCALE_WITH_TIME, SCALE_WITH_DAMAGE
    }

    public void Setup(IGameMode attachedGameMode, GameObject attachedPlayer, int playerNumber, int roundNumber, DisplayMode displayMode)
    {
        this.displayMode = displayMode;
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
            Color? targetColor = null;
            if (playerHealth.Dead)
                targetColor = Color.black;
            else if(displayMode == DisplayMode.SCALE_WITH_TIME)
            {
                int maxSteps = attachedGameMode.GameState.MaxSteps;
                int curStep = attachedGameMode.GameState.StepNumber;
                float scale = curStep / (float)maxSteps * 0.8f + 0.2f;
                if (!attachedGameMode.GameState.TimeAdvancing || attachedGameMode.GameState.GetPlayerPositionsLocked(playerNumber,roundNumber))
                    scale = 0.2f;
                gameObject.transform.localScale = new Vector2(scale, 1);

                float healthScale = playerHealth.Health / (float)playerHealth.MaxHealth;
                healthScale = ColorCorrectScale(healthScale);
                targetColor = defaultColor * healthScale;
            } else if(displayMode == DisplayMode.SCALE_WITH_DAMAGE)
            {
                float healthScale = playerHealth.Health / (float)playerHealth.MaxHealth;
                float scale = 0.2f + 0.8f * healthScale;
                gameObject.transform.localScale = new Vector2(scale, 1);

                float colorScale = ColorCorrectScale(healthScale);
                targetColor = defaultColor * healthScale;
            }

            if (!fillImage.color.Equals(targetColor ?? defaultColor))
                fillImage.color = targetColor ?? defaultColor;
        }

    }

    private float ColorCorrectScale(float rawScale)
    {
        return -1 * (rawScale - 1) * (rawScale - 1) + 1; //correct for the bad way humans see color
    }
}
