using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a standin for a later system
public class PlayerController : MonoBehaviour
{
    private int speed = 1000;
    // Update is called once per frame
    void Update()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        rb.AddForce(direction * speed * Time.deltaTime);
    }
}
