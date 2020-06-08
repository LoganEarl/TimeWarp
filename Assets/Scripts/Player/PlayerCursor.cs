using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    public PlayerController Player { private get; set; }

    private static readonly float WEAPON_HEIGHT = 0.75f;
    private bool isInitialized = false;

    void FixedUpdate()
    {
        if (Player == null && isInitialized)
            Destroy(this);
        else if (Player != null && !isInitialized)
            isInitialized = true;

        if (Player != null) { 
            transform.position =
                Player.GetComponent<Rigidbody>().position +
                Player.LookDirection +
                new Vector3(0, WEAPON_HEIGHT, 0);
        }
    }
}
