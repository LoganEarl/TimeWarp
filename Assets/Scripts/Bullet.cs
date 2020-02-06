using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int playerNumber;
    public AudioSource bounceSound;
    public int bulletDmg = 1;
    public float bouncesLeft = 400;
    public float maxLifeTime = 10f;

    void Start() {
        Destroy(gameObject, maxLifeTime);
    }

    private void OnCollisionEnter(Collision collision) {
        Rigidbody bulletInstance = GetComponent<Rigidbody>();
        Collider other = collision.collider;

        if (other.tag == "Wall" && bouncesLeft != 0) bouncesLeft--;
        else if (other.tag == "Wall") Destroy(gameObject, 0f);
        else if (other.tag != "Player") {
            other.GetComponent<PlayerHealth>().TakeDamage(bulletDmg);
            Destroy(gameObject, 0f);
        }
    }
}
