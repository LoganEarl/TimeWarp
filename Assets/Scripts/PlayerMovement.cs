using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int playerNumber = 1;
    public float turnSpeed = 20f, moveSpeed = 0.05f;
    public Transform targetingCursor, fireTransform;
    Vector3 movement, mousePosition, aimAt, lastPosition, curVelocity;
    Quaternion desiredRotation = Quaternion.identity, torsoRotation = Quaternion.identity;

    private float weaponHeight;
    Animator animator;
    Rigidbody rgbd;

    void Start() {
        animator = GetComponentInChildren<Animator>();
        rgbd = GetComponent<Rigidbody>();
        weaponHeight = fireTransform.TransformPoint(fireTransform.position).y;
    }

    void FixedUpdate() {
        float horizontalMove = Input.GetAxis("Horizontal" + playerNumber);
        float verticalMove = Input.GetAxis("Vertical" + playerNumber);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, ray.direction, out hit, Mathf.Infinity))
            mousePosition = new Vector3(hit.point.x, 0.00001f, hit.point.z);

        targetingCursor.position = mousePosition;
        curVelocity = (rgbd.position - lastPosition).normalized;
        Vector3 aimAt = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);
        
        movement.Set(horizontalMove, 0f, verticalMove);
        movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontalMove, 0f);
        bool hasVerticalInput = !Mathf.Approximately(verticalMove, 0f);
        bool isIdle = !(hasHorizontalInput || hasVerticalInput);
        animator.SetBool("IsIdle", isIdle);

        float walkingAngle = Vector3.SignedAngle(transform.forward, curVelocity, Vector3.up);
        int animDirection = 2;

        if (walkingAngle > -45f && walkingAngle < 45f) animDirection = 0;
        else if (walkingAngle > 45f && walkingAngle < 135f) animDirection = 1;
        else if (walkingAngle > -135f && walkingAngle < -45f) animDirection = 3;

        animator.SetInteger("WalkingAngle", animDirection);

        aimAt = aimAt - transform.position;
        Debug.Log("transform Position: " + transform.forward + ", Aiming at: " + aimAt);

        Vector3 desiredLook = Vector3.RotateTowards(transform.forward, aimAt, turnSpeed * Time.deltaTime, 0f);
        desiredLook.y = 0f;

        desiredRotation = Quaternion.LookRotation(desiredLook);
        lastPosition = rgbd.position;

        rgbd.MovePosition(rgbd.position + movement * moveSpeed);
        rgbd.MoveRotation(desiredRotation);

    }

    void OnAnimatorMove() {
    }
}
