using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public float gravityScale;
    public CharacterController controller;

    private Vector3 moveDirection;

    // Start is called before the first frame update
    void Start() {
        controller = GetComponent<CharacterController>();
        moveDirection = new Vector3();
    }

    // Update is called once per frame
    void FixedUpdate() {
        moveDirection = new Vector3() {
            x = moveSpeed * Input.GetAxis("Horizontal"),
            y = moveDirection.y,
            z = moveSpeed * Input.GetAxis("Vertical")
        };

        if (controller.isGrounded) {
            moveDirection.y = 0;
            if (Input.GetButtonDown("Jump")) {
                moveDirection.y = jumpForce;
            }
        }

        moveDirection.y += Physics.gravity.y * gravityScale * Time.fixedDeltaTime;
        controller.Move(moveDirection * Time.fixedDeltaTime);
    }
}
