using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaLevelConfig : ILevelConfig
{
    private static readonly Vector3[][] playerSpawns = new Vector3[][] {
        new Vector3[]
        {
            new Vector3(-17,0.2f,0),
            new Vector3(-17,0.2f,-7),
            new Vector3(-17,0.2f,7),
            new Vector3(-7,0.2f,8),
            new Vector3(-7,0.2f,-8)
        },
        new Vector3[]
        {
            new Vector3(17,0.2f,0),
            new Vector3(17,0.2f,7),
            new Vector3(17,0.2f,-7),
            new Vector3(7,0.2f,-8),
            new Vector3(7,0.2f,8)
        }
    };

    public string GetSceneName()
    {
        return "LavaMap";
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
