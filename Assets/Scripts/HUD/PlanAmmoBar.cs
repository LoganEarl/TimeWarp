using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlanAmmoBar : MonoBehaviour
{
    private PlanManager attachedGameMode;
    private int playerNumber;
    private bool setup = false;

    [SerializeField]
    private GameObject imageActualShots;
    [SerializeField]
    private GameObject imageProjectedShots;
    [SerializeField]
    private TextMeshProUGUI textAmmo;

    public void Setup(PlanManager attachedGameMode, int playerNumber)
    {
        this.playerNumber = playerNumber;
        this.attachedGameMode = attachedGameMode;
        setup = true;
    }

    public void Update()
    {
        int maxShots = attachedGameMode.MaxShots(playerNumber);
        int shotsRemaining = attachedGameMode.ShotsRemaining(playerNumber);
        int projectedShotsRemaining = attachedGameMode.ProjectedShotsRemaining(playerNumber);

        float shotsScalar = shotsRemaining / (float)maxShots;
        float projectedScalar = projectedShotsRemaining / (float)maxShots;

        imageActualShots.transform.localScale = new Vector3(shotsScalar, 1, 1);
        imageProjectedShots.transform.localScale = new Vector3(projectedScalar, 1, 1);

        textAmmo.text = shotsRemaining + "/" + maxShots;
    }
}

