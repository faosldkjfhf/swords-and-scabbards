using System;
using Unity.VisualScripting;
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

    [Header("Sensitivity")]
    public float xSens = 30.0f;
    public float ySens = 30.0f;

    [Header("Physics")]
    public double weight = 1.0f;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    private PlayerInputActions playerInput;
    private InputUser user;
    private InputAction move;
    private InputAction look;
    private InputAction swing;
    private InputAction block;
    private InputAction sprint;

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 lookPosition = Vector2.zero;
    private CharacterController controller;
    private float originalSpeed;
    private bool isSprinting = false;
    private float currentStamina;

    private Weapon weapon;
    private new Camera camera;

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
        swing = playerInput.Player.Swing;
        swing.performed += OnSwing;

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
        weapon = this.GetComponentInChildren<Weapon>();
        camera = this.GetComponentInChildren<Camera>();

        originalSpeed = moveSpeed;
        currentStamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.DrawRay(camera.transform.position, camera.transform.forward * 100, Color.red);
        MovePlayer();
        PlayerLook();
        UpdateStamina();
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
