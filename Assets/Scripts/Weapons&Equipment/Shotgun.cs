using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour, IWeapon
{
    [SerializeField] private Transform fireTransform;
    [SerializeField] private GameObject projectile;
    [SerializeField] private string projectileLayer = "Projectile";

    [SerializeField] private string firingSound = "WeaponLaserShot1";
    [SerializeField] private float fireRate = 0.7f;
    [SerializeField] private float projectileSpread = 10.0f;
    [SerializeField] private GameObject targetingCursor;

    public GameObject TargetingCursor => targetingCursor;
    public float FireRate => fireRate;
    public int CostToFire { get; } = 2;
    public bool LoadedCursor { get; set; } = false;
    public string WeaponName { get; } = "Shotgun";
    public int WeaponType { get; } = 1;
    private bool friendlyFire;

    void Awake()
    {
        fireTransform = transform.childCount > 0 ? transform.GetChild(0) : transform;
        friendlyFire = FindObjectOfType<AudioManager>().FriendlyFire;
    }

    public GameObject[] Fire(int playerNumber, Color playerColor)
    {
        if (fireTransform.parent != transform.root)
            fireTransform.parent = transform.root;

        GameObject projectileInstance1 = InstantiateProjectile(playerNumber, playerColor, -1 * projectileSpread);
        GameObject projectileInstance2 = InstantiateProjectile(playerNumber, playerColor, 0);
        GameObject projectileInstance3 = InstantiateProjectile(playerNumber, playerColor, projectileSpread);

        FindObjectOfType<AudioManager>().PlaySFX(firingSound);

        return new GameObject[] { projectileInstance1, projectileInstance2, projectileInstance3 };
    }

    private GameObject InstantiateProjectile(int playerNumber, Color playerColor, float bulletAngle)
    {
        Quaternion newRotation = fireTransform.rotation;
        newRotation *= Quaternion.AngleAxis(bulletAngle, Vector3.up);

        GameObject projectileInstance = Instantiate(
                projectile,
                fireTransform.position,
                newRotation
            ) as GameObject;

        projectileInstance.GetComponent<Bullet>().BulletColor = playerColor;
        projectileInstance.GetComponent<Bullet>().BouncesLeft = 0;

        projectileInstance.gameObject.layer =
            LayerMask.NameToLayer(
                friendlyFire ? projectileLayer : projectileLayer + playerNumber
            );

        projectileInstance.GetComponent<Rigidbody>().velocity = projectileInstance.transform.forward;

        return projectileInstance;
    }
}
