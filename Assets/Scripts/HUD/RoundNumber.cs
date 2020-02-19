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
            roundNumberText.text = "Round " + gameMode.RoundNumber;
    }
}
