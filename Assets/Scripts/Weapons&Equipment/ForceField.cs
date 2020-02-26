using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    public int shieldHealth = 3;
    public Color Damaged1;
    public Color Damaged2;

    void Start()
    {
        Mesh temp = GetComponent<MeshFilter>().mesh;
        Vector3[] normals = temp.normals;
        Vector3[] vertices = temp.vertices;

        for(int i = 0; i < temp.vertexCount; i++)
        {
            if (normals[i].z < 0)
            {
                normals[i] = Vector3.back;
                vertices[i].z = vertices[i].z / 2 * -1;
            }
        }

        temp.normals = normals;
        temp.vertices = vertices;

        GetComponent<MeshCollider>().convex = true;
    }

    public void DoDamage()
    {
        shieldHealth--;

        if (shieldHealth <= 0)
            foreach (Transform go in GetComponentsInParent<Transform>())
                Destroy(go.gameObject);


        if (shieldHealth == 2) 
            GetComponent<MeshRenderer>().material.SetColor("_Color", Damaged1);
        else if (shieldHealth == 1) 
            GetComponent<MeshRenderer>().material.SetColor("_Color", Damaged2);

    }
}
