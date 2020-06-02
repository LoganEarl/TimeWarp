using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 25;
    [SerializeField] private int bulletDmg = 1;
    [SerializeField] public int BouncesLeft { private get; set; } = 4;

    public Color BulletColor { private get; set; }

    private Rigidbody bulletInstance;
    
    private void Start() {
        bulletInstance = GetComponent<Rigidbody>();


        GetComponent<MeshRenderer>().material.SetColor("_GlowColor", BulletColor);
        GetComponent<TrailRenderer>().material.SetColor("_GlowColor", BulletColor);
    }

    private void LateUpdate() {
        // Needed to maintain the correct speed even when bouncing off of surfaces that aren't
        // actually a clean vertical surface to rebound off of.
        bulletInstance.velocity = bulletInstance.velocity.normalized * bulletSpeed;
    }

    private void OnCollisionEnter(Collision collision) {
        Rigidbody bulletInstance = GetComponent<Rigidbody>();
        Collider other = collision.collider;

        if (other.tag != "Wall")
        {
            if (other.tag.StartsWith("Player"))
            {
                PlayerHealth pHealth = other.GetComponent<PlayerHealth>();

                if (pHealth == null)
                    pHealth = other.GetComponentInParent<PlayerHealth>();

                pHealth?.DoDamage(bulletDmg);
                Destroy(gameObject);
            }
            else if (other.tag == "ForceField")
                other.GetComponent<ForceField>()?.DoDamage();
        }

        FindObjectOfType<AudioManager>().PlaySFX("WeaponLaserRicochet");

        if ((other.tag == "Wall") && BouncesLeft != 0)
            BouncesLeft--;
        else if (!other.tag.StartsWith("Player"))
            Destroy(gameObject);
    }
}
