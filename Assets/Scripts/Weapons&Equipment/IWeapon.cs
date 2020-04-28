using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    bool HasProjectile { get; }
    float FireRate { get; }
    Transform FireTransform { get; }
    
    GameObject Fire(int playerNumber, Color playerColor);
}
