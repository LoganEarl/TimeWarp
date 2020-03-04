﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private readonly AudioClip bounceSound;
    [SerializeField] private readonly float bulletSpeed = 25;
    [SerializeField] private readonly int bulletDmg = 1;
    [SerializeField] private int bouncesLeft = 4;

    public Color bulletColor { private get; set; }

    private Rigidbody bulletInstance;

    private void Start() {
        bulletInstance = GetComponent<Rigidbody>();

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

    private void OnDestroy() {
        GetComponent<AudioSource>().PlayOneShot(bounceSound);
    }
}