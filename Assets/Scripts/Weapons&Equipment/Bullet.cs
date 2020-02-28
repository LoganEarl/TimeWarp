using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private float bulletSpeed = 25;
    [SerializeField] private int bulletDmg = 1;
    [SerializeField] private int bouncesLeft = 4;
    [SerializeField] private Color bulletColor;

    private Rigidbody bulletInstance;

    void Start() {
        bulletInstance = GetComponent<Rigidbody>();

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

        GetComponent<AudioSource>().PlayOneShot(bounceSound);

        if ((other.tag == "Wall") && bouncesLeft != 0)
            bouncesLeft--;
        else
            Destroy(gameObject);
    }
}
