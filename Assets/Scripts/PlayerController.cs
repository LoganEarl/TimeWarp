using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IRecordable
{
    //bullet information
    public float bulletSpeed = 10;
    private string fireButton;
    private bool firingGun = false;
    private bool fired = false;
    private List<GameObject> firedBullets = new List<GameObject>();

    //player components/info
    private Animator animator;
    private Rigidbody rigidBody;
    public Rigidbody bullet;
    public Transform fireTransform;
    private int playerNumber = 0;
    private bool usingSnapshots = false;
    public GameObject targetingCursor;
    private bool loadedCursor = false;
    private bool setupPlayer = false;
    private PlayerHealth health;

    //movement information
    public float turnSpeed, moveSpeed;
    private Vector3 position, velocity, lookDirection;
    private Quaternion desiredRotation = Quaternion.identity;
    private bool isIdle = true;
    private PlayerSnapshot snapshot;
    private int frameCounter = 0;

    void Start() {
        
    }

    void FixedUpdate() {
        if (setupPlayer && !health.Dead)
        {
            frameCounter++;
            if (frameCounter > 30)
            {
                frameCounter = 0;
                firedBullets.RemoveAll(bullet => bullet == null);
            }

            if (!loadedCursor && !usingSnapshots)
            {
                loadedCursor = true;
                targetingCursor = Instantiate(targetingCursor);
            }
            else if (usingSnapshots)
                Destroy(targetingCursor);

            velocity = rigidBody.velocity;
            position = rigidBody.position;

            if (usingSnapshots)
                RecordedFrame();
            else
                ControlledFrame();

            animator.SetBool("IsIdle", isIdle);
            int animDirection = 2;
            if (!isIdle)
            {
                float walkingAngle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);

                if (walkingAngle > -45f && walkingAngle < 45f) animDirection = 0;
                else if (walkingAngle > 45f && walkingAngle < 135f) animDirection = 1;
                else if (walkingAngle > -135f && walkingAngle < -45f) animDirection = 3;

                animator.SetInteger("WalkingAngle", animDirection);
            }
            rigidBody.velocity = velocity;

            if (usingSnapshots)
                rigidBody.transform.position = position;

            Quaternion desiredRotation = Quaternion.LookRotation(lookDirection);
            rigidBody.MoveRotation(desiredRotation);
            //rigidBody.transform.rotation = desiredRotation;

            if (firingGun)
                Shoot();
        }
    }

    private void ControlledFrame()
    {
        float horizontalMove = Input.GetAxis("Horizontal" + playerNumber);
        float verticalMove = Input.GetAxis("Vertical" + playerNumber);
        bool hasHorizontalInput = !Mathf.Approximately(horizontalMove, 0f);
        bool hasVerticalInput = !Mathf.Approximately(verticalMove, 0f);
        isIdle = !(hasHorizontalInput || hasVerticalInput);

        Vector3 inputVector = new Vector3();
        if (!isIdle)
        {
            inputVector.Set(horizontalMove, 0f, verticalMove);
            inputVector.Normalize();
            inputVector *= moveSpeed;
        }
        velocity += (inputVector * Time.fixedDeltaTime);

        Vector3 mousePosition = new Vector3();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(Camera.main.transform.position, ray.direction, out RaycastHit hit, Mathf.Infinity))
            mousePosition.Set(hit.point.x, 0.00001f, hit.point.z);
        targetingCursor.transform.position = mousePosition;
        Vector3 desiredLookAngle = mousePosition - transform.position;
        lookDirection = Vector3.RotateTowards(transform.forward, desiredLookAngle, turnSpeed * Time.deltaTime, 0f);
        lookDirection.y = 0f;

        firingGun = false;
        if (!fired && Input.GetButtonDown(fireButton))
        {
            fired = true;
            firingGun = true;
        }
        else if (!Input.GetButtonDown(fireButton))
            fired = false;
    }

    private void RecordedFrame()
    {
        position = snapshot.Translation;
        velocity = snapshot.Velocity;
        lookDirection = snapshot.LookDirection;
        firingGun = snapshot.Firing;
        isIdle = snapshot.IsIdle;
    }

    void OnAnimatorMove() {
    }

    private void Shoot()
    {
        Rigidbody bulletInstance = Instantiate(bullet, fireTransform.position, fireTransform.rotation) as Rigidbody;
        bulletInstance.GetComponent<Bullet>().playerNumber = playerNumber;
        bulletInstance.velocity = bulletSpeed * fireTransform.forward;
        firedBullets.Add(bulletInstance.gameObject);
    }

    private void DestroyAllBullets()
    {
        for (int i = 0; i < firedBullets.Count; i++)
            Destroy(firedBullets[i]);
        firedBullets.Clear();
    }

    public void SetPlayerInformation(int playerNumber, int sourceRoundNum, Vector3 initialPosition)
    {
        this.playerNumber = playerNumber;
        this.fireButton = "Fire" + playerNumber;

        animator = GetComponentInChildren<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        health = GetComponent<PlayerHealth>();

        rigidBody.MovePosition(initialPosition);

        setupPlayer = true;
    }

    public PlayerSnapshot GetSnapshot()
    {
        return new PlayerSnapshot(position, velocity, lookDirection, firingGun, isIdle);
    }

    public void SetSnapshot(PlayerSnapshot playerSnapshot)
    {
        snapshot = playerSnapshot;
    }

    public void SetUseSnapshots(bool useSnapshots)
    {
        this.usingSnapshots = useSnapshots;
    }

    public void OnReset()
    {
        DestroyAllBullets();
        health.FullHeal();
    }
}
