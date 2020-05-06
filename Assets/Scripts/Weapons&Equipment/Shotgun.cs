using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private string projectileLayer = "Projectile";

    [SerializeField] private string firingSound = "WeaponLaserShot1";
    [SerializeField] private float fireRate = 0.7f;
    [SerializeField] private GameObject targetingCursor;

    public GameObject TargetingCursor => targetingCursor;
    public float FireRate => fireRate;
    public bool LoadedCursor { get; set; } = false;
    public string WeaponName { get; } = "Shotgun";
    public int WeaponType { get; } = 1;
    public Transform FireTransform { get; private set; }
    private bool friendlyFire;

    void Awake()
    {
        FireTransform = transform.childCount > 0 ? transform.GetChild(0) : transform;
        friendlyFire = FindObjectOfType<AudioManager>().FriendlyFire;
    }

    public GameObject[] Fire(int playerNumber, Color playerColor)
    {
        return new GameObject[0];
    }
}
