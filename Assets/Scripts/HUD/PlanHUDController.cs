using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanHUDController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] playerHealthAreas;
    [SerializeField]
    private GameObject[] playerHealthBarPrefabs;
    [SerializeField]
    private Timer timer;
    [SerializeField]
    private RoundNumber roundNumber;

    private PlanManager planManager;

    private List<GameObject>[] loadedPlayerHealthBars = new List<GameObject>[0];

    public void Setup(PlanManager planManager)
    {
        this.planManager = planManager;
        timer.Setup(planManager);
        roundNumber.Setup(planManager);
    }

    public void ReloadAll()
    {
        for (int playerNum = 0; playerNum < loadedPlayerHealthBars.Length; playerNum++)
            foreach (GameObject obj in loadedPlayerHealthBars[playerNum])
                Destroy(obj);

        LoadPlayerHealthBars();
    }

    private void LoadPlayerHealthBars()
    {
        int numPlayers = planManager.NumPlayers;
        int numRounds = planManager.RoundNumber + 1; //round number is 0 indexed
        loadedPlayerHealthBars = new List<GameObject>[numPlayers];

        for(int playerNum = 0; playerNum < numPlayers; playerNum++)
        {
            loadedPlayerHealthBars[playerNum] = new List<GameObject>();
            for(int roundNum = 0; roundNum < numRounds; roundNum++)
            {
                Vector3 relativePosition = new Vector3(0, -1 * 15 * roundNum - 5,0);
                loadedPlayerHealthBars[playerNum].Add(Instantiate(
                    playerHealthBarPrefabs[playerNum],
                    playerHealthAreas[playerNum].transform,
                    false));
                loadedPlayerHealthBars[playerNum][roundNum].transform.localPosition = relativePosition;
                GameObject currentPlayer = planManager.GetPlayerObject(playerNum, roundNum);
                HealthBar toSetup = loadedPlayerHealthBars[playerNum][roundNum].GetComponent<HealthBar>();
                toSetup.Setup(planManager, currentPlayer, playerNum, roundNum);
            }
        }
    }
}
