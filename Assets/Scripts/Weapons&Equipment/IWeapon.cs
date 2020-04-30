using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    string WeaponName { get; }
    int WeaponType { get; }
    float FireRate { get; }
    Transform FireTransform { get; }
    
    GameObject[] Fire(int playerNumber, Color playerColor);
}
