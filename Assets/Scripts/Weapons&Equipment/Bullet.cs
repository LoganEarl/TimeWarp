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
    public Color bulletColor;
    public float inversePathDensity = 0.01f;
    public float inversePathLength = 1f;

    private Rigidbody bulletInstance;
    private float timeSinceLastPath = 0f;

    void Start() {
        bulletInstance = GetComponent<Rigidbody>();
        Destroy(gameObject, maxLifeTime);

        GetComponent<MeshRenderer>().material.SetColor("_GlowColor", bulletColor);
        GetComponent<TrailRenderer>().material.SetColor("_GlowColor", bulletColor);
    }

    private void FixedUpdate() {
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
        else if (other.tag == "ForceField")
        {
            other.GetComponent<ForceField>().DoDamage();
            Destroy(gameObject);
        }

        if ((other.tag == "Wall") && bouncesLeft != 0)
            bouncesLeft--;
        else if (bouncesLeft == 0)
            Destroy(gameObject);
    }
}
