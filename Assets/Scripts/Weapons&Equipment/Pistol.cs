using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private string projectileLayer = "Projectile";

    [SerializeField] private string firingSound = "WeaponLaserShot1";
    [SerializeField] private float fireRate = 5f;
    [SerializeField] private GameObject targetingCursor;

    public GameObject TargetingCursor => targetingCursor;
    public float FireRate => fireRate;
    public int CostToFire { get; } = 1;
    public bool LoadedCursor { get; set; } = false;
    public string WeaponName { get; } = "Pistol";
    public int WeaponType { get; } = 0;
    public Transform FireTransform { get; private set; }
    private bool friendlyFire;

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
