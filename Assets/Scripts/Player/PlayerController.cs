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
    public bool FiringGun { get; private set; } = false;
    private bool fired = false;
    public bool UsingEquipment { get; private set; } = false;
    private bool placingEquipment = false;
    private bool usedEquipment = false;
    private GameObject shieldPlacer;
    #endregion

    #region SerializableFields
#pragma warning disable IDE0044
    [SerializeField] private Transform shieldTransform;
    [SerializeField] private Transform weaponsTransform;

    [SerializeField] private GameObject equipment;
    [SerializeField] private GameObject equipmentGuide;

    [SerializeField] private float lookOffset;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float moveSpeed;

    [SerializeField] private Vector3 cameraHeight = new Vector3(0, 10, 0);
    [SerializeField] private int lookSnap = 5;
    [SerializeField] private float lookMagnitude = 16; //how far they can look in splitscreen
#pragma warning restore IDE0044
    #endregion

    #region Components
    public int PlayerNumber { get; private set; } = 0;
    public IWeapon Weapon { get; private set; }
    public int RoundNumber { get; private set; } = 0;
    private string ghostLayerName = "Ghost";
    private string PlayerLayerName { get => "Player" + PlayerNumber; }
    private string currentLayer = "Unset";
    public bool UsingSnapshots { get; private set; } = false;
    private bool aimLocked = false;
    private Animator animator;
    private Rigidbody rigidBody;
    private PlayerHealth health;
    private Color playerColor;
    private AudioManager audioManager;
    private RandomShoot shootSound;
    private RandomShield shieldSound;
    private bool loadedCursor = false;
    private GameObject playerModel;
    private GameObject targetingCursor;

    public bool IsVisible {
        get => GameMode.GameState.GetPlayerVisible(PlayerNumber, RoundNumber);
        set {
            if (value) playerModel.transform.localScale = new Vector3(1, 1, 1);
            else playerModel.transform.localScale = new Vector3(0, 0, 0);
        }
    }
    #endregion

    #region movement
    private bool isIdle = true;
    private Vector3 position, velocity;
    public Vector3 LookDirection { get; set; }
    private Quaternion desiredRotation = Quaternion.identity;
    private PlayerSnapshot snapshot;
    public IGameMode GameMode { get; private set; }
    public Vector3 CameraPosition
    {
        get { return position + (LookDirection * .5f) + cameraHeight; }
    }
    #endregion

    private void Awake()
    {
        LookDirection = new Vector3(0, 0, 1);
        audioManager = FindObjectOfType<AudioManager>();
        animator = GetComponentInChildren<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        health = GetComponent<PlayerHealth>();
        shootSound = GetComponent<RandomShoot>();
        shieldSound = GetComponent<RandomShield>();
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if(child.gameObject.name == "Model")
            {
                playerModel = child.gameObject;
                break;
            }
        }
        Weapon = GetComponentsInChildren<IWeapon>()[0];
        animator.SetInteger("WeaponType", Weapon.WeaponType);
    }

    private void LateUpdate()
    {
        IsVisible = GameMode?.GameState.GetPlayerVisible(PlayerNumber, RoundNumber) ?? false && !health.Dead;
        if (!health.Dead && IsVisible)
        {
            if (!loadedCursor && !UsingSnapshots)
            {
                loadedCursor = true;
                targetingCursor = Instantiate(Weapon.TargetingCursor);
                targetingCursor.GetComponent<PlayerCursor>().Player = this;
            }

            //read in the calculated values from the rigidbody
            velocity = rigidBody.velocity;
            position = rigidBody.position; // seperate method?
            //end

            SetLayer(GameMode.GameState.GetPlayerCanTakeDamage(PlayerNumber, RoundNumber) ? PlayerLayerName : ghostLayerName);

            if (UsingSnapshots)
                RecordedFrame();
            else
            {
                ControlledFrame();
                if (GameMode.GameState.GetPlayerPositionsLocked(PlayerNumber, RoundNumber))
                {
                    velocity = new Vector3();
                    isIdle = true;
                }
            }

            //write the calculated values to the rigidbody depending on type of player moving
            rigidBody.velocity = velocity;
            if (UsingSnapshots)
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
            bool isLooking = LookDirection.magnitude > 0.1;
            bool isMoving = velocity.magnitude > 0.2;

            if (isLooking) {
                Vector3 moddedDirection = LookDirection;

                float lookAngle = Vector3.Angle(Vector3.forward, moddedDirection) % 45;
                if (lookAngle > lookSnap) lookAngle -= 45;

                if (Mathf.Abs(lookAngle) <= lookSnap)
                    moddedDirection = Quaternion.AngleAxis(lookAngle, Vector3.up) * moddedDirection;

                desiredRotation = Quaternion.LookRotation(moddedDirection, Vector3.up);
                rigidBody.MoveRotation(desiredRotation);
            }
            else if (isMoving) {
                Vector3 moddedDirection = velocity;
                moddedDirection.y = 0;

                float lookAngle = Vector3.Angle(Vector3.forward, moddedDirection) % 45;
                if (lookAngle > lookSnap) lookAngle -= 45;

                if (Mathf.Abs(lookAngle) <= lookSnap)
                    moddedDirection = Quaternion.AngleAxis(lookAngle, Vector3.up) * moddedDirection;

                desiredRotation = Quaternion.LookRotation(moddedDirection, Vector3.up);
                if (!GameMode.GameState.GetPlayerLookLocked(PlayerNumber, RoundNumber))
                    rigidBody.MoveRotation(desiredRotation);
            }

            if (!GameMode.GameState.GetPlayerFireLocked(PlayerNumber, RoundNumber) &&
                FiringGun &&
                (FireCallback?.Invoke() ?? true))
                Shoot();

            if (!GameMode.GameState.GetPlayerFireLocked(PlayerNumber, RoundNumber) &&
                UsingEquipment &&
                (EquipmentCallback?.Invoke() ?? true))
                PlaceEquipment();
        }

        if(targetingCursor != null)
            targetingCursor.SetActive(
                !health.Dead &&
                IsVisible &&
                LookDirection.magnitude > 0.1 &&
                !UsingSnapshots
                );
    }

    private void ControlledFrame()
    {
        float horizontalMove = Input.GetAxis("Horizontal" + PlayerNumber);
        float verticalMove = Input.GetAxis("Vertical" + PlayerNumber);

        bool hasHorizontalInput = DeltaExceeds(horizontalMove, 0f, 0.02f);
        bool hasVerticalInput = DeltaExceeds(verticalMove, 0f, 0.02f);
        isIdle = !(hasHorizontalInput || hasVerticalInput);

        Vector3 inputVector = new Vector3(horizontalMove, 0f, verticalMove).normalized;
        inputVector *= moveSpeed;
        velocity += (inputVector * Time.fixedDeltaTime);

        //if (!hasHorizontalInput && velocity.x <= 0.2f) velocity.x = 0f;
        //if (!hasVerticalInput && velocity.y <= 0.2f) velocity.y = 0f;

        //Debug.Log(velocity);

        float horizontalAim = Input.GetAxis("AimHorizontal" + PlayerNumber);
        float verticalAim = Input.GetAxis("AimVertical" + PlayerNumber);

        bool hasHorizontalAim = DeltaExceeds(horizontalAim, 0f, 0.1f);
        bool hasVerticalAim = DeltaExceeds(verticalAim, 0f, 0.1f);

        if ((hasHorizontalAim || hasVerticalAim) && !aimLocked)
        {
            LookDirection = new Vector3(horizontalAim, 0f, verticalAim);
            LookDirection *= lookMagnitude;
        }
        else if (!aimLocked)
            LookDirection = velocity.normalized / 20;

        float fireActivity = Input.GetAxis("Fire" + PlayerNumber);
        float equipmentActivity = Input.GetAxis("Equipment" + PlayerNumber);

        FiringGun = false;
        if (!fired && fireActivity > 0f) //this works becuase of the masssive deadzone setting
        {
            fired = true;
            FiringGun = true;
            Invoke("FiringReset", Weapon.FireRate);
        }

        usedEquipment = false;
        if (!placingEquipment && 
            equipmentActivity > 0f && 
            !GameMode.GameState.GetPlayerFireLocked(PlayerNumber, RoundNumber))
        {
            placingEquipment = true;
            PlacingEquipmentGuide();
        }
        else if (placingEquipment && equipmentActivity == 0f)
        {
            Destroy(shieldPlacer);
            placingEquipment = false;
            usedEquipment = true;
            UsingEquipment = true;
        }
        else if (equipmentActivity == 0f)
            UsingEquipment = false;


        if (Input.GetButtonDown("AimLock" + PlayerNumber))
            aimLocked = !aimLocked;

        if (Input.GetButtonDown("ChangeToPistol" + PlayerNumber) || Input.GetButtonDown("ChangeToSniperOrShotgun" + PlayerNumber))
        {
            if (Input.GetAxis("ChangeToPistol" + PlayerNumber) > 0)
                ChangeWeapon("Pistol");
            
            if (Input.GetAxis("ChangeToSniperOrShotgun" + PlayerNumber) > 0)
                ChangeWeapon("Sniper");
            else if (Input.GetAxis("ChangeToSniperOrShotgun" + PlayerNumber) < 0)
                ChangeWeapon("Shotgun");

            
            //Maybe Change this to keep the targeting Cursors attached to the weapon in question?
            Destroy(targetingCursor);
            loadedCursor = false;
        }
    }

    private void FiringReset() { fired = false; }

    private static bool DeltaExceeds(float value, float target, float delta)
    {
        var actualDifference = Mathf.Abs(value - target);
        return actualDifference > delta;
    }

    private void RecordedFrame()
    {
        position = snapshot.Translation;
        velocity = snapshot.Velocity;
        LookDirection = snapshot.LookDirection;
        FiringGun = snapshot.Firing;
        UsingEquipment = snapshot.UsingEquipment;
        usedEquipment = snapshot.UsedEquipment;
        isIdle = snapshot.IsIdle;
    }

    private void PlacingEquipmentGuide()
    {
        shieldPlacer = Instantiate(equipmentGuide, shieldTransform);
        shieldPlacer.transform.localScale = new Vector3(.1f, .1f, .1f);
        GameMode.ClearOnRoundChange(shieldPlacer);
    }

    private void PlaceEquipment()
    {
        GameObject shieldInstance = Instantiate(equipment, shieldTransform.position, shieldTransform.rotation) as GameObject;
        int destLayer = LayerMask.NameToLayer("ForceField" + PlayerNumber);
        MeshCollider[] colliders = shieldInstance.GetComponentsInChildren<MeshCollider>();
        shieldInstance.layer = destLayer;
        foreach (MeshCollider collider in colliders)
            collider.gameObject.layer = destLayer;
        GameMode.ClearOnRoundChange(shieldInstance.gameObject);

        audioManager.PlayVoice(shieldSound.GetClip());
    }

    private void Shoot()
    {
        GameMode.ClearOnRoundChange(Weapon.Fire(PlayerNumber, playerColor));
        audioManager.PlayVoice(shootSound.GetClip());
    }

    private void ChangeWeapon(string weaponName)
    {
        if (Weapon.WeaponName != weaponName)
        {
            Transform newWeapon = weaponsTransform.Find(weaponName);
            Transform oldWeapon = weaponsTransform.Find(Weapon.WeaponName);

            //Debug.Log("OldWeapon: " + Weapon.WeaponName + ", NewWeapon: " + weaponName);

            oldWeapon.gameObject.SetActive(false);
            newWeapon.gameObject.SetActive(true);

            Weapon = (IWeapon)newWeapon.GetComponentInChildren(System.Type.GetType(weaponName));

            animator.SetInteger("WeaponType", Weapon.WeaponType);
        }
    }

    private void SetLayer(string newLayer)
    {
        if(currentLayer != newLayer)
        {
            gameObject.layer = LayerMask.NameToLayer(newLayer);

            foreach (Transform obj in gameObject.GetComponentsInChildren<Transform>())
                obj.gameObject.layer = LayerMask.NameToLayer(newLayer);

            currentLayer = newLayer;
        }
    }

    public void SetPlayerInformation(int playerNumber, int sourceRoundNum, Vector3 initialPosition, IGameMode gameMode)
    {
        this.PlayerNumber = playerNumber;
        this.RoundNumber = sourceRoundNum;

        Collider[] setColliderTags = GetComponentsInChildren<Collider>();
        foreach (Collider collider in setColliderTags)
            collider.gameObject.tag = "Player" + playerNumber;

        rigidBody.MovePosition(initialPosition);

        this.GameMode = gameMode;

        GetComponentInChildren<TrailRecorder>()?.Setup(this, gameMode);
    }

    public PlayerSnapshot GetSnapshot()
    {
        return new PlayerSnapshot(position, velocity, LookDirection, FiringGun, UsingEquipment, usedEquipment, isIdle);
    }

    public void SetSnapshot(PlayerSnapshot playerSnapshot)
    {
        snapshot = playerSnapshot;
    }

    public void SetUseSnapshots(bool useSnapshots)
    {
        this.UsingSnapshots = useSnapshots;

        SkinnedMeshRenderer secondaryRenderer = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>()[0];
        SkinnedMeshRenderer primaryRenderer = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>()[1];

        if (useSnapshots)
        {
            primaryRenderer.material = ColorManager.Instance.GetPlayerMaterial(PlayerNumber, ColorManager.PlayerColorVarient.MODEL_PRIMARY_INACTIVE);
            secondaryRenderer.material = ColorManager.Instance.GetPlayerMaterial(PlayerNumber, ColorManager.PlayerColorVarient.MODEL_SECONDARY_INACTIVE);
            Destroy(targetingCursor);
        }
        else
        {
            primaryRenderer.material = ColorManager.Instance.GetPlayerMaterial(PlayerNumber, ColorManager.PlayerColorVarient.MODEL_PRIMARY_ACTIVE);
            secondaryRenderer.material = ColorManager.Instance.GetPlayerMaterial(PlayerNumber, ColorManager.PlayerColorVarient.MODEL_SECONDARY_ACTIVE);
        }

        playerColor = primaryRenderer.material.GetColor("_GlowColor");
    }

    public void OnReset()
    {
        health.FullHeal();
        health.ResetStatistics();
        //PlaySpawnParticles();
    }

    public void PlaySpawnParticles()
    {
        GetComponent<ParticleSystem>().Play();
    }
}
