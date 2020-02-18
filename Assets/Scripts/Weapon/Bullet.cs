using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int playerNumber;
    public AudioSource bounceSound;
    public float bulletSpeed = 30;
    public int bulletDmg = 1;
    public float bouncesLeft = 4f;
    public float maxLifeTime = 4f;

    private Rigidbody bulletInstance;

    void Start() {
        bulletInstance = GetComponent<Rigidbody>();
        Destroy(gameObject, maxLifeTime);
    }

    private void FixedUpdate()
    {
        bulletInstance.velocity = bulletInstance.velocity.normalized * bulletSpeed;
    }

    private void OnCollisionEnter(Collision collision) {
        Rigidbody bulletInstance = GetComponent<Rigidbody>();
        Collider other = collision.collider;

        if (other.tag.StartsWith("Player"))
        {
            other.GetComponent<PlayerHealth>().DoDamage(bulletDmg);
            Destroy(gameObject);
        }

        if ((other.tag == "Wall" || other.tag == "ForceField") && bouncesLeft != 0)
            bouncesLeft--;
        else if (bouncesLeft == 0)
            Destroy(gameObject);
    }
}
