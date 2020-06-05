using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IRecordable
{
    #region delegates
    public delegate bool FireEventCallback(int costToFire);
    public FireEventCallback FireCallback { private get; set; } = null;
    public delegate bool EquipmentEventCallback();
    public EquipmentEventCallback EquipmentCallback { private get; set; } = null;
    #endregion

    #region equipment
    private bool changingGun = false;
    private string newWeapon = "Pistol";
    public int FiringGun { get; private set; } = 0;
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
    [SerializeField] private GameObject equipmentIcon;

    [SerializeField] private float lookOffset;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float accelSpeed;
    [SerializeField] private float deaccelSpeed;
    [SerializeField] private float maxSpeed;

    [SerializeField] private Vector3 cameraHeight = new Vector3(0, 10, 0);
    [SerializeField] private int lookSnap = 5;
    [SerializeField] private float lookMagnitude = 16; //how far they can look in splitscreen
#pragma warning restore IDE0044
    #endregion

    #region Components
    public GameObject EquipmentIconPrefab => equipmentIcon;
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
        get => GameMode.GameState.GetPlayerVisible(PlayerNumber, RoundNumber) && !health.Dead;
        set {
            if (value) playerModel.transform.localScale = new Vector3(1, 1, 1);
            else playerModel.transform.localScale = new Vector3(0, 0, 0);
        }
    }
    #endregion

    #region movement
    private bool isIdle = true;
    private Vector3 position, velocity, moveDirection;
    public Vector3 LookDirection { get; set; }
    private Vector3 lastLookDirection = new Vector3(1,0,0);
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

        SetTag(transform, "Player" + PlayerNumber);

        Weapon = GetComponentsInChildren<IWeapon>()[0];
        animator.SetInteger("WeaponType", Weapon.WeaponType);
    }

    private void FixedUpdate()
    {
        IsVisible = GameMode.GameState.GetPlayerVisible(PlayerNumber, RoundNumber) && !health.Dead;

        if (!health.Dead && IsVisible)
        {
            if (!loadedCursor && !UsingSnapshots)
            {
                loadedCursor = true;
                targetingCursor = Instantiate(Weapon.TargetingCursor);
                targetingCursor.GetComponent<PlayerCursor>().Player = this;
            }

            //read in the calculated values from the rigidbody
            //velocity = rigidBody.velocity;
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
            
            bool isLooking = LookDirection.magnitude > 0.1;
            bool isMoving = velocity.magnitude > 0.2;

            if (lastLookDirection == Vector3.zero)
                lastLookDirection = rigidBody.rotation.eulerAngles;

            if (isLooking) {
                lastLookDirection = new Vector3(LookDirection.x, LookDirection.y, LookDirection.z);
                LookTo(LookDirection);
            }
            else if (moveDirection != Vector3.zero) {
                lastLookDirection = new Vector3(moveDirection.x, moveDirection.y, moveDirection.z);
                LookTo(moveDirection);
            }
            else if(lastLookDirection != Vector3.zero)
            {
                LookTo(new Vector3(lastLookDirection.x, lastLookDirection.y, lastLookDirection.z));
            }

            if (!GameMode.GameState.GetPlayerFireLocked(PlayerNumber, RoundNumber) && changingGun)
                ChangeWeapon(newWeapon);

            if (!GameMode.GameState.GetPlayerFireLocked(PlayerNumber, RoundNumber) &&
                FiringGun != 0 &&
                (FireCallback?.Invoke(Weapon.CostToFire) ?? true))
                Shoot();
            else FiringGun = 0;

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

    private void LookTo(Vector3 direction)
    {
        Vector3 moddedDirection = direction;

        float lookAngle = Vector3.Angle(Vector3.forward, moddedDirection) % 45;
        if (lookAngle > lookSnap) lookAngle -= 45;

        if (Mathf.Abs(lookAngle) <= lookSnap)
            moddedDirection = Quaternion.AngleAxis(lookAngle, Vector3.up) * moddedDirection;

        desiredRotation = Quaternion.LookRotation(moddedDirection, Vector3.up);
        rigidBody.MoveRotation(desiredRotation);
    }

    private void ControlledFrame()
    {
        float horizontalMove = Input.GetAxis("Horizontal" + PlayerNumber);
        float verticalMove = Input.GetAxis("Vertical" + PlayerNumber);

        bool hasHorizontalInput = DeltaExceeds(horizontalMove, 0f, 0.02f);
        bool hasVerticalInput = DeltaExceeds(verticalMove, 0f, 0.02f);
        isIdle = !(hasHorizontalInput || hasVerticalInput);

        Vector3 inputVector = new Vector3(horizontalMove, 0f, verticalMove);

        if (!isIdle)
            inputVector = inputVector.normalized * Mathf.Lerp(velocity.magnitude, maxSpeed, accelSpeed);
        else if (inputVector != Vector3.zero)
            inputVector = velocity.normalized * Mathf.Lerp(velocity.magnitude, 0, accelSpeed * 10);

        if (isIdle)
            moveDirection = Vector3.zero;
        else
            moveDirection = inputVector;

        velocity = inputVector;

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
            LookDirection = transform.forward / 20;

        float fireActivity = Input.GetAxis("Fire" + PlayerNumber);
        float equipmentActivity = Input.GetAxis("Equipment" + PlayerNumber);

        FiringGun = 0;
        if (!fired && fireActivity > 0) //this works becuase of the masssive deadzone setting
        {
            fired = true;
            FiringGun = Weapon.CostToFire;
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

        changingGun = false;
        if (Input.GetAxis("ChangeToPistol" + PlayerNumber) > 0 || Input.GetAxis("ChangeToSniperOrShotgun" + PlayerNumber) != 0)
        {
            changingGun = true;

            if (Input.GetAxis("ChangeToPistol" + PlayerNumber) > 0)
                newWeapon = "Pistol";
            
            if (Input.GetAxis("ChangeToSniperOrShotgun" + PlayerNumber) > 0)
                newWeapon = "Sniper";
            else if (Input.GetAxis("ChangeToSniperOrShotgun" + PlayerNumber) < 0)
                newWeapon = "Shotgun";
            
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
        moveDirection = snapshot.MoveDirection;
        LookDirection = snapshot.LookDirection;
        changingGun = snapshot.Changing;
        newWeapon = snapshot.WeaponName;
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

            oldWeapon.gameObject.SetActive(false);
            newWeapon.gameObject.SetActive(true);
            newWeapon.tag = "Player" + PlayerNumber;

            Weapon = (IWeapon)newWeapon.GetComponentInChildren(System.Type.GetType(weaponName));

            lookMagnitude = Weapon.LookMagnitude;
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

    private void SetTag(Transform root, string tag)
    {
        root.tag = tag;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            child.tag = tag;

            if (child.childCount != 0)
                SetTag(child, tag);
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
        return new PlayerSnapshot(position, velocity, LookDirection, moveDirection, changingGun, newWeapon, FiringGun, UsingEquipment, usedEquipment, isIdle);
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
        ChangeWeapon("Pistol");
    }
}
