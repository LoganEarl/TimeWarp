using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGameMode
{
    //this will be added to later
    void Begin();
    void Setup(int numPlayers, ILevelConfig levelConfig);
    int StepNumber { get; }
    int MaxSteps { get; }
    int NumPlayers { get; }
}
