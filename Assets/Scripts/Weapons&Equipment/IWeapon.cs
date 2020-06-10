﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    string WeaponName { get; }
    bool LoadedCursor { get; set; }
    int WeaponType { get; }
    float FireRate { get; }
    int CostToFire { get; }
    int LookMagnitude { get; }
    GameObject TargetingCursor { get; }
    GameObject ProjectileIconPrefab { get; }
    
    GameObject[] Fire(int playerNumber, Color playerColor);
}