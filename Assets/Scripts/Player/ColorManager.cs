using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance { get; private set; }

    [SerializeField]
    private Material[] player0Materials = { };

    [SerializeField]
    private Material[] player1Materials = { };

    private Material[][] playerMaterials;

    public enum PlayerColorVarient
    {
        MODEL_PRIMARY_INACTIVE = 0,
        MODEL_PRIMARY_ACTIVE = 1,
        MODEL_SECONDARY_INACTIVE = 2,
        MODEL_SECONDARY_ACTIVE = 3,
        UI_PRIMARY_INACTIVE = 4,
        UI_PRIMARY_ACTIVE = 5,
        SPAWN_PRIMARY = 6,
        LASER_PRIMARY = 7
    }

    public Material GetPlayerMaterial(int playerNum, PlayerColorVarient varient)
    {
        if (playerNum < 0 || playerNum > playerMaterials.Length) return null;
        if (varient < 0 || (int)varient >= playerMaterials[playerNum].Length) return null;
        return playerMaterials[playerNum][(int)varient];
    }

    void Awake()
    {
        CheckInstance();
        playerMaterials = new Material[][]{ player0Materials, player1Materials };
    }

    private void CheckInstance()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
