using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class maintains the health bar of a player. Taking damage will cause the
 * slider to change in value in accordance to how much health that player has left.
 * Other classes may reference this to control the UI of the player health bar.
 */

public class HealthBar : MonoBehaviour {
    private IGameMode attachedGameMode;
    private int playerNumber;
    private int roundNumber;
    private bool setup = false;
    private PlayerHealth playerHealth;

    public void Setup(IGameMode attachedGameMode, GameObject attachedPlayer, int playerNumber, int roundNumber)
    {
        this.attachedGameMode = attachedGameMode;
        this.playerNumber = playerNumber;
        this.roundNumber = roundNumber;
        this.playerHealth = attachedPlayer.GetComponent<PlayerHealth>();
        setup = true;
    }

    public void Update()
    {
        if (setup)
        {
            if (playerHealth.Dead)
            {
                //TODO: change color based on damage
            }
            else
            {
                int maxSteps = attachedGameMode.MaxSteps;
                int curStep = attachedGameMode.StepNumber;
                float scale = curStep / (float)maxSteps * 0.8f + 0.2f;

                gameObject.transform.localScale = new Vector2(scale,1);

                //TODO: change color based on damage
            }
        }
    }
}
