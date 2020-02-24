using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWeapon : MonoBehaviour
{
    [SerializeField]
    private GameObject weaponModel;
    [SerializeField]
    private bool rotateClockwise;

    private float speed = 30.0f;

    void Update()
    {
        if (rotateClockwise)
            weaponModel.transform.Rotate(Vector3.down * speed * Time.deltaTime);
        else
            weaponModel.transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}
