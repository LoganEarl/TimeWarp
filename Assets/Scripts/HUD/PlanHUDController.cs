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
    private AmmoBar[] playerAmmoBars;
    [SerializeField]
    private Timer timer;
    [SerializeField]
    private RoundNumber roundNumber;
    [SerializeField]
    private EquipmentBar[] equipmentBars;

    private PlanManager planManager;

    private List<GameObject>[] loadedPlayerHealthBars = new List<GameObject>[0];

    public void Setup(PlanManager planManager)
    {
        this.planManager = planManager;
        timer.Setup(planManager);
        roundNumber.Setup(planManager);

        for(int i = 0; i < playerAmmoBars.Length; i++)
            playerAmmoBars[i].Setup(planManager, i);
        for (int i = 0; i < equipmentBars.Length; i++)
            equipmentBars[i].Setup(planManager, i);

        Canvas drawCanvas = gameObject.GetComponent<Canvas>();
        drawCanvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        drawCanvas.planeDistance = 1;

        ReloadAll();
    }

    public void ReloadAll()
    {
        for (int playerNum = 0; playerNum < loadedPlayerHealthBars.Length; playerNum++)
            foreach (GameObject obj in loadedPlayerHealthBars[playerNum])
                Destroy(obj);

        LoadPlayerWeapons();
        LoadPlayerHealthBars();
    }

    private void LoadPlayerWeapons()
    {
        GameObject p0Weapon = GameObject.Find("P0Weapon");
        GameObject p1Weapon = GameObject.Find("P1Weapon");
        p0Weapon.transform.GetChild(2).gameObject.GetComponent<WeaponModelExchange>().ResetWeapons();
        p1Weapon.transform.GetChild(2).gameObject.GetComponent<WeaponModelExchange>().ResetWeapons();
    }

    private void LoadPlayerHealthBars()
    {
        int numPlayers = planManager.NumPlayers;
        int numRounds = planManager.RoundNumber;
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
                GameObject currentPlayer = planManager.GetPlayerManager(playerNum).GetPlayerObject(roundNum);
                HealthBar toSetup = loadedPlayerHealthBars[playerNum][roundNum].GetComponent<HealthBar>();
                toSetup.Setup(planManager, currentPlayer, playerNum, roundNum, HealthBar.DisplayMode.SCALE_WITH_TIME);
            }
        }
    }

    private void OnDestroy()
    {
        for(int i = 0; i < loadedPlayerHealthBars.Length; i++)
            foreach (GameObject bar in loadedPlayerHealthBars[i])
                Destroy(bar);
        Destroy(timer.gameObject);
        Destroy(roundNumber.gameObject);
        foreach (EquipmentBar bar in equipmentBars)
            Destroy(bar.gameObject);
    }
}
