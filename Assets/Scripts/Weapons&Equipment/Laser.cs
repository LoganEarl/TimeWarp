using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private int maxBounces;

    [SerializeField] private GameObject laser;
    [SerializeField] private GameObject laserRings;
    [SerializeField] private GameObject laserAfterEffects;

    [SerializeField] private float laserDensity = 0.1f;
    [SerializeField] private float ringDensity = 0.05f;
    [SerializeField] private float afterEffectDensity = 1.0f;
    [SerializeField] private float laserDuration = 0.75f;
    [SerializeField] private float afterEffectsDuration = 0.75f;

    private List<GameObject> toDelete = new List<GameObject>();
    private List<GameObject> toDeleteLater = new List<GameObject>();
    private bool friendlyFire;
    private LayerMask layerMask;

    public void Initialize(int playerNumber, Color playerColor)
    {
        friendlyFire = FindObjectOfType<AudioManager>().FriendlyFire;

        List<string> layerList = new List<string> { "Default", "ScreenTransitions", "Player0", "Player1", "ForceField0", "ForceField1" };
        if (!friendlyFire)
        {
            layerList.Remove("Player" + playerNumber);
            layerList.Remove("ForceField" + playerNumber);
        }

        layerMask = LayerMask.GetMask(layerList.ToArray());

        Material playerMaterial = ColorManager.Instance.GetPlayerMaterial(playerNumber, ColorManager.PlayerColorVarient.SPAWN_PRIMARY);
        laserRings.GetComponent<ParticleSystemRenderer>().material = playerMaterial;

        var main = laserRings.GetComponent<ParticleSystem>().main;
        main.startColor = playerColor;

        laserAfterEffects.GetComponent<ParticleSystemRenderer>().material = playerMaterial;
        main = laserAfterEffects.GetComponent<ParticleSystem>().main;
        main.startColor = playerColor;

        Vector3 fireDirection = new Vector3(0, transform.forward.y, transform.forward.z);

        FireRay(transform.position, transform.forward, maxBounces);

        Invoke("ClearEffects", laserDuration);
        Invoke("ClearAfterEffects", afterEffectsDuration);
    }

    private bool FireRay(Vector3 position, Vector3 direction, int bouncesLeft)
    {
        bool result = false;
        direction.y = 0f;

        if (Physics.Raycast(position, direction, out RaycastHit hit, 1000, layerMask))
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

                if (pHealth.Health <= 0)
                    result = bouncesLeft >= 0 ? FireRay(hit.point, direction, --bouncesLeft) : false;
                else
                    result = true;
            }
            else if (hit.collider.tag == "Wall" || hit.collider.tag == "ForceField")
            {
                if (bouncesLeft != 0)
                    result = FireRay(hit.point, Vector3.Reflect(direction, hit.normal), --bouncesLeft);
                else
                    result = false;
            }
            else
            {
                Debug.Log("Did not collide");
                result = false;
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

    private void OnDestroy()
    {
        ClearEffects();
        ClearAfterEffects();
    }
}
