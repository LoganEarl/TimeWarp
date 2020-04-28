using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private string projectileLayer = "Projectile";

    [SerializeField] private string firingSound = "WeaponLaserShot1";
    [SerializeField] public float FireRate { get; } = 0.4f;

    public Transform FireTransform { get; private set; }
    private bool friendlyFire;

    public bool HasProjectile { get; } = true;

    void Awake()
    {
        FireTransform = transform.childCount > 0 ? transform.GetChild(0) : transform;
        friendlyFire = FindObjectOfType<AudioManager>().FriendlyFire;
    }

    public GameObject[] Fire(int playerNumber, Color playerColor)
    {
        GameObject projectileInstance = Instantiate(projectile, FireTransform.position, FireTransform.rotation) as GameObject;
        projectileInstance.GetComponent<Bullet>().BulletColor = playerColor;
        
        projectileInstance.gameObject.layer = 
            LayerMask.NameToLayer(
                friendlyFire ? projectileLayer : projectileLayer + playerNumber
            );

        projectileInstance.GetComponent<Rigidbody>().velocity = FireTransform.forward;

        FindObjectOfType<AudioManager>().PlaySFX(firingSound);

        return new GameObject[] { projectileInstance };
    }
}
