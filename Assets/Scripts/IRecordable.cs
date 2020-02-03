using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IRecordable
{
    PlayerSnapshot GetSnapshot();
    void SetSnapshot(PlayerSnapshot playerSnapshot);
    void SetUseSnapshots(bool useSnapshots);
}

public class PlayerSnapshot
{
    public Vector3 Transformation { get; }
    public Vector3 Force { get; }
    //eventually have endpoints for actions taken.
    public PlayerSnapshot(Vector3 transformation, Vector3 force) =>
        (Transformation, Force) = (transformation, force);
}
