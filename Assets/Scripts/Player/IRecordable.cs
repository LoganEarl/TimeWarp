using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IRecordable
{
    PlayerSnapshot GetSnapshot();                       //cosntruct a snapshot of current state and return it
    void SetSnapshot(PlayerSnapshot playerSnapshot);    //load in a snapshot to be reproduced if useSanpshots was last set to true
    void SetUseSnapshots(bool useSnapshots);            //sets if controller should be relying on SetSnapshot() calls
    void OnReset();                                     //resets the player back to starting conditions. Called at end of round
}