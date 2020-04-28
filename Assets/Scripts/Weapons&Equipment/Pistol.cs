using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private string projectileLayer = "Projectile";

    [SerializeField] private string firingSound = "WeaponLaserShot1";
    [SerializeField] public float FireRate { get; } = 0.25f;

    public Transform FireTransform { get; private set; }
    private AudioManager audioManager;

    public bool HasProjectile { get; } = true;

    void Awake()
    {
        FireTransform = transform.GetChild(0);
        audioManager = FindObjectOfType<AudioManager>();
    }

    public GameObject Fire(int playerNumber, Color playerColor)
    {
        GameObject projectileInstance = Instantiate(projectile, FireTransform.position, FireTransform.rotation) as GameObject;
        projectileInstance.GetComponent<Bullet>().BulletColor = playerColor;

        string layer = projectileLayer;
        if (!audioManager.GetFriendlyFire()) layer += playerNumber;

        projectileInstance.gameObject.layer = LayerMask.NameToLayer(layer);
        projectileInstance.GetComponent<Rigidbody>().velocity = FireTransform.forward;

        audioManager.PlaySFX(firingSound);

        return projectileInstance;
    }
}
