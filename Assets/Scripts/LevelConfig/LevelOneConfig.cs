using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOneConfig : ILevelConfig
{
    private static readonly Vector3[][] playerSpawns = new Vector3[][] {
        new Vector3[]
        {
            new Vector3(-16,.1f,0),
            new Vector3(-16,.1f,-7),
            new Vector3(-16,.1f,7),
            new Vector3(-6,.1f,9),
            new Vector3(-6,.1f,-9)
        },
        new Vector3[]
        {
            new Vector3(16,1,0),
            new Vector3(16,1,7),
            new Vector3(16,1,-7),
            new Vector3(6,1,-9),
            new Vector3(6,1,9)
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
        return "Prefabs/Player";
    }

    public Vector3 GetPlayerSpawnPosition(int playerNum, int matchNum)
    {
        return playerSpawns[playerNum % 2][matchNum % 5];
    }
}
