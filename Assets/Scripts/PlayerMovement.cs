using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int playerNumber = 1;
    public float turnSpeed = 20f, moveSpeed = 20f;
    Vector3 movement, mousePosition;
    Quaternion rotation = Quaternion.identity, torsoRotation = Quaternion.identity;

    Animator animator;
    Rigidbody rgbd, torsoRGBD;

    void Start() {
        animator = GetComponent<Animator>();
        rgbd = GetComponent<Rigidbody>();
        torsoRGBD = GetComponentInChildren<Rigidbody>();
    }

    void FixedUpdate() {
        float horizontalMove = Input.GetAxis("Horizontal" + playerNumber);
        float verticalMove = Input.GetAxis("Vertical" + playerNumber);

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 2f;
        mousePosition.y = 0;

        movement.Set(horizontalMove, 0f, verticalMove);
        movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontalMove, 0f);
        bool hasVerticalInput = !Mathf.Approximately(verticalMove, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;

        //animator.SetBool("IsWalking", isWalking);

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, movement, turnSpeed * Time.deltaTime, 0f);
        Vector3 desiredLook = Vector3.RotateTowards(torsoRGBD.transform.forward, mousePosition, turnSpeed * Time.deltaTime, 0f);
        rotation = Quaternion.LookRotation(desiredForward);
        //torsoRotation = Quaternion.LookRotation(desiredForward);

        rgbd.MovePosition(rgbd.position + movement * moveSpeed);
        //torsoRGBD.MovePosition(torsoRGBD.position + movement * moveSpeed);
        rgbd.MoveRotation(rotation);
        //torsoRGBD.MoveRotation(rotation);
    }

    void OnAnimatorMove() {
    }
}
