using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundNumber : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI roundNumberText;
    [SerializeField]
    private int roundNumber;

    private int maxRounds = 6;

    //public PlanManager planManager;

    public void Update()
    {
        if (roundNumber <= maxRounds)
            roundNumberText.text = "Round " + roundNumber; // planManager.GetComponent<PlanManager>().RoundNumber;
    }

    public void incrementRoundNumber()
    {
        roundNumber++;
    }
}
