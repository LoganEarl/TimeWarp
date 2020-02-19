using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFade : MonoBehaviour
{
    private float pathLength = 1f;
    private float adjustableAlpha;

    void Start()
    {
        adjustableAlpha = 0.5f;
        gameObject.GetComponent<MeshRenderer>().material.SetFloat(
            "_AdjustableAlpha",
            adjustableAlpha
            );
    }
    
    void FixedUpdate()
    {
        gameObject.GetComponent<MeshRenderer>().material.SetFloat(
            "_AdjustableAlpha",
            adjustableAlpha
            );
        

        adjustableAlpha = adjustableAlpha - (pathLength * Time.fixedDeltaTime);

        if (adjustableAlpha < 0) Destroy(gameObject, 0f);
    }

    public void SetPathLength(float length) { pathLength = length; }
}
