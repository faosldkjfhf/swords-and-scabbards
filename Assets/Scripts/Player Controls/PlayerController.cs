using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float xSens = 30.0f;
    public float ySens = 30.0f;
    private float xRotation = 0.0f;

    private PlayerInputActions playerInput;
    private InputUser user;
    private InputAction move;
    private InputAction look;
    private InputAction swing;
    private InputAction block;

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 lookPosition = Vector2.zero;
    private CharacterController controller;

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
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(camera.transform.position, camera.transform.forward * 100, Color.red);
        MovePlayer();
        PlayerLook();
    }

    private void MovePlayer()
    {
        Vector3 moveVec = new Vector3(moveDirection.x, 0, moveDirection.y);
        controller.Move(moveSpeed * Time.deltaTime * transform.TransformDirection(moveVec));
    }

    private void PlayerLook()
    {
        xRotation -= lookPosition.y * Time.deltaTime * ySens;
        xRotation = Mathf.Clamp(xRotation, -80.0f, 80.0f);

        camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(lookPosition.x * Time.deltaTime * xSens * Vector3.up);
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
