using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AmmoBar : MonoBehaviour
{
    private IGameMode attachedGameMode;
    private int playerNumber;
    private bool setup = false;

    [SerializeField]
    private GameObject imageActualShots;
    [SerializeField]
    private GameObject imageProjectedShots;
    [SerializeField]
    private TextMeshProUGUI textAmmo;

    public void Setup(IGameMode attachedGameMode, int playerNumber)
    {
        this.playerNumber = playerNumber;
        this.attachedGameMode = attachedGameMode;
        setup = true;
    }

    public void Update()
    {
        int roundNumber = attachedGameMode.RoundNumber;
        int step = attachedGameMode.GameState.StepNumber;
        int maxShots = attachedGameMode.GetPlayerManager(playerNumber).GetMaxProjectiles(roundNumber);
        int shotsRemaining = attachedGameMode.GetPlayerManager(playerNumber).GetAvailableProjectiles(roundNumber);

        int projectedShotsRemaining;
        if(attachedGameMode.GameState.GetPlayerPositionsLocked(playerNumber, roundNumber))
            projectedShotsRemaining = attachedGameMode.GetPlayerManager(playerNumber).GetProjectedProjectilesRemaining(0);
        else
            projectedShotsRemaining = attachedGameMode.GetPlayerManager(playerNumber).GetProjectedProjectilesRemaining(step);

        float shotsScalar = shotsRemaining / (float)maxShots;
        float projectedScalar = projectedShotsRemaining / (float)maxShots;

        imageActualShots.transform.localScale = new Vector3(shotsScalar, 1, 1);
        imageProjectedShots.transform.localScale = new Vector3(projectedScalar, 1, 1);

        textAmmo.text = shotsRemaining + "/" + maxShots;
    }
}

