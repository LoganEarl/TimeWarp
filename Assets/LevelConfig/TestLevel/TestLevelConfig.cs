﻿using System.Collections;
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
        return "Player/Player";
    }

    public int GetMaxRounds()
    {
        return 10;
    }

    public Vector3 GetPlayerSpawnPosition(int playerNum, int matchNum)
    {
        if(playerNum % 2 == 0)
            return new Vector3(-5f + matchNum * 0.5f,0f, matchNum * 0.5f);
        else
            return new Vector3(5f - matchNum * 0.5f, 0f, matchNum * -0.5f);
    }
}
