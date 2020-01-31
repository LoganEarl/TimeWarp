using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int playerNumber = 1;
    public float turnSpeed = 20f;
    public float moveSpeed = 20f;
    Vector3 movement;
    Quaternion rotation = Quaternion.identity;

    Animator animator;
    Rigidbody rgbd;

    void Start() {
        animator = GetComponent<Animator>();
        rgbd = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        float horizontal = Input.GetAxis("Horizontal" + playerNumber);
        float vertical = Input.GetAxis("Vertical" + playerNumber);

        movement.Set(horizontal, 0f, vertical);
        movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;

        animator.SetBool("IsWalking", isWalking);

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, movement, turnSpeed * Time.deltaTime, 0f);
        rotation = Quaternion.LookRotation(desiredForward);
    }

    void OnAnimatorMove() {
        rgbd.MovePosition(rgbd.position + movement * moveSpeed);
        rgbd.MoveRotation(rotation);
    }
}
