using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : MonoBehaviour, IWeapon
{
    [SerializeField] private string projectileLayer = "Projectile";
    [SerializeField] private int maxBounces;
    [SerializeField] private GameObject laser;
    [SerializeField] private GameObject laserRings;
    [SerializeField] private GameObject laserAfterEffects;

    [SerializeField] private float laserDensity = 0.1f;
    [SerializeField] private float ringDensity = 0.05f;
    [SerializeField] private float afterEffectDensity = 1.0f;
    [SerializeField] private float laserDuration = 0.75f;
    [SerializeField] private float afterEffectsDuration = 0.75f;

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
    private bool friendlyFire;
    private List<GameObject> toDelete = new List<GameObject>();
    private List<GameObject> toDeleteLater = new List<GameObject>();

    void Awake()
    {
        FireTransform = transform.childCount > 0 ? transform.GetChild(0) : transform;
        friendlyFire = FindObjectOfType<AudioManager>().FriendlyFire;

    }

    public GameObject[] Fire(int playerNumber, Color playerColor)
    {
        Material playerMaterial = ColorManager.Instance.GetPlayerMaterial(playerNumber, ColorManager.PlayerColorVarient.SPAWN_PRIMARY);
        laserRings.GetComponent<ParticleSystemRenderer>().material = playerMaterial;
        laserAfterEffects.GetComponent<ParticleSystemRenderer>().material = playerMaterial;

        FireRay(FireTransform.position, FireTransform.forward, maxBounces);

        Invoke("ClearEffects", laserDuration);
        Invoke("ClearAfterEffects", afterEffectsDuration);

        return new GameObject[0];
    }

    //See about making the laser check if it's gonna kill the person and then make it go
    //Through the body so that it doesn't just stop on empty air
    //Maybe add particle effects to surfaces that it impacts to imply heating them up?
    //more tweaking is needed for the aftereffects, also the clear methods might be
    //deleting the next shot's aftereffect particles.
    //

    private bool FireRay(Vector3 position, Vector3 direction, int bouncesLeft)
    {
        bool result = false;

        if (Physics.Raycast(position, direction, out RaycastHit hit, 1000))
        {
            Vector3 ringPosition = position;
            for (int i = 0; i < hit.distance / laserDensity; i++)
            {
                ringPosition = Vector3.MoveTowards(ringPosition, hit.point, laserDensity);
                toDelete.Add(Instantiate(laser, ringPosition, Quaternion.LookRotation(direction, Vector3.up)));
            }

            ringPosition = position;
            for (int i = 0; i < hit.distance / ringDensity; i++)
            {
                ringPosition = Vector3.MoveTowards(ringPosition, hit.point, ringDensity);
                toDelete.Add(Instantiate(laserRings, ringPosition, Quaternion.LookRotation(direction, Vector3.up)));
            }

            ringPosition = position;
            for (int i = 0; i < hit.distance / afterEffectDensity; i++)
            {
                ringPosition = Vector3.MoveTowards(ringPosition, hit.point, afterEffectDensity);
                toDeleteLater.Add(Instantiate(laserAfterEffects, ringPosition, Quaternion.LookRotation(direction, Vector3.up)));
            }

            if (hit.collider.tag.StartsWith("Player"))
            {
                PlayerHealth pHealth = hit.transform.GetComponent<PlayerHealth>();

                if (pHealth == null)
                    pHealth = hit.transform.GetComponentInParent<PlayerHealth>();

                pHealth?.DoDamage(2);

                return true;
            }
            else if (hit.collider.tag == "Wall" || hit.collider.tag == "ForceField")
            {
                if (bouncesLeft != 0)
                    return FireRay(hit.point, Vector3.Reflect(direction, hit.normal), --bouncesLeft);
                else
                    return false;
            }
            else
            {
                Debug.Log("Did not collide");
                return false;
            }
        }

        return result;
    }

    private void ClearEffects()
    {
        foreach (GameObject obj in toDelete)
            Destroy(obj);
    }

    private void ClearAfterEffects()
    {
        foreach (GameObject obj in toDeleteLater)
            Destroy(obj);
    }
}
