using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSnapshot
{
    public Vector3 Translation { get; }     //xyz coords
    public Vector3 Velocity { get; }        //xyz velocity
    public Vector3 LookDirection { get; }   //look vector of the player
    public bool IsIdle { get; }             //if they are not doing anything
    public bool Firing { get; }             //if triggered the fire button this frame
    public bool UsingEquipment { get; }     //if triggered the equipment button this frame
    public bool UsedEquipment { get; }      //if released the equipment button this frame

    //eventually have endpoints for actions taken.
    public PlayerSnapshot(
        Vector3 translation, 
        Vector3 velocity, 
        Vector3 lookDirection, 
        bool firing, 
        bool usingEquipment, 
        bool usedEquipment,
        bool isIdle
        ) =>
        (Translation, Velocity, LookDirection, Firing, UsingEquipment, UsedEquipment, IsIdle) = 
        (translation, velocity, lookDirection, firing, usingEquipment, usedEquipment, isIdle);
}
