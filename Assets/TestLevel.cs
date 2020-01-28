using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevel : ILevel
{
   public string GetSceneName()
    {
        return "testLevel";
    }

    public Vector3 GetPlayerSpawnPosition(int playerNum, int matchNum)
    {
        //all spawn at the origin
        return new Vector3();
    }
}
