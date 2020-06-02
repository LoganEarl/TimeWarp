using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    public PlayerController Player { private get; set; }

    private static readonly float WEAPON_HEIGHT = 0.75f;

    void FixedUpdate()
    {
        transform.position =
            Player.GetComponent<Rigidbody>().position +
            Player.LookDirection +
            new Vector3(0, WEAPON_HEIGHT, 0);
    }
}
