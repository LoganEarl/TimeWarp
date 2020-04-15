﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IRecordable
{
    #region delegates
    public delegate bool FireEventCallback();
    public FireEventCallback FireCallback { private get; set; } = null;
    public delegate bool EquipmentEventCallback();
    public EquipmentEventCallback EquipmentCallback { private get; set; } = null;
    #endregion

    #region equipment
    private bool firingGun = false;
    private bool fired = false;
    private bool usingEquipment = false;
    private bool usedEquipment = false;
    private GameObject shieldPlacer;
    private List<GameObject> roundClearingList = new List<GameObject>();
    #endregion

    #region SerializableFields
#pragma warning disable IDE0044
    [SerializeField] private Transform fireTransform;
    [SerializeField] private Rigidbody bullet;
    [SerializeField] private GameObject targetingCursor;
    [SerializeField] private Transform shieldTransform;
    [SerializeField] private GameObject equipment;
    [SerializeField] private GameObject equipmentGuide;
    [SerializeField] private float lookOffset;
    [SerializeField] private float turnSpeed, moveSpeed;
    [SerializeField] private int lookSnap = 5;
    [SerializeField] private float lookMagnitude = 16; //how far they can look in splitscreen
    [SerializeField] private Vector3 cameraHeight = new Vector3(0, 10, 0);
#pragma warning restore IDE0044
    #endregion

    #region Components
    //player components/info
    private static bool talking = false;
    [SerializeField] private int playerNumber = 0;
    private bool usingSnapshots = false;
    private bool aimLocked = false;
    private bool loadedCursor = false;
    private Animator animator;
    private Rigidbody rigidBody;
    private PlayerHealth health;
    private Color playerColor;
    private AudioSource voiceLine;
    #endregion

    #region movement
    private bool isIdle = true;
    private int frameCounter = 0;
    private Vector3 position, velocity, lookDirection;
    private Quaternion desiredRotation = Quaternion.identity;
    private PlayerSnapshot snapshot;
    private IGameMode gameMode;
    public Vector3 CameraPosition
    {
        get { return position + (lookDirection * .5f) + cameraHeight; }
    }
    #endregion

    private void Awake()
    {
        lookDirection = new Vector3(0, 0, 1);
        voiceLine = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        health = GetComponent<PlayerHealth>();
    }

    private void FixedUpdate()
    {
        if (!health.Dead && gameMode.GameState.PlayersVisible)
        {
            gameObject.transform.localScale = new Vector3(2, 2, 2); //unhides the player if they are hidden

            frameCounter++;
            if (frameCounter > 30) //maxFrames or something
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
            position = rigidBody.position; // seperate method?
            //end

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
            //write the calculated values to the rigidbody depending on type of player moving
            rigidBody.velocity = velocity;
            if (usingSnapshots)
                rigidBody.transform.position = position;

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

            Quaternion desiredRotation;
            bool isLooking = lookDirection.magnitude > 0;
            bool isMoving = velocity.magnitude > 0.2;

            if (isLooking) {
                Vector3 moddedDirection = Quaternion.AngleAxis(lookOffset, Vector3.up) * lookDirection;

                float lookAngle = Vector3.Angle(Vector3.forward, moddedDirection) % 45;
                if (lookAngle > lookSnap) lookAngle -= 45;

                if (Mathf.Abs(lookAngle) <= lookSnap)
                    moddedDirection = Quaternion.AngleAxis(lookAngle, Vector3.up) * moddedDirection;

                desiredRotation = Quaternion.LookRotation(moddedDirection, Vector3.up);
                rigidBody.MoveRotation(desiredRotation);
            }
            else if (isMoving) {
                Vector3 moddedDirection = Quaternion.AngleAxis(lookOffset, Vector3.up) * velocity;
                moddedDirection.y = 0;

                desiredRotation = Quaternion.LookRotation(moddedDirection, Vector3.up);
                if (!gameMode.GameState.PlayersLookLocked)
                    rigidBody.MoveRotation(desiredRotation);
            }

            if (targetingCursor != null) { 
                targetingCursor.SetActive(isLooking && !usingSnapshots);

                if(targetingCursor.activeInHierarchy)
                    targetingCursor.transform.position =
                         rigidBody.position + lookDirection + new Vector3(0, fireTransform.position.y, 0);
            }

            if (firingGun && !gameMode.GameState.PlayersFireLocked && (FireCallback?.Invoke() ?? true))
                Shoot();

            if (!usingEquipment && usedEquipment && !gameMode.GameState.PlayersFireLocked && (EquipmentCallback?.Invoke() ?? true))
                PlaceEquipment();
        }
        else
        {
            gameObject.transform.localScale = new Vector3(0, 0, 0); //hides the player without deactiviating
            if (targetingCursor != null) targetingCursor.SetActive(false);
            if (shieldPlacer != null) Destroy(shieldPlacer);
            
            //foreach (Transform obj in gameObject.GetComponentsInChildren<Transform>())
            //    obj.gameObject.layer = LayerMask.NameToLayer("Player" + playerNumber);
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

        bool hasHorizontalAim = DeltaExceeds(horizontalAim, 0f, 0.1f);
        bool hasVerticalAim = DeltaExceeds(verticalAim, 0f, 0.1f);

        if ((hasHorizontalAim || hasVerticalAim) && !aimLocked) {
            lookDirection = new Vector3(horizontalAim, 0f, verticalAim);
            lookDirection *= lookMagnitude;
        }
        else if (!aimLocked)
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

        usedEquipment = false;
        if (!usingEquipment && equipmentActivity > 0f && !gameMode.GameState.PlayersFireLocked && gameMode.EquipmentRemaining(playerNumber) != 0)
        {
            usingEquipment = true;
            PlacingEquipmentGuide();
        }
        else if (usingEquipment && equipmentActivity == 0f)
        {
            Destroy(shieldPlacer);
            usedEquipment = true;
            usingEquipment = false;
        }
        else if (equipmentActivity == 0f)
            usingEquipment = false;


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
        usedEquipment = snapshot.UsedEquipment;
        isIdle = snapshot.IsIdle;
    }

    private void PlacingEquipmentGuide()
    {
        shieldPlacer = Instantiate(equipmentGuide, shieldTransform);
        shieldPlacer.transform.localScale = new Vector3(.1f, .1f, .1f);
    }

    private void PlaceEquipment()
    {
        GameObject shieldInstance = Instantiate(equipment, shieldTransform.position, shieldTransform.rotation) as GameObject;
        shieldInstance.layer = LayerMask.NameToLayer("ForceField" + playerNumber);
        roundClearingList.Add(shieldInstance.gameObject);
    }

    private void Shoot()
    {
        Rigidbody bulletInstance = Instantiate(bullet, fireTransform.position, fireTransform.rotation) as Rigidbody;
        bulletInstance.GetComponent<Bullet>().bulletColor = playerColor;
        bulletInstance.GetComponent<Bullet>().playerNumber = playerNumber;

        bulletInstance.gameObject.layer = LayerMask.NameToLayer("Projectile" + playerNumber);
        bulletInstance.velocity = fireTransform.forward;

        roundClearingList.Add(bulletInstance.gameObject);

        bool speaking = (Random.value * 100) <= 40;
        //int randomSound = Mathf.RoundToInt(Random.value * (smacktalk.Length - 1));
        
        //if (speaking)
        //{
        //    if (!talking)
        //    {
        //        voiceLine.PlayOneShot(smacktalk[randomSound]);
        //        talking = true;
        //        Invoke("TalkingStopped", smacktalk[randomSound].length);
        //    }
        //}
    }

    private void TalkingStopped() { talking = false; }

    private void DestroyAllPlayerCreations()
    {
        for (int i = 0; i < roundClearingList.Count; i++)
            Destroy(roundClearingList[i]);
        roundClearingList.Clear();
    }

    public void SetPlayerInformation(int playerNumber, int sourceRoundNum, Vector3 initialPosition, IGameMode gameMode)
    {
        this.playerNumber = playerNumber;

        Collider[] setColliderTags = GetComponentsInChildren<Collider>();
        foreach (Collider collider in setColliderTags)
            collider.gameObject.tag = "Player" + playerNumber;

        rigidBody.MovePosition(initialPosition);

        this.gameMode = gameMode;
    }

    public PlayerSnapshot GetSnapshot()
    {
        return new PlayerSnapshot(position, velocity, lookDirection, firingGun, usingEquipment, usedEquipment, isIdle);
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
            foreach (Transform obj in gameObject.GetComponentsInChildren<Transform>())
                obj.gameObject.layer = LayerMask.NameToLayer("Player" + playerNumber);

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
