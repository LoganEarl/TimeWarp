using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelConfig : ILevelConfig
{
   public string GetSceneName()
    {
        return "TestLevel";
    }

    public string GetPlayerModelName(int playerNum, int matchNum)
    {
        return "Player";
    }

    public Vector3 GetPlayerSpawnPosition(int playerNum, int matchNum)
    {
        return new Vector3(10 * playerNum,3f,3 * matchNum);
    }
}
