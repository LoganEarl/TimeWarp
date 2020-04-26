using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public Vector3 explosionPos;

    private float radius = 5.0F;
    private float power = 10.0F;

    void Awake()
    {

        Destroy(GetComponentsInChildren<SkinnedMeshRenderer>()[0].gameObject);
        Destroy(transform.GetChild(0).gameObject);
        Destroy(GetComponentsInChildren<AudioSource>()[0].gameObject);
        Destroy(GetComponentsInChildren<AudioSource>()[1].gameObject);
        Destroy(GetComponentsInChildren<AudioSource>()[2].gameObject);
        Destroy(GetComponentsInChildren<AudioSource>()[3].gameObject);

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

            Destroy(child.gameObject, 5f);
        }

        ApplyExplosion();
    }

    void ApplyExplosion()
    {
        if(explosionPos == null) explosionPos = transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        foreach (Collider hit in colliders)
        {
            if (hit.tag == "DeadPlayer")
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
            }
        }
    }
}
