﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IRecordable
{
    //delegates
    public delegate bool FireEventCallback();
    public FireEventCallback FireCallback { private get; set; } = null;
    public delegate bool EquipmentEventCallback();
    public EquipmentEventCallback EquipmentCallback { private get; set; } = null;

    //equipment info
    private bool firingGun = false;
    private bool fired = false;
    private bool usingEquipment = false;
    private bool usedEquipment = false;
    private List<GameObject> roundClearingList = new List<GameObject>();

#pragma warning disable IDE0044
    [SerializeField] private float bulletSpeed = 10;
    [SerializeField] private Rigidbody bullet;
    [SerializeField] private Transform fireTransform;
    [SerializeField] private Transform shieldTransform;
    [SerializeField] private GameObject equipment;
    [SerializeField] private float lookOffset;
    [SerializeField] private GameObject targetingCursor;
    [SerializeField] private float turnSpeed, moveSpeed;
#pragma warning restore IDE0044

    //player components/info
    private int playerNumber = 0;
    private bool usingSnapshots = false;
    private bool aimLocked = false;
    private bool loadedCursor = false;
    private bool setupPlayer = false;
    private Animator animator;
    private Rigidbody rigidBody;
    private PlayerHealth health;
    private Color playerColor;

    //movement
    private bool isIdle = true;
    private int frameCounter = 0;
    private Vector3 position, velocity, lookDirection;
    private Quaternion desiredRotation = Quaternion.identity;
    private PlayerSnapshot snapshot;
    private IGameMode gameMode;

    void Start()
    {
        lookDirection = new Vector3(0, 0, 1);
    }

    void FixedUpdate()
    {
        if (setupPlayer && !health.Dead)
        {
            frameCounter++;
            if (frameCounter > 30)
            {
                frameCounter = 0;
                roundClearingList.RemoveAll(gameObject => gameObject == null);
            }

            if (!loadedCursor && !usingSnapshots)
            {
                loadedCursor = true;
                targetingCursor = Instantiate(targetingCursor);
            }
            else if (usingSnapshots)
                Destroy(targetingCursor);

            //read in the calculated values from the rigidbody
            velocity = rigidBody.velocity;
            position = rigidBody.position;

            if (usingSnapshots)
                RecordedFrame();
            else
            {
                ControlledFrame();
                if (gameMode.GameState.PlayersPositionsLocked)
                {
                    velocity = new Vector3();
                    isIdle = true;
                }
                
            }

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

            Quaternion desiredRotation;

            if (lookDirection.magnitude > 0.1)
            {
                Vector3 moddedDirection = Quaternion.AngleAxis(lookOffset, Vector3.up) * lookDirection;
                desiredRotation = Quaternion.LookRotation(moddedDirection, Vector3.up);
                if (!gameMode.GameState.PlayersLookLocked)
                    rigidBody.MoveRotation(desiredRotation);
                //TODO: this does not belong here. Make a targetingCursor script to handle this
                if (!usingSnapshots)
                {
                    targetingCursor.SetActive(true);
                    targetingCursor.transform.position = rigidBody.position + lookDirection * 5 + new Vector3(0, fireTransform.position.y, 0);
                }
            }
            else if (velocity.magnitude > 0.2)
            {
                Vector3 moddedDirection = Quaternion.AngleAxis(lookOffset, Vector3.up) * velocity;
                moddedDirection.y = 0;
                desiredRotation = Quaternion.LookRotation(moddedDirection, Vector3.up);
                if(!gameMode.GameState.PlayersLookLocked)
                    rigidBody.MoveRotation(desiredRotation);
                if (!usingSnapshots)
                    targetingCursor.SetActive(false);
            }
            else if (!usingSnapshots)
                targetingCursor.SetActive(false);
            if (firingGun && !gameMode.GameState.PlayersFireLocked && (FireCallback?.Invoke() ?? true))
                Shoot();
            if (usingEquipment && !gameMode.GameState.PlayersFireLocked && (EquipmentCallback?.Invoke() ?? true))
                PlaceEquipment();
        }
    }

    private void ControlledFrame()
    {
        float horizontalMove = Input.GetAxis("Horizontal" + playerNumber);
        float verticalMove = Input.GetAxis("Vertical" + playerNumber);

        bool hasHorizontalInput = DeltaExceeds(horizontalMove, 0f, 0.02f);
        bool hasVerticalInput = DeltaExceeds(verticalMove, 0f, 0.02f);
        isIdle = !(hasHorizontalInput || hasVerticalInput);

        Vector3 inputVector = new Vector3(horizontalMove, 0f, verticalMove);
        inputVector *= moveSpeed;
        velocity += (inputVector * Time.fixedDeltaTime);

        float horizontalAim = Input.GetAxis("AimHorizontal" + playerNumber);
        float verticalAim = Input.GetAxis("AimVertical" + playerNumber);

        bool hasHorizontalAim = DeltaExceeds(horizontalAim, 0f, 0.02f);
        bool hasVerticalAim = DeltaExceeds(verticalAim, 0f, 0.02f);

        if ((hasHorizontalAim || hasVerticalAim) && !aimLocked)
            lookDirection = new Vector3(horizontalAim, 0f, verticalAim);
        else if (lookDirection.magnitude < 0.03)
            lookDirection.Set(0, 0, 0);

        float fireActivity = Input.GetAxis("Fire" + playerNumber);
        float equipmentActivity = Input.GetAxis("Equipment" + playerNumber);

        firingGun = false;
        if (!fired && fireActivity > 0f) //this works becuase of the masssive deadzone setting
        {
            fired = true;
            firingGun = true;
        }
        else if (fireActivity == 0f)
            fired = false;

        usingEquipment = false;
        if (!usedEquipment && equipmentActivity > 0f)
        {
            usedEquipment = true;
            usingEquipment = true;
        }
        else if (equipmentActivity == 0f)
            usedEquipment = false;


        if (Input.GetButtonDown("AimLock" + playerNumber))
            aimLocked = !aimLocked;
    }

    private static bool DeltaExceeds(float value, float target, float delta)
    {
        var actualDifference = Mathf.Abs(value - target);
        return actualDifference > delta;
    }

    private void RecordedFrame()
    {
        position = snapshot.Translation;
        velocity = snapshot.Velocity;
        lookDirection = snapshot.LookDirection;
        firingGun = snapshot.Firing;
        usingEquipment = snapshot.UsingEquipment;
        isIdle = snapshot.IsIdle;
    }

    private void PlaceEquipment()
    {
        GameObject shieldInstance = Instantiate(equipment, shieldTransform.position, shieldTransform.rotation) as GameObject;
        roundClearingList.Add(shieldInstance.gameObject);
    }

    private void Shoot()
    {
        Rigidbody bulletInstance = Instantiate(bullet, fireTransform.position, fireTransform.rotation) as Rigidbody;
        bulletInstance.GetComponent<Bullet>().playerNumber = playerNumber;
        bulletInstance.GetComponent<Bullet>().bulletColor = playerColor;
        bulletInstance.velocity = fireTransform.forward;

        roundClearingList.Add(bulletInstance.gameObject);
    }

    private void DestroyAllPlayerCreations()
    {
        for (int i = 0; i < roundClearingList.Count; i++)
            Destroy(roundClearingList[i]);
        roundClearingList.Clear();
    }

    public void SetPlayerInformation(int playerNumber, int sourceRoundNum, Vector3 initialPosition, IGameMode gameMode)
    {
        this.playerNumber = playerNumber;

        animator = GetComponentInChildren<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        health = GetComponent<PlayerHealth>();

        Collider[] setColliderTags = GetComponentsInChildren<Collider>();
        foreach (Collider collider in setColliderTags)
            collider.gameObject.tag = "Player" + playerNumber;

        rigidBody.MovePosition(initialPosition);

        this.gameMode = gameMode;

        setupPlayer = true;
    }

    public PlayerSnapshot GetSnapshot()
    {
        return new PlayerSnapshot(position, velocity, lookDirection, firingGun, usingEquipment, isIdle);
    }

    public void SetSnapshot(PlayerSnapshot playerSnapshot)
    {
        snapshot = playerSnapshot;
    }

    public void SetUseSnapshots(bool useSnapshots)
    {
        this.usingSnapshots = useSnapshots;

        SkinnedMeshRenderer secondaryRenderer = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>()[0];
        SkinnedMeshRenderer primaryRenderer = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>()[1];
        if (useSnapshots)
        {
            gameObject.layer = LayerMask.NameToLayer("BlockingLayer");
            primaryRenderer.material = Resources.Load<Material>("Materials/Player/Player" + playerNumber + "Primary");
            secondaryRenderer.material = Resources.Load<Material>("Materials/Player/Player" + playerNumber + "Secondary");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Ghost");
            primaryRenderer.material = Resources.Load<Material>("Materials/Player/Player" + playerNumber + "PrimaryGhost");
            secondaryRenderer.material = Resources.Load<Material>("Materials/Player/Player" + playerNumber + "SecondaryGhost");
        }

        playerColor = primaryRenderer.material.GetColor("_GlowColor");
    }

    public void OnReset()
    {
        DestroyAllPlayerCreations();
        health.FullHeal();
        health.ResetStatistics();
    }

    public void OnDestroy() => DestroyAllPlayerCreations();
}
