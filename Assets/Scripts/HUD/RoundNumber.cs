using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundNumber : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI roundNumberText;
    private IGameMode gameMode;
    private bool setup = false;

    public void Setup(IGameMode gameMode)
    {
        this.gameMode = gameMode;
        setup = true;
    }

    public void Update()
    {
        if (setup)
        {
            if (!gameMode.GameState.TimeAdvancing || gameMode.GameState.PlayersPositionsLocked)
                roundNumberText.text = "Get Ready";
            else
                roundNumberText.text = "Round " + (gameMode.RoundNumber + 1);
        }
    }
}
