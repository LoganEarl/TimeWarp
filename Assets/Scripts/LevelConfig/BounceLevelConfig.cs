using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceLevelConfig: ILevelConfig
{
    private static readonly Vector3[][] playerSpawns = new Vector3[][] {
        new Vector3[]
        {
            new Vector3(-17.25f,.1f,9.25f),
            new Vector3(-17.25f,.1f,-9.25f),
            new Vector3(-15f,.1f,0),
            new Vector3(-7f,.1f,8),
            new Vector3(2,.1f,6)
        },
        new Vector3[]
        {
            new Vector3(17.25f,.1f,-9.25f),
            new Vector3(17.25f,.1f,9.25f),
            new Vector3(15f,.1f,0),
            new Vector3(7f,.1f,-8),
            new Vector3(-2,.1f,-6)
        }
    };

    public string GetSceneName()
    {
        return "BounceMap";
    }

    public int GetMaxRounds()
    {
        return 5;
    }

    public string GetPlayerModelName(int playerNum, int matchNum)
    {
        return "Prefabs/Player";
    }

    public Vector3 GetPlayerSpawnPosition(int playerNum, int matchNum)
    {
        return playerSpawns[playerNum % 2][matchNum % 5];
    }
}
