using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IRecordable
{
    PlayerSnapshot GetSnapshot();
    void SetSnapshot(PlayerSnapshot playerSnapshot);
    void SetUseSnapshots(bool useSnapshots);
    void OnReset();
}

public class PlayerSnapshot
{
    public Vector3 Translation { get; }
    public Vector3 Velocity { get; }
    public Vector3 LookDirection { get; }
    public bool IsIdle { get; }
    public bool Firing { get; }
    //eventually have endpoints for actions taken.
    public PlayerSnapshot(Vector3 translation, Vector3 velocity, Vector3 lookDirection, bool firing, bool isIdle) =>
        (Translation, Velocity, LookDirection, Firing, IsIdle) = (translation, velocity, lookDirection, firing, isIdle);
}
