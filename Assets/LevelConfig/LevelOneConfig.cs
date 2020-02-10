using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOneConfig : ILevelConfig
{
    private static readonly Vector3[][] playerSpawns = new Vector3[][] {
        new Vector3[]
        {
            new Vector3(16,0,0),
            new Vector3(16,0,-7),
            new Vector3(16,0,7),
            new Vector3(6,0,9),
            new Vector3(6,0,-9)
        },
        new Vector3[]
        {
            new Vector3(-16,0,0),
            new Vector3(-16,0,7),
            new Vector3(-16,0,-7),
            new Vector3(-6,0,-9),
            new Vector3(-6,0,9)
        }
    };

    public string GetSceneName()
    {
        return "FirstMap";
    }

    public int GetMaxRounds()
    {
        return 5;
    }

    public string GetPlayerModelName(int playerNum, int matchNum)
    {
        return "Player/Player";
    }

    public Vector3 GetPlayerSpawnPosition(int playerNum, int matchNum)
    {
        return playerSpawns[playerNum % 2][matchNum % 5];
    }
}
