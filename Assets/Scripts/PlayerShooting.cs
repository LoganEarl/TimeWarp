using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public int playerNumber = 1;
    public Rigidbody bullet;
    public Transform fireTransform;
    public float bulletSpeed = 40f;

    private string fireButton;

    void Start() {
        fireButton = "Fire" + playerNumber;
    }
    
    void FixedUpdate() {

        if (Input.GetButtonDown(fireButton)) Shoot();

    }

    private void Shoot() {
        Rigidbody bulletInstance = Instantiate(bullet, fireTransform.position, fireTransform.rotation) as Rigidbody;

        bulletInstance.velocity = bulletSpeed * fireTransform.forward;
    }
}
