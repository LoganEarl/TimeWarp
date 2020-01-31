using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public AudioSource bounceSound;
    public GameObject bullet;
    public float bulletDmg = 1;
    public float maxBounces = 2;

    void Start() {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
            Debug.Log("Player has been hit.");
        else if (other.tag == "Wall")
            Debug.Log("Wall has been hit.");
    }
}
