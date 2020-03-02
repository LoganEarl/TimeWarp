using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : BasicForceField
{
    [SerializeField] private int shieldHealth = 3;
    [SerializeField] private readonly Color[] damagedColor;

    private new void Start()
    {
        base.Start();
        GetComponent<MeshCollider>().convex = true;
    }

    public void DoDamage()
    {
        shieldHealth--;

        if (shieldHealth <= 0)
            foreach (Transform go in GetComponentsInParent<Transform>())
                Destroy(go.gameObject);
        else
            GetComponent<MeshRenderer>().material.SetColor("_Color", damagedColor[shieldHealth + 1]);
    }
}
