using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int playerNumber = 1;
    public float turnSpeed = 20f, moveSpeed = 0.05f;
    public Transform targetingCursor, fireTransform;
    Vector3 movement, mousePosition, aimAt;
    Quaternion desiredRotation = Quaternion.identity, torsoRotation = Quaternion.identity;

    Animator animator;
    Rigidbody rgbd;

    void Start() {
        animator = GetComponentInChildren<Animator>();
        rgbd = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        float horizontalMove = Input.GetAxis("Horizontal" + playerNumber);
        float verticalMove = Input.GetAxis("Vertical" + playerNumber);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, ray.direction, out hit, Mathf.Infinity))
            mousePosition = new Vector3(hit.point.x, 0.00001f, hit.point.z);

        targetingCursor.position = mousePosition;
        //Vector3 aimAt = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z + 2);
        
        movement.Set(horizontalMove, 0f, verticalMove);
        movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontalMove, 0f);
        bool hasVerticalInput = !Mathf.Approximately(verticalMove, 0f);
        bool isIdle = !(hasHorizontalInput || hasVerticalInput);

        animator.SetBool("IsIdle", isIdle);
        float walkingAngle = Vector3.Angle(rgbd.velocity, transform.forward);


        if (!isIdle)  animator.SetFloat("WalkingAngle", walkingAngle);

        Debug.Log("isIdle: " + isIdle + "WalkingAngle: " + walkingAngle);

        Vector3 desiredLook = Vector3.RotateTowards(transform.forward, targetingCursor.position, turnSpeed * Time.deltaTime, 0f);
        desiredLook.y = 0f;

        desiredRotation = Quaternion.LookRotation(desiredLook);

        rgbd.MovePosition(rgbd.position + movement * moveSpeed);
        rgbd.MoveRotation(desiredRotation);
    }

    void OnAnimatorMove() {
    }
}
