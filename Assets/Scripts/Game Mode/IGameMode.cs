﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGameMode
{
    void Begin();
    void Setup(int numPlayers, ILevelConfig levelConfig);
    void Reset();
    int NumPlayers { get; }
    int RoundNumber { get; }
    int MaxRounds { get; }
    IGameState GameState { get; }
    GameObject GameObject { get; }
    PlayerManager GetPlayerManager(int playerNum);
    void ClearOnRoundChange(params GameObject[] toClear);
    void ClearOnMatchChange(params GameObject[] toClear);
}
