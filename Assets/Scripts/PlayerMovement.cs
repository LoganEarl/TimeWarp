using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int playerNumber = 1;
    public float turnSpeed = 20f, moveSpeed = 0.05f;
    public Transform targetingCursor;
    private Vector3 movement, mousePosition;
    private Quaternion desiredRotation = Quaternion.identity;
    
    private Animator animator;
    private Rigidbody rgbd;

    void Start() {
        animator = GetComponentInChildren<Animator>();
        rgbd = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        float horizontalMove = Input.GetAxis("Horizontal" + playerNumber);
        float verticalMove = Input.GetAxis("Vertical" + playerNumber);

        movement.Set(horizontalMove, 0f, verticalMove);
        movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontalMove, 0f);
        bool hasVerticalInput = !Mathf.Approximately(verticalMove, 0f);
        bool isIdle = !(hasHorizontalInput || hasVerticalInput);
        animator.SetBool("IsIdle", isIdle);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(Camera.main.transform.position, ray.direction, out RaycastHit hit, Mathf.Infinity))
            mousePosition = new Vector3(hit.point.x, 0.00001f, hit.point.z);

        targetingCursor.position = mousePosition;
        int animDirection = 2;

        if (!isIdle) {
            float walkingAngle = Vector3.SignedAngle(transform.forward, movement, Vector3.up);

            if (walkingAngle > -45f && walkingAngle < 45f) animDirection = 0;
            else if (walkingAngle > 45f && walkingAngle < 135f) animDirection = 1;
            else if (walkingAngle > -135f && walkingAngle < -45f) animDirection = 3;

            animator.SetInteger("WalkingAngle", animDirection);
        }

        Debug.Log(isIdle + ", animDirection: " + animDirection);

        Vector3 aimAt = mousePosition - transform.position;

        Vector3 desiredLook = Vector3.RotateTowards(transform.forward, aimAt, turnSpeed * Time.deltaTime, 0f);
        desiredLook.y = 0f;

        Quaternion desiredRotation = Quaternion.LookRotation(desiredLook);

        rgbd.MovePosition(rgbd.position + movement * moveSpeed);
        rgbd.MoveRotation(desiredRotation);
    }

    void OnAnimatorMove() {
    }
}
