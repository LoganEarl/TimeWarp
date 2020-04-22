using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 25;
    [SerializeField] private int bulletDmg = 1;
    [SerializeField] private int bouncesLeft = 4;

    public Color bulletColor { private get; set; }
    public int playerNumber { private get; set; }

    private Rigidbody bulletInstance;
    
    private void Start() {
        bulletInstance = GetComponent<Rigidbody>();

        FindObjectOfType<AudioManager>().PlaySFX("WeaponLaserShot1");

        GetComponent<MeshRenderer>().material.SetColor("_GlowColor", bulletColor);
        GetComponent<TrailRenderer>().material.SetColor("_GlowColor", bulletColor);
    }

    private void FixedUpdate() {
        // Needed to maintain the correct speed even when bouncing off of surfaces that aren't
        // actually a clean vertical surface to rebound off of.
        bulletInstance.velocity = bulletInstance.velocity.normalized * bulletSpeed;
    }

    private void OnCollisionEnter(Collision collision) {
        Rigidbody bulletInstance = GetComponent<Rigidbody>();
        Collider other = collision.collider;

        if (other.tag != "Wall")
        {
            if (other.tag == "Player" + playerNumber || other.tag == "Ghost")
                other.GetComponent<PlayerHealth>()?.DoDamage(bulletDmg);
            else if (other.tag == "ForceField")
                other.GetComponent<ForceField>()?.DoDamage();
        }

        FindObjectOfType<AudioManager>().PlaySFX("WeaponLaserRicochet");

        if ((other.tag == "Wall") && bouncesLeft != 0)
            bouncesLeft--;
        else if (other.tag != "Player" + playerNumber)
            Destroy(gameObject);
    }
}
