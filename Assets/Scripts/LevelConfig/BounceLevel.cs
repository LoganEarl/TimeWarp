using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceLevel: ILevelConfig
{
    private static readonly Vector3[][] playerSpawns = new Vector3[][] {
        new Vector3[]
        {
            new Vector3(9.25f,.1f,-17.25f),
            new Vector3(-9.25f,.1f,-17.25f),
            new Vector3(0,.1f,-15f),
            new Vector3(8,.1f,-7),
            new Vector3(6,.1f,2)
        },
        new Vector3[]
        {
            new Vector3(-9.25f,.1f,17.25f),
            new Vector3(9.25f,.1f,17.25f),
            new Vector3(0,.1f,15f),
            new Vector3(-8,.1f,7),
            new Vector3(-6,.1f,-2)
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
