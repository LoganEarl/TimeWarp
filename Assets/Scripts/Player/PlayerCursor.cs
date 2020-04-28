using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    public PlayerController Player { private get; set; }

    void FixedUpdate()
    {
        transform.position =
            Player.GetComponent<Rigidbody>().position +
            Player.LookDirection +
            new Vector3(0, Player.Weapon.FireTransform.position.y, 0);
    }
}
