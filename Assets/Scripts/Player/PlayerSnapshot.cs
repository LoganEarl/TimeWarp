using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSnapshot
{
    public Vector3 Translation { get; }     //xyz coords
    public Vector3 Velocity { get; }        //xyz velocity
    public Vector3 MoveDirection { get; }   //xyz desired direction to move
    public Vector3 LookDirection { get; }   //look vector of the player
    public bool IsIdle { get; }             //if they are not doing anything
    public bool Changing { get; }           //if triggered the change weapon button this frame
    public string WeaponName { get; }       //string of the weapon currently held
    public bool Firing { get; }             //if triggered the fire button this frame
    public bool UsingEquipment { get; }     //if triggered the equipment button this frame
    public bool UsedEquipment { get; }      //if released the equipment button this frame

    //eventually have endpoints for actions taken.
    public PlayerSnapshot(
        Vector3 translation,
        Vector3 velocity,
        Vector3 lookDirection,
        Vector3 moveDirection,
        bool changing,
        string weaponName,
        bool firing, 
        bool usingEquipment, 
        bool usedEquipment,
        bool isIdle
        ) =>
        (Translation, Velocity, LookDirection, MoveDirection, Changing, WeaponName, Firing, UsingEquipment, UsedEquipment, IsIdle) = 
        (translation, velocity, lookDirection, moveDirection, changing, weaponName, firing, usingEquipment, usedEquipment, isIdle);
}
