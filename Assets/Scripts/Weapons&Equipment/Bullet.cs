using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int playerNumber;
    public float bulletSpeed = 30;
    public int bulletDmg = 1;
    public float bouncesLeft = 4f;
    public float maxLifeTime = 4f;
    public Color bulletColor;

    private Rigidbody bulletInstance;
    private AudioSource bounceSound;

    void Start() {
        bulletInstance = GetComponent<Rigidbody>();
        bounceSound = GetComponent<AudioSource>();
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
            other.GetComponent<PlayerHealth>().DoDamage(bulletDmg);
        else if (other.tag == "ForceField")
            other.GetComponent<ForceField>().DoDamage();

        if ((other.tag == "Wall") && bouncesLeft != 0)
        {
            bounceSound.Play();
            bouncesLeft--;
        }
        else
            Destroy(gameObject);
    }
}
