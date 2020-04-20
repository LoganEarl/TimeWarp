using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanEquipmentBar : MonoBehaviour
{
    [SerializeField]
    private GameObject equipmentPrefab;

    private GameObject[] instantiatedPrefabs = new GameObject[0];
    private ColorSelection[] colorSelections = new ColorSelection[0];
    private SpriteRenderer[] spriteRenderers = new SpriteRenderer[0];

    private int playerNumber;
    private PlanManager gameMode;
    private bool setup = false;

    public void Setup(PlanManager gameMode, int playerNumber)
    {
        this.playerNumber = playerNumber;
        this.gameMode = gameMode;
        setup = true;
    }

    private void Update()
    {
        if (setup)
        {
            PlayerManager manager = gameMode.GetPlayerManager(playerNumber);
            if (instantiatedPrefabs.Length != gameMode.GetPlayerManager(playerNumber).GetMaxEquipment())
                ReloadDisplay();
            for(int i = 0; i < instantiatedPrefabs.Length; i++)
            {
                int colorIndex = 0;
                if (i >= manager.GetProjectedEquipmentRemaining(0,gameMode.GameState.StepNumber)) colorIndex++;
                if (i >= manager.GetAvailableEquipment()) colorIndex++;
                spriteRenderers[i].color = colorSelections[i].colors[colorIndex];
            }
        }
    }

    private void ReloadDisplay()
    {
        if (setup)
        {
            foreach (GameObject obj in instantiatedPrefabs)
                Destroy(obj);
            int numSprites = gameMode.GetPlayerManager(playerNumber).GetMaxEquipment();
            instantiatedPrefabs = new GameObject[numSprites];
            colorSelections = new ColorSelection[numSprites];
            spriteRenderers = new SpriteRenderer[numSprites];

            for (int i = 0; i < instantiatedPrefabs.Length; i++)
            {
                instantiatedPrefabs[i] = Instantiate(equipmentPrefab, gameObject.transform, false);
                instantiatedPrefabs[i].transform.localPosition = new Vector3(i * 20, 0, 0);
                colorSelections[i] = instantiatedPrefabs[i].GetComponent<ColorSelection>();
                spriteRenderers[i] = instantiatedPrefabs[i].GetComponent<SpriteRenderer>();
            }
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject dot in instantiatedPrefabs)
            Destroy(dot);
    }
}
