using System.Collections;
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
    [SerializeField] private Transform shieldTransform;
    [SerializeField] private Transform weaponsTransform;

    [SerializeField] private GameObject targetingCursor;
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
    private int PlayerNumber { get; set; } = 0;
    public IWeapon Weapon { get; private set; }
    private int sourceRoundNum = 0;
    private string ghostLayerName = "Ghost";
    private string PlayerLayerName { get => "Player" + PlayerNumber; }
    private string currentLayer = "Unset";
    private bool usingSnapshots = false;
    private bool aimLocked = false;
    private bool loadedCursor = false;
    private Animator animator;
    private Rigidbody rigidBody;
    private PlayerHealth health;
    private Color playerColor;
    private AudioManager audioManager;
    private RandomShoot shootSound;
    private RandomShield shieldSound;
    public bool IsVisible {
        get => gameMode.GameState.GetPlayerVisible(PlayerNumber, sourceRoundNum);
        set {
            if (value) gameObject.transform.localScale = new Vector3(2, 2, 2);
            else gameObject.transform.localScale = new Vector3(0, 0, 0);
        }
    }
    #endregion

    #region movement
    private bool isIdle = true;
    private int frameCounter = 0;
    private Vector3 position, velocity;
    public Vector3 LookDirection { get; set; }
    private Quaternion desiredRotation = Quaternion.identity;
    private PlayerSnapshot snapshot;
    private IGameMode gameMode;
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
        Weapon = GetComponentsInChildren<IWeapon>()[0];
        animator.SetInteger("WeaponType", Weapon.WeaponType);
    }

    private void FixedUpdate()
    {
        if (!health.Dead && IsVisible)
        {
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
                targetingCursor.GetComponent<PlayerCursor>().Player = this;
            }

            //read in the calculated values from the rigidbody
            velocity = rigidBody.velocity;
            position = rigidBody.position; // seperate method?
            //end

            SetLayer(gameMode.GameState.GetPlayerCanTakeDamage(PlayerNumber, sourceRoundNum) ? PlayerLayerName : ghostLayerName);

            if (usingSnapshots)
                RecordedFrame();
            else
            {
                ControlledFrame();
                if (gameMode.GameState.GetPlayerPositionsLocked(PlayerNumber, sourceRoundNum))
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
            bool isLooking = LookDirection.magnitude > 0.1;
            bool isMoving = velocity.magnitude > 0.2;

            if (isLooking) {
                Vector3 moddedDirection = Quaternion.AngleAxis(lookOffset, Vector3.up) * LookDirection;

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
                if (!gameMode.GameState.GetPlayerLookLocked(PlayerNumber, sourceRoundNum))
                    rigidBody.MoveRotation(desiredRotation);
            }

            if (!gameMode.GameState.GetPlayerFireLocked(PlayerNumber, sourceRoundNum) &&
                firingGun &&
                (FireCallback?.Invoke() ?? true))
                Shoot();

            if (!gameMode.GameState.GetPlayerFireLocked(PlayerNumber, sourceRoundNum) &&
                usedEquipment &&
                !usingEquipment &&
                (EquipmentCallback?.Invoke() ?? true))
                PlaceEquipment();
        }

        IsVisible = IsVisible;

        if(targetingCursor != null)
            targetingCursor.SetActive(
                !health.Dead &&
                IsVisible &&
                LookDirection.magnitude > 0 &&
                !usingSnapshots
                );
    }

    private void ControlledFrame()
    {
        float horizontalMove = Input.GetAxis("Horizontal" + PlayerNumber);
        float verticalMove = Input.GetAxis("Vertical" + PlayerNumber);

        bool hasHorizontalInput = DeltaExceeds(horizontalMove, 0f, 0.02f);
        bool hasVerticalInput = DeltaExceeds(verticalMove, 0f, 0.02f);
        isIdle = !(hasHorizontalInput || hasVerticalInput);

        Vector3 inputVector = new Vector3(horizontalMove, 0f, verticalMove);
        inputVector *= moveSpeed;
        velocity += (inputVector * Time.fixedDeltaTime);

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
            LookDirection = Vector3.zero;

        float fireActivity = Input.GetAxis("Fire" + PlayerNumber);
        float equipmentActivity = Input.GetAxis("Equipment" + PlayerNumber);

        firingGun = false;
        if (!fired && fireActivity > 0f) //this works becuase of the masssive deadzone setting
        {
            fired = true;
            firingGun = true;
            Invoke("FiringReset", Weapon.FireRate);
        }

        usedEquipment = false;
        if (!usingEquipment && 
            equipmentActivity > 0f && 
            !gameMode.GameState.GetPlayerFireLocked(PlayerNumber, sourceRoundNum) && 
            gameMode.GetPlayerManager(PlayerNumber).GetAvailableEquipment(sourceRoundNum) != 0)
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


        if (Input.GetButtonDown("AimLock" + PlayerNumber))
            aimLocked = !aimLocked;

        if (Input.GetButtonDown("ChangeToPistol" + PlayerNumber))
        {
            if (Input.GetAxis("ChangeToPistol" + PlayerNumber) > 0)
                ChangeWeapon("Pistol");
        }
        else if (Input.GetButtonDown("ChangeToSniperOrShotgun" + PlayerNumber))
        {
            if (Input.GetAxis("ChangeToSniperOrShotgun" + PlayerNumber) > 0)
                ChangeWeapon("Sniper");
            else if (Input.GetAxis("ChangeToSniperOrShotgun" + PlayerNumber) < 0)
                ChangeWeapon("Shotgun");
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
        firingGun = snapshot.Firing;
        usingEquipment = snapshot.UsingEquipment;
        usedEquipment = snapshot.UsedEquipment;
        isIdle = snapshot.IsIdle;
    }

    private void PlacingEquipmentGuide()
    {
        shieldPlacer = Instantiate(equipmentGuide, shieldTransform);
        shieldPlacer.transform.localScale = new Vector3(.1f, .1f, .1f);
        roundClearingList.Add(shieldPlacer);
    }

    private void PlaceEquipment()
    {
        GameObject shieldInstance = Instantiate(equipment, shieldTransform.position, shieldTransform.rotation) as GameObject;
        int destLayer = LayerMask.NameToLayer("ForceField" + PlayerNumber);
        MeshCollider[] colliders = shieldInstance.GetComponentsInChildren<MeshCollider>();
        shieldInstance.layer = destLayer;
        foreach (MeshCollider collider in colliders)
            collider.gameObject.layer = destLayer;
        roundClearingList.Add(shieldInstance.gameObject);

        audioManager.PlayVoice(shieldSound.GetClip());
    }

    private void Shoot()
    {
        roundClearingList.AddRange(Weapon.Fire(PlayerNumber, playerColor));
        audioManager.PlayVoice(shootSound.GetClip());
    }

    private void ChangeWeapon(string weaponName)
    {
        Transform newWeapon = weaponsTransform.Find(weaponName + "Transform");

        weaponsTransform.Find(Weapon.WeaponName + "Transform").gameObject.SetActive(false);
        newWeapon.gameObject.SetActive(true);
        Weapon = (IWeapon) newWeapon.GetComponentInChildren(System.Type.GetType(weaponName));

        animator.SetInteger("WeaponType", Weapon.WeaponType);
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

    private void DestroyAllPlayerCreations()
    {
        for (int i = 0; i < roundClearingList.Count; i++)
            Destroy(roundClearingList[i]);
        roundClearingList.Clear();
    }

    public void SetPlayerInformation(int playerNumber, int sourceRoundNum, Vector3 initialPosition, IGameMode gameMode)
    {
        this.PlayerNumber = playerNumber;
        this.sourceRoundNum = sourceRoundNum;

        Collider[] setColliderTags = GetComponentsInChildren<Collider>();
        foreach (Collider collider in setColliderTags)
            collider.gameObject.tag = "Player" + playerNumber;

        rigidBody.MovePosition(initialPosition);

        this.gameMode = gameMode;
    }

    public PlayerSnapshot GetSnapshot()
    {
        return new PlayerSnapshot(position, velocity, LookDirection, firingGun, usingEquipment, usedEquipment, isIdle);
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
            primaryRenderer.material = Resources.Load<Material>("Materials/Player/Player" + PlayerNumber + "Primary");
            secondaryRenderer.material = Resources.Load<Material>("Materials/Player/Player" + PlayerNumber + "Secondary");
            Destroy(targetingCursor);
        }
        else
        {
            primaryRenderer.material = Resources.Load<Material>("Materials/Player/Player" + PlayerNumber + "PrimaryGhost");
            secondaryRenderer.material = Resources.Load<Material>("Materials/Player/Player" + PlayerNumber + "SecondaryGhost");
        }

        playerColor = primaryRenderer.material.GetColor("_GlowColor");
    }

    public void OnReset()
    {
        DestroyAllPlayerCreations();
        health.FullHeal();
        health.ResetStatistics();
    }
    
    public void AddToRoundClearingList(GameObject obj)
    {
        roundClearingList.Add(obj);
    }

    public void OnDestroy() => DestroyAllPlayerCreations();
}
