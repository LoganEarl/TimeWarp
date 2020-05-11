using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : MonoBehaviour, IWeapon
{
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
    public Transform FireTransform { get; private set; }

    void Awake()
    {
        FireTransform = transform.childCount > 0 ? transform.GetChild(0) : transform;
    }

    public GameObject[] Fire(int playerNumber, Color playerColor)
    {
        GameObject laserBeam = Instantiate(laser, FireTransform.position, FireTransform.rotation);
        laserBeam.GetComponent<Laser>().PlayerNumber = playerNumber;


        return new GameObject[] { laserBeam };
    }

    //See about making the laser check if it's gonna kill the person and then make it go
    //Through the body so that it doesn't just stop on empty air
    //Maybe add particle effects to surfaces that it impacts to imply heating them up?

    
}
