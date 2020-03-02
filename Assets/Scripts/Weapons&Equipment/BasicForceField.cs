using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicForceField : MonoBehaviour
{
    // Start is called before the first frame update
    protected void Start()
    {
        Mesh temp = GetComponent<MeshFilter>().mesh;
        Vector3[] normals = temp.normals;
        Vector3[] vertices = temp.vertices;

        for (int i = 0; i < temp.vertexCount; i++)
        {
            if (normals[i].z < 0)
            {
                normals[i] = Vector3.back;
                vertices[i].z = vertices[i].z / 2 * -1;
            }
        }

        temp.normals = normals;
        temp.vertices = vertices;
    }
}
