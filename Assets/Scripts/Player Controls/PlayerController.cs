using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float maxStamina = 100.0f;
    public float staminaDecay = 20.0f;
    public float staminaRegen = 10.0f;

    [Header("Sensitivity")]
    public float xSens = 30.0f;
    public float ySens = 30.0f;

    [Header("Physics")]
    public double weight = 1.0f;

    [Header("Weapon List")]
    [SerializeField]
    private List<GameObject> weaponPrefabs;

    [Header("Weapon")]
    [SerializeField]
    private EmptyWeapon weapon;
    public RuntimeAnimatorController animationStyle;
    public Transform rightHandGrip;
    public Transform leftHandGrip;

  


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

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 lookPosition = Vector2.zero;
    private CharacterController controller;
    private float originalSpeed;
    private bool isSprinting = false;
    private float currentStamina;
    private new Camera camera;



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

        heavyAttack = playerInput.Player.HeavyAttack;
        heavyAttack.performed += OnSwing;

        specialAttack = playerInput.Player.WeaponArt;
        specialAttack.performed += OnSwing;

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
        weapon = this.GetComponentInChildren<EmptyWeapon>();
        camera = this.GetComponentInChildren<Camera>();
        animationController = this.GetComponentInChildren<TwoDimensionalAnimationStateController>();

        originalSpeed = moveSpeed;
        currentStamina = maxStamina;
        currentHealth = GameManager.playerHealth;

        SelectAndEquipWeapon();
        setGrip();
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
                Debug.Log("Right hand grip set successfully.");
            }

            if (rightHandGrip != null && leftHandGrip == null)
            {
                GetComponentInChildren<IKConstraintController>().leftHandGrabWeapon.enabled = false;
                Debug.LogError("one handed weapon");
            }

            if (rightHandGrip != null && leftHandGrip != null)
            {
                Debug.LogError("grips succeeded");
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
            Debug.LogError("No weapon prefabs assigned.");
            return;
        }

        // Randomly select a weapon prefab
        int randomIndex = UnityEngine.Random.Range(0, weaponPrefabs.Count);

        Vector3 spawnPosition = transform.position + transform.forward * 0.5f;

        // Instantiate the selected weapon at the calculated position in front of the current GameObject
        GameObject weaponInstance = Instantiate(weaponPrefabs[randomIndex], spawnPosition, Quaternion.identity);

        // Get the Weapon component
        weapon = weaponInstance.GetComponent<EmptyWeapon>();
        weapon.wielder = this.gameObject;
        Debug.LogError("PLEASDASD");
        weaponInstance.transform.SetParent(hand);
        Debug.LogError(weaponInstance.name);
        weaponInstance.transform.localPosition = new Vector3(-0.004f, -0.0088f, 0.0001f); // Adjust as needed
        weaponInstance.transform.localRotation = Quaternion.Euler(-19.382f, 10.009f, 88.022f);
        weaponInstance.transform.localScale = Vector3.one;// Adjust as needed
        Debug.Log("Weapon instantiated at default position.");

    

    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, GameManager.playerHealth);

        if (currentHealth <= 0)
        {
            GameManager.running = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        PlayerLook();
        UpdateStamina();

        weapon.setAttacking(animationController.isAttacking());
    }

    private void MovePlayer()
    {
        Vector3 moveVec = new Vector3(moveDirection.x, 0, moveDirection.y);
        controller.Move(moveSpeed * Time.deltaTime * transform.TransformDirection(moveVec));
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

        Debug.Log(currentStamina);
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
        Debug.Log("moved");
        moveDirection = ctx.ReadValue<Vector2>();
    }

    private void OnBlock(InputAction.CallbackContext ctx)
    {
        Debug.Log("blocked");
    }
}
