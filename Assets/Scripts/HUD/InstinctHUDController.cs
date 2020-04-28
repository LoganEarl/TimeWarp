using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstinctHUDController : MonoBehaviour
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

    private InstinctManager instinctManager;

    private GameObject[] loadedPlayerHealthBars;

    public void Setup(InstinctManager instinctManager)
    {
        this.instinctManager = instinctManager;
        timer.Setup(instinctManager);
        roundNumber.Setup(instinctManager);

        for(int i = 0; i < playerAmmoBars.Length; i++)
            playerAmmoBars[i].Setup(instinctManager, i);
        for (int i = 0; i < equipmentBars.Length; i++)
            equipmentBars[i].Setup(instinctManager, i);

        Canvas drawCanvas = gameObject.GetComponent<Canvas>();
        drawCanvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        drawCanvas.planeDistance = 1;

        ReloadAll();
    }

    public void ReloadAll()
    {
        foreach (GameObject obj in loadedPlayerHealthBars)
            Destroy(obj);

        LoadPlayerHealthBars();
    }

    private void LoadPlayerHealthBars()
    {
        int numPlayers = instinctManager.NumPlayers;
        int roundNumber = instinctManager.RoundNumber;
        loadedPlayerHealthBars = new GameObject[numPlayers];

        for(int playerNum = 0; playerNum < numPlayers; playerNum++)
        {
            loadedPlayerHealthBars[playerNum] = Instantiate(
                playerHealthBarPrefabs[playerNum],
                playerHealthAreas[playerNum].transform,
                false);

            Vector3 relativePosition = new Vector3(0, -20, 0);
            loadedPlayerHealthBars[playerNum].transform.localPosition = relativePosition;
            GameObject currentPlayer = instinctManager.GetPlayerManager(playerNum).GetPlayerObject(roundNumber);
            HealthBar toSetup = loadedPlayerHealthBars[playerNum].GetComponent<HealthBar>();
            toSetup.Setup(instinctManager, currentPlayer, playerNum, roundNumber, HealthBar.DisplayMode.SCALE_WITH_DAMAGE);
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject bar in loadedPlayerHealthBars)
            Destroy(bar);
        Destroy(timer.gameObject);
        Destroy(roundNumber.gameObject);
        foreach (EquipmentBar bar in equipmentBars)
            Destroy(bar);
    }
}
