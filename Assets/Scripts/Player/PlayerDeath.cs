using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private float timeToFade = 2.0f;

    public Vector3 ExplosionPos { private get; set; }
    public Material PlayerMaterial { get; set; }

    private readonly float radius = 50.0f;
    private readonly float power = 10.0f;

    private new SkinnedMeshRenderer renderer;

    void Awake()
    {
        renderer = GetComponentsInChildren<SkinnedMeshRenderer>()[1];

        Destroy(transform.GetChild(0).gameObject);
        Destroy(GetComponentsInChildren<AudioSource>()[0].gameObject);
        Destroy(GetComponentsInChildren<AudioSource>()[1].gameObject);
        Destroy(GetComponentsInChildren<AudioSource>()[2].gameObject);
        Destroy(GetComponentsInChildren<AudioSource>()[3].gameObject);
        Destroy(GetComponentsInChildren<SkinnedMeshRenderer>()[0].gameObject);

        Transform[] allTransforms = GetComponentsInChildren<Transform>();

        foreach (Transform child in allTransforms)
        {
            CapsuleCollider childCollider = child.gameObject.AddComponent<CapsuleCollider>();
            childCollider.radius = .008f;

            Rigidbody childRigidbody = child.gameObject.AddComponent<Rigidbody>();
            childRigidbody.mass = 10;
            childRigidbody.angularDrag = 10;
            childRigidbody.drag = 3;

            child.parent = null;

            Destroy(child.gameObject, 3f);
        }
    }

    private void Start()
    {
        renderer.material = PlayerMaterial;
        ApplyExplosion();
    }

    private void FixedUpdate()
    {
        if (PlayerMaterial != null)
        {
            renderer.material = PlayerMaterial;

            PlayerMaterial.SetFloat(
                "_Alpha", Mathf.Lerp(
                    PlayerMaterial.GetFloat("_Alpha"),
                    0f,
                    timeToFade * Time.deltaTime
                )
            );
        }
    }

    void ApplyExplosion()
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
}
