using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float timeToFade = 2.0f;
    [SerializeField] private Transform weaponsTransform;

    public Vector3 ExplosionPos { private get; set; }
    public Material PlayerMaterial { get; set; }

    private readonly float radius = 50.0f;
    private readonly float power = 10.0f;

    private new SkinnedMeshRenderer renderer;
    private Transform[] allTransforms;

    void Awake()
    {

        renderer = GetComponentsInChildren<SkinnedMeshRenderer>()[1];

        renderer.material = ColorManager.Instance.GetPlayerMaterial(0, ColorManager.PlayerColorVarient.MODEL_PRIMARY_ACTIVE);
        Destroy(transform.GetChild(0).gameObject);
        Destroy(GetComponentsInChildren<AudioSource>()[0].gameObject);
        Destroy(GetComponentsInChildren<AudioSource>()[1].gameObject);
        Destroy(GetComponentsInChildren<AudioSource>()[2].gameObject);
        Destroy(GetComponentsInChildren<AudioSource>()[3].gameObject);
        Destroy(GetComponentsInChildren<SkinnedMeshRenderer>()[0].gameObject);

        allTransforms = GetComponentsInChildren<Transform>();

        foreach (Transform child in allTransforms)
        {
            CapsuleCollider childCollider = child.gameObject.AddComponent<CapsuleCollider>();
            childCollider.radius = .008f;

            Rigidbody childRigidbody = child.gameObject.AddComponent<Rigidbody>();
            childRigidbody.mass = 10;
            childRigidbody.angularDrag = 10;
            childRigidbody.drag = 3;

            child.parent = null;

            //Destroy(child.gameObject, 5f);
        }
    }

    private void Start()
    {
        renderer.material = PlayerMaterial;
        ApplyExplosion();
    }

    private void FixedUpdate()
    {
        float alpha = 1;

        if (PlayerMaterial != null)
        {
            alpha = PlayerMaterial.GetFloat("_Alpha");

            renderer.material = PlayerMaterial;

            PlayerMaterial.SetFloat(
                "_Alpha", Mathf.Lerp(
                    alpha,
                    0f,
                    timeToFade
                )
            );
        }

        Debug.Log("Alpha: " + alpha);

        if (alpha <= 0.25)
            DestroyAllTransforms();
    }

    public void SetHeldWeapon(string weaponName)
    {
        if ("Pistol" != weaponName)
        {
            Transform newWeapon = weaponsTransform.Find(weaponName);
            Transform oldWeapon = weaponsTransform.Find("Pistol");

            if (newWeapon != null && oldWeapon != null)
            {
                oldWeapon.gameObject?.SetActive(false);
                newWeapon.gameObject?.SetActive(true);
            }
        }
    }

    private void ApplyExplosion()
    {
        if(ExplosionPos == null) ExplosionPos = transform.position;

        Collider[] colliders = Physics.OverlapSphere(ExplosionPos, radius);
            
        foreach (Collider hit in colliders)
        {
            if (hit.tag == "DeadPlayer")
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.AddExplosionForce(power, ExplosionPos, radius, 3.0F);
            }
        }
    }

    private void DestroyAllTransforms()
    {
        foreach (Transform t in allTransforms)
            if(t != null)
                Destroy(t.gameObject);
        Destroy(this.gameObject);
    }
}
