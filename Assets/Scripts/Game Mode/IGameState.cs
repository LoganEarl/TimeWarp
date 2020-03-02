using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IGameState
{
    bool PlayersVisible { get; }
    bool PlayersPositionsLocked { get; }
    bool PlayersLookLocked { get; }
    bool PlayersFireLocked { get; }
    bool TimeAdvancing { get; }
    int StepNumber { get; }
    int MaxSteps { get; }
    float SecondsRemaining { get; }
}
