using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelConfig
{
    //The name of the scene so we know which one to load
    string GetSceneName();

    //Given a player and which round it is, what player model should be loaded?
    string GetPlayerModelName(int playerNumber, int playerRoundNumber);

    //Given which player and which round it is, where should the player spawn?
    Vector3 GetPlayerSpawnPosition(int playerNumber, int playerRoundNumber);

    int GetMaxRounds();
}
