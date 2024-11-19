using System;
using System.Collections.Generic;
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

    [SerializeField]
    private float wtf;

    public RuntimeAnimatorController animationStyle;
    public Transform rightHandGrip;
    public Transform leftHandGrip;

    [Header("Audio")]
    public GameObject audioSpot;
    public AudioSource[] sounds;

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

    private Rigidbody rb;

    private static int id = 0;

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

    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }

    public bool GetLightAttack()
    {
        return lightAttack.triggered;
        // TODO: cooldown linked to speed of animation
        // use coroutine to wait for animation to finish
    }

    public bool GetHeavyAttack()
    {
        // Debug.Log("heavy attack:" + heavyAttack.triggered);
        return heavyAttack.triggered;
    }

    public bool GetSpecialAttack()
    {
        // Debug.Log("heavy attack:" + specialAttack.triggered);
        return specialAttack.triggered;
        // TODO: cooldown linked to speed of animation
        // use coroutine to wait for animation to finish
    }

    public bool GetBlocking()
    {
        // Debug.Log("block:" + block.triggered);
        return block.triggered;
    }

    private void Awake()
    {
        id++;
        PlayerInput input = this.GetComponentInChildren<PlayerInput>();

        user = InputUser.PerformPairingWithDevice(input.devices[0]);
        if (input.devices.Count > 1)
        {
            for (int i = 1; i < input.devices.Count; i++)
            {
                InputUser.PerformPairingWithDevice(input.devices[1], user);
            }
        }
        playerInput = new PlayerInputActions();
        user.AssociateActionsWithUser(playerInput);
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        playerInput.Enable();

        // Movement
        move = playerInput.Player.Move;
        move.performed += OnMove;
        move.canceled += ctx => moveDirection = Vector2.zero;

        // Look
        look = playerInput.Player.Look;
        look.performed += OnLook;
        look.canceled += ctx => lookPosition = Vector2.zero;

        // Fire - placeholder for now
        lightAttack = playerInput.Player.Swing;
        lightAttack.performed += OnSwing;
        // lightAttack.performed += ctx =>
        // {
        //     audioSpot.transform.position = transform.position;
        //     sounds[0].Play();
        // };

        heavyAttack = playerInput.Player.HeavyAttack;
        // heavyAttack.performed += ctx =>
        // {
        //     audioSpot.transform.position = transform.position;
        //     sounds[1].Play();
        // };

        specialAttack = playerInput.Player.WeaponArt;
        // specialAttack.performed += OnSwing;

        // Block - placeholder for now
        block = playerInput.Player.Block;
        block.performed += OnBlock;

        // Sprint
        // Logarithmic decrease in speed, base weight of 1.0 = 1.5x speed
        sprint = playerInput.Player.Sprint;
        sprint.performed += ctx =>
        {
            if (currentStamina > 0)
            {
                moveSpeed *= (float)(1.5f - Math.Log10(weight));
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
            // Debug.LogError("blade is is:" + bladeStats.name);
            // Debug.LogError(" blade weight is:" + bladeStats.WeightValue);
            // Debug.LogError("Found ExampleBlade component. Weight is: " + exampleBlade.WeightValue);
            //weight = exampleBlade.WeightValue; // Store the weight value
        }
        else
        {
            // Debug.LogError("ExampleBlade component not found in children.");
        }
        // Debug.LogError("weapon is is:" + weapon.name);
        // Debug.LogError("weight is:" + weapon.weight);
        // Debug.LogError("DIFF weight is:" + gameObject.GetComponentInChildren<EmptyWeapon>().weight);
        //weaponWeight = weapon.weight;
        // weight = bladeStats.WeightValue;
    }

    public void setGrip()
    {
        animationStyle = weapon.animationStyle;

        if (weapon.handle != null)
        {
            rightHandGrip = weapon.handle.transform.Find("rightHandGrip");
            leftHandGrip = weapon.handle.transform.Find("leftHandGrip");
            if (rightHandGrip != null)
            {
                // Debug.Log("Right hand grip set successfully.");
            }

            if (rightHandGrip != null && leftHandGrip == null)
            {
                GetComponentInChildren<IKConstraintController>().leftHandGrabWeapon.enabled = false;
                // Debug.LogError("one handed weapon");
            }

            if (rightHandGrip != null && leftHandGrip != null)
            {
                // Debug.LogError("grips succeeded");
            }
        }
        else
        {
            // Debug.LogError("Handle is missing from the weapon.");
        }
    }

    private void SelectAndEquipWeapon()
    {
        if (weaponPrefabs.Count == 0)
        {
            // Debug.LogError("No weapon prefabs assigned.");
            return;
        }

        // Randomly select a weapon prefab
        int randomIndex = UnityEngine.Random.Range(0, weaponPrefabs.Count);

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
        // Debug.LogError(bladeStats.WeightValue + " <--- ASSIGNMENT CALL");
        // Debug.LogError(weapon.blade.GetComponent<ExampleBlade>().WeightValue + " <--- DIRECT CALL");
        weight = bladeStats.WeightValue;
        wtf = bladeStats.WeightValue;
        weapon.wielder = this.gameObject;
        //weaponInstance.transform.SetParent(hand);
        //Debug.LogError(weaponInstance.name);
        //weaponInstance.transform.localPosition = new Vector3(-0.004f, -0.0088f, 0.0001f); // Adjust as needed
        //weaponInstance.transform.localRotation = Quaternion.Euler(-19.382f, 10.009f, 88.022f);
        //weaponInstance.transform.localScale = Vector3.one;// Adjust as needed
        //Debug.Log("Weapon instantiated at default position.");
    }

    public void TakeDamage(float damage)
    {
        Debug.Log(this.gameObject.name + "took " + damage + " damage");
        effects.ScreenDamageEffect(damage / currentHealth);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, GameManager.playerHealth);

        if (currentHealth <= 0)
        {
            GameManager.running = false;
            Debug.Log(id + " DIED");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //MovePlayer();
        UpdateMovement();
        PlayerLook();
        UpdateStamina();
        ApplyGravity();
        controller.Move(currentMove * Time.deltaTime);

        weapon.setAttacking(animationController.isAttacking());
        //weight = exampleBlade.WeightValue; // Store the weight value
    }

    private void MovePlayer()
    {
        Vector3 moveVec = new Vector3(moveDirection.x, 0, moveDirection.y);
        //controller.Move(moveSpeed * Time.deltaTime * transform.TransformDirection(moveVec));

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
                staminaDecay * (1.0f + 0.5f * (float)Math.Pow(weight - 1, 2)) * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            if (currentStamina <= 0)
            {
                moveSpeed = originalSpeed;
                isSprinting = false;
            }
        }

        // Debug.Log(currentStamina);
    }

    private void PlayerLook()
    {
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
            // TakeDamage(10);
            weapon.Swing();
        }
        else
        {
            Debug.Log("No weapon equipped");
        }
    }

    private void OnDisconnect(InputAction.CallbackContext ctx)
    {
        user.UnpairDevices();
        playerInput.Dispose();
        Destroy(this.gameObject);
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookPosition = ctx.ReadValue<Vector2>();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        // Debug.Log("moved");
        moveDirection = ctx.ReadValue<Vector2>();
    }

    private void OnBlock(InputAction.CallbackContext ctx)
    {
        Debug.Log("blocked");
    }

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
}
