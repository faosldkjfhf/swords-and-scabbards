using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float xSens = 10.0f;
    public float ySens = 10.0f;
    private float xRotation = 0.0f;

    private PlayerInputActions playerControls;
    private InputAction move;
    private InputAction look;
    private InputAction swing;
    private InputAction block;

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 lookPosition = Vector2.zero;
    private CharacterController controller;

    private Weapon weapon;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        // Movement
        move = playerControls.Player.Move;
        move.Enable();
        move.performed += OnMove;
        move.canceled += ctx => moveDirection = Vector2.zero;

        // Look
        look = playerControls.Player.Look;
        look.Enable();
        look.performed += OnLook;
        look.canceled += ctx => lookPosition = Vector2.zero;

        // Fire - placeholder for now
        swing = playerControls.Player.Swing;
        swing.Enable();
        swing.performed += OnSwing;

        // Block - placeholder for now
        block = playerControls.Player.Block;
        block.Enable();
        block.performed += OnBlock;
    }

    private void OnDisable()
    {
        move.Disable();
        look.Disable();
        swing.Disable();
        block.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        weapon = GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        PlayerLook();
    }

    private void MovePlayer()
    {
        Vector3 move = new Vector3(moveDirection.x, 0, moveDirection.y);
        controller.Move(moveSpeed * Time.deltaTime * transform.TransformDirection(move));
    }

    private void PlayerLook()
    {
        xRotation -= lookPosition.y * Time.deltaTime * ySens;
        xRotation = Mathf.Clamp(xRotation, -80.0f, 80.0f);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
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

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookPosition = ctx.ReadValue<Vector2>();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveDirection = ctx.ReadValue<Vector2>();
    }

    private void OnBlock(InputAction.CallbackContext ctx)
    {
        Debug.Log("blocked");
    }
}
