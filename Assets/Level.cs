using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevel
{
    //The name of the scene so we know which one to load
    string GetSceneName();

    //Given which player and which round it is, where should the player spawn 
    Vector3 GetPlayerSpawnPosition(int playerNumber, int playerRoundNumber);
}
