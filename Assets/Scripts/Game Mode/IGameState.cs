using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IGameState
{
    bool GetPlayerVisible(int playerNum, int roundNum);
    bool GetPlayerPositionsLocked(int playerNum, int roundNum);
    bool GetPlayerLookLocked(int playerNum, int roundNum);
    bool GetPlayerFireLocked(int playerNum, int roundNum);
    bool GetPlayerCanTakeDamage(int playerNum, int roundNum);
    bool TimeAdvancing { get; }
    int StepNumber { get; }
    int MaxSteps { get; }
    float SecondsRemaining { get; }
}
