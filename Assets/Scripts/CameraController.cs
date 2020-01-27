using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform target;
    public Vector3 offset;
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start() {
        offset = target.position - transform.position;
    }

    // Update is called once per frame
    void Update() {
        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
        target.Rotate(0, horizontal, 0);

        float desiredYAngle = target.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, desiredYAngle, 0);
        transform.position = target.position - rotation * offset;

        transform.LookAt(target);
    }
}
