using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : MonoBehaviour, IWeapon
{
    [SerializeField] private Transform fireTransform;
    [SerializeField] private GameObject laser;

    [SerializeField] private string firingSound = "WeaponLaserShot1";
    [SerializeField] private float fireRate = 0.85f;
    [SerializeField] private GameObject targetingCursor;

    public GameObject TargetingCursor => targetingCursor;
    public float FireRate => fireRate;
    public int CostToFire { get; } = 0;
    public bool LoadedCursor { get; set; } = false;
    public string WeaponName { get; } = "Sniper";
    public int WeaponType { get; } = 1;

    void Awake()
    {
        fireTransform = transform.childCount > 0 ? transform.GetChild(0) : transform;
    }

    public GameObject[] Fire(int playerNumber, Color playerColor)
    {
        if(fireTransform.parent != transform.root)
            fireTransform.parent = transform.root;

        GameObject laserBeam = Instantiate(laser, fireTransform.position, fireTransform.rotation);
        laserBeam.GetComponent<Laser>().Initialize(playerNumber, playerColor);


        return new GameObject[] { laserBeam };
    }
    //Maybe add particle effects to surfaces that it impacts to imply heating them up?
}
