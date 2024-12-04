using System.Collections.Generic;
using System.Linq.Expressions;
using Ami.BroAudio;
using Ami.BroAudio.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float maxStamina = 100.0f;
    public float staminaDecay = 20.0f;
    public float staminaRegen = 10.0f;
    public float acceleration = 10.0f;
    private Vector3 velocity;
    public float deceleration = 5.0f;
    public float gravity = -9.81f;

    [SerializeField]
    private float Speedometer = 0;

    [Header("Sensitivity")]
    public float xSens = 30.0f;
    public float ySens = 30.0f;

    [Header("Physics")]
    public float weight = 1.0f;

    [Header("Weapon List")]
    [SerializeField]
    private List<GameObject> weaponPrefabs;

    [Header("Weapon")]
    [SerializeField]
    private EmptyWeapon weapon;

    [SerializeField]
    private ExampleBlade bladeStats;

    public RuntimeAnimatorController animationStyle;
    public Transform rightHandGrip;
    public Transform leftHandGrip;

    [Header("Audio")]
    public SoundID lightSound = default;
    public SoundID heavySound = default;
    public SoundID[] footsteps = default;
    public SoundID[] sprintingFootsteps = default;
    public SoundID deathSound = default;
    public SoundID swingSound = default;

    [Header("RightHand")]
    public Transform hand;

    private float currentHealth;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    private PlayerInputActions playerInput;
    private TwoDimensionalAnimationStateController animationController;
    private InputUser user;
    private InputAction move;
    private InputAction look;
    private InputAction lightAttack;
    private InputAction block;
    private InputAction sprint;
    private InputAction heavyAttack;
    private InputAction specialAttack;
    private Effects effects;

    private Vector3 currentMove = Vector3.zero;
    private Vector2 moveDirection = Vector2.zero;
    private Vector2 lookPosition = Vector2.zero;
    private float currentSpeed = 0f;
    private CharacterController controller;
    private float originalSpeed;
    private bool isSprinting = false;

    [SerializeField]
    private float currentStamina;
    private new Camera camera;
    private GameObject spawnPoint;

    private Rigidbody rb;

    private static int totalPlayers = 0;
    private int id;

    private bool footstepsPlaying = false;
    private int footstepId = 0;
    private int sprintFootstepId = 0;
    private bool swingSoundPlaying = false;

    private Transform zeroPosition;


    public Transform headBone; // Drag the head bone here in the inspector.
    public float snapBackSpeed = 5f; // Adjust for smoothness.
    public Vector3 hitRotation = new Vector3(-30, 0, 0); // Example rotation when hit.

    private Quaternion originalRotation;
    private bool isHit = false;

    public enum actionEnum
    {
        LightSwing,
        HeavySwing,
        SpecialAttack,
        Block
    }

    public bool GetSprinting()
    {
        return isSprinting;
    }

    public int GetId()
    {
        return id;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }

    public bool GetLightAttack()
    {
        return lightAttack.triggered;
    }

    public bool GetHeavyAttack()
    {
        return heavyAttack.triggered;
    }

    public bool GetSpecialAttack()
    {
        return specialAttack.triggered;
    }

    public bool GetBlocking()
    {
        return block.triggered;
    }

    public EmptyWeapon GetWeapon()
    {
        return weapon;
    }

    public void Disconnect()
    {
        user.UnpairDevices();
        playerInput.Dispose();
    }

    public void OnDeath()
    {
        IAudioPlayer deathPlayer = BroAudio.Play(deathSound);
        totalPlayers = 0;

        deathAnimation();
    }

    public void Reset()
    {
        transform.position = Vector3.zero;
        currentHealth = GameManager.playerHealth;
    }

    private void Awake()
    {
        totalPlayers++;
        id = totalPlayers;

        Debug.Log("id: " + id);

        // Set spawn point and have players look at each other
        spawnPoint =
            id == 1
                ? GameObject.FindGameObjectWithTag("Player 1 Spawn")
                : GameObject.FindGameObjectWithTag("Player 2 Spawn");
        zeroPosition = GameObject.FindGameObjectWithTag("Zero").transform;
        this.transform.position = spawnPoint.transform.position + new Vector3(0, 3, 0);

        PlayerInput input = this.GetComponentInChildren<PlayerInput>();
        user = InputUser.PerformPairingWithDevice(input.devices[0]);

        playerInput = new PlayerInputActions();
        user.AssociateActionsWithUser(playerInput);
        rb = GetComponent<Rigidbody>();

        currentHealth = GameManager.playerHealth;
        PlayerManager.Register(this);
    }

    private void OnEnable()
    {
        playerInput.Enable();

        move = playerInput.Player.Move;
        move.performed += OnMove;
        move.canceled += ctx => moveDirection = Vector2.zero;

        look = playerInput.Player.Look;
        look.performed += OnLook;
        look.canceled += ctx => lookPosition = Vector2.zero;

        lightAttack = playerInput.Player.Swing;
        lightAttack.performed += OnSwing;

        heavyAttack = playerInput.Player.HeavyAttack;

        specialAttack = playerInput.Player.WeaponArt;

        block = playerInput.Player.Block;
        block.performed += OnBlock;

        // Sprint
        // Logarithmic decrease in speed, base weight of 1.0 = 1.5x speed
        sprint = playerInput.Player.Sprint;
        sprint.performed += ctx =>
        {
            if (currentStamina > 0)
            {
                moveSpeed *= (float)(1.5f - Mathf.Log(weight));
                isSprinting = true;
            }
        };
        sprint.canceled += ctx =>
        {
            moveSpeed = originalSpeed;
            isSprinting = false;
        };
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {

        if (headBone != null)
        {
            originalRotation = headBone.localRotation;
        }

        controller = this.GetComponent<CharacterController>();
        camera = this.GetComponentInChildren<Camera>();
        animationController = this.GetComponentInChildren<TwoDimensionalAnimationStateController>();
        effects = this.GetComponent<Effects>();

        originalSpeed = moveSpeed;
        currentStamina = maxStamina;
        currentHealth = GameManager.playerHealth;

        SelectAndEquipWeapon();
        setGrip();
        ExampleBlade exampleBlade = GetComponentInChildren<ExampleBlade>();
        if (exampleBlade != null)
        {
            weight = exampleBlade.WeightValue; // Store the weight value
        }
    }

    public void setGrip()
    {
        animationStyle = weapon.animationStyle;

        if (weapon.handle != null)
        {
            rightHandGrip = weapon.handle.transform.Find("rightHandGrip");
            leftHandGrip = weapon.handle.transform.Find("leftHandGrip");

            if (rightHandGrip != null && leftHandGrip == null)
            {
                GetComponentInChildren<IKConstraintController>().leftHandGrabWeapon.enabled = false;
            }
        }
        else
        {
            Debug.LogError("Handle is missing from the weapon.");
        }
    }

    private void SelectAndEquipWeapon()
    {
        if (weaponPrefabs.Count == 0)
        {
            return;
        }

        // Randomly select a weapon prefab
        int randomIndex = Random.Range(0, weaponPrefabs.Count);

        Vector3 spawnPosition = new Vector3(
            transform.position.x,
            transform.position.y + 1.231f,
            transform.position.y
        );
        Quaternion spawnRotation = Quaternion.Euler(90f, 0f, 0f);

        // Instantiate the selected weapon at the calculated position in front of the current GameObject
        GameObject weaponInstance = Instantiate(
            weaponPrefabs[randomIndex],
            spawnPosition,
            spawnRotation
        );

        // Get the Weapon component
        weapon = weaponInstance.GetComponent<EmptyWeapon>();
        bladeStats = weapon.blade.GetComponent<ExampleBlade>();
        weight = bladeStats.WeightValue;
        weapon.wielder = this.gameObject;
    }

    public void TakeDamage(float damage, AttackType type)
    {
        switch (type)
        {
            case AttackType.LIGHT:
                BroAudio.Play(lightSound);
                break;
            case AttackType.HEAVY:
                BroAudio.Play(heavySound);
                break;
        }

        effects.ScreenDamageEffect(damage / currentHealth);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, GameManager.playerHealth);

        if (currentHealth <= 0)
        {
            // GameManager.running = false;
        }

        if (headBone != null)
        {
            // Apply the hit rotation
            Debug.Log("this is running" + headBone.rotation);
            headBone.localRotation = Quaternion.Euler(hitRotation);
            Debug.Log("afterHit" + headBone.rotation);
            isHit = true;
        }
}

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.running)
        {
            playerInput.Disable();
            return;
        }
        else
        {
            playerInput.Enable();
        }

        if (move.inProgress)
        {
            if (!isSprinting)
            {
                if (!footstepsPlaying)
                {
                    footstepsPlaying = true;
                    IAudioPlayer footstepsPlayer = BroAudio.Play(footsteps[footstepId]);
                    footstepId = (footstepId + 1) % footsteps.Length;
                    footstepsPlayer.OnEnd(soundId => footstepsPlaying = false);
                }
            }
            else
            {
                if (!footstepsPlaying)
                {
                    footstepsPlaying = true;
                    IAudioPlayer footstepsPlayer = BroAudio.Play(
                        sprintingFootsteps[sprintFootstepId]
                    );
                    sprintFootstepId = (sprintFootstepId + 1) % sprintingFootsteps.Length;
                    footstepsPlayer.OnEnd(soundId => footstepsPlaying = false);
                }
            }
        }

        if (animationController.isAttacking() && !swingSoundPlaying)
        {
            swingSoundPlaying = true;
            IAudioPlayer audioPlayer = BroAudio.Play(swingSound);
        }
        else if (!animationController.isAttacking())
        {
            swingSoundPlaying = false;
        }


        if (isHit)
        {
            // Gradually return to the original rotation
            headBone.localRotation = Quaternion.Lerp(headBone.localRotation, originalRotation, Time.deltaTime * snapBackSpeed);

            // Stop the motion once it's back to the original position
            if (Quaternion.Angle(headBone.localRotation, originalRotation) < 0.1f)
            {
                isHit = false;
            }
        }

        UpdateMovement();
        PlayerLook();
        UpdateStamina();
        ApplyGravity();
        controller.Move(currentMove * Time.deltaTime);
    }

    private void MovePlayer()
    {
        Vector3 moveVec = new Vector3(moveDirection.x, 0, moveDirection.y);

        // Transform the movement vector to be relative to the player's current rotation
        Vector3 worldMove = transform.TransformDirection(moveVec);

        // Apply movement speed and keep the current vertical velocity
        Vector3 velocity = worldMove * moveSpeed;
        velocity.y = rb.velocity.y; // Retain the vertical velocity (for jumping or falling)

        //Set the Rigidbody's velocity
        rb.velocity = velocity;
    }

    private void UpdateStamina()
    {
        if (!isSprinting)
        {
            currentStamina += staminaRegen * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
        else
        {
            // Exponential stamina decay - should probably change to be less punishing
            currentStamina -=
                staminaDecay * (1.0f + 0.5f * (float)Mathf.Pow(weight - 1, 2)) * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            if (currentStamina <= 0)
            {
                moveSpeed = originalSpeed;
                isSprinting = false;
            }
        }
    }

    private void PlayerLook()
    {
        if (lookPosition == Vector2.zero)
        {
            return;
        }

        xRotation -= lookPosition.y * Time.deltaTime * xSens;
        xRotation = Mathf.Clamp(xRotation, -80.0f, 80.0f);
        yRotation += lookPosition.x * Time.deltaTime * ySens;

        camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void OnSwing(InputAction.CallbackContext ctx)
    {
        if (weapon)
        {
            weapon.Swing();
        }
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookPosition = ctx.ReadValue<Vector2>();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveDirection = ctx.ReadValue<Vector2>();
    }

    private void OnBlock(InputAction.CallbackContext ctx) { }

    private void UpdateMovement()
    {
        // Calculate target movement direction
        Vector3 targetMove = new Vector3(moveDirection.x, 0, moveDirection.y);
        targetMove = transform.TransformDirection(targetMove); // Make movement relative to player rotation

        // Adjust speed based on acceleration or deceleration
        float targetSpeed = isSprinting ? moveSpeed * 1.5f : moveSpeed;

        // Cap the speed to the maximum allowed speed
        float maxSpeed = isSprinting ? moveSpeed * 1.5f : moveSpeed;

        // Update current speed with acceleration or deceleration
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed * targetMove.magnitude,
            (targetMove.magnitude > 0 ? acceleration : deceleration) * Time.deltaTime
        );

        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed); // Ensure speed doesn't exceed the cap

        // Calculate final movement vector
        currentMove = targetMove * currentSpeed;
        Speedometer = currentSpeed;
        currentMove.y = velocity.y; // Preserve vertical velocity for gravity
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            velocity.y = -2f;
            // Small downward force to keep grounded
        }
        else
        {
            velocity.y += gravity * Time.deltaTime; // Apply gravity
        }
    }

    private void deathAnimation()
    {
        animationController.death();
    }
}
