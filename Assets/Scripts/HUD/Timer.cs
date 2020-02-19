using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI timerText;

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
            timerText.text = string.Format("{0:F1}", gameMode.SecondsRemaining);
    }
}
