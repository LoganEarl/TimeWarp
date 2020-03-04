using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGameMode
{
    //this will be added to later
    void Begin();
    void Setup(int numPlayers, ILevelConfig levelConfig);
    void Reset();
    int NumPlayers { get; }
    int RoundNumber { get; }
    int MaxRounds { get; }
    IGameState GameState { get; }
    GameObject GameObject { get; }

    int EquipmentRemaining(int playerNumber);
}
