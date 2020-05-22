using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceLevelConfig: ILevelConfig
{
    private static readonly Vector3[][] playerSpawns = new Vector3[][] {
        new Vector3[]
        {
            new Vector3(-17.25f,.2f,9.25f),
            new Vector3(-17.25f,.2f,-9.25f),
            new Vector3(-15f,.2f,0),
            new Vector3(-7f,.2f,8),
            new Vector3(2,.2f,5)
        },
        new Vector3[]
        {
            new Vector3(17.25f,.2f,-9.25f),
            new Vector3(17.25f,.2f,9.25f),
            new Vector3(15f,.2f,0),
            new Vector3(7f,.2f,-8),
            new Vector3(-2,.2f,-5)
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
        return "Prefabs/Player/Player";
    }

    public Vector3 GetPlayerSpawnPosition(int playerNum, int matchNum)
    {
        return playerSpawns[playerNum % 2][matchNum % 5];
    }

    public Vector3[][] GetAllSpawnPositions()
    {
        return playerSpawns;
    }
}
