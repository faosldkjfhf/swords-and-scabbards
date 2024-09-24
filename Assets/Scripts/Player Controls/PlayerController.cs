using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float xSens = 10.0f;
    public float ySens = 10.0f;

    private PlayerInputActions playerControls;
    private InputAction move;
    private InputAction look;
    private InputAction fire;

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 lookPosition = Vector2.zero;
    private CharacterController controller;

    private float xRotation = 0.0f;

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
        fire = playerControls.Player.Fire;
        fire.Enable();
        fire.performed += OnFire;
    }

    private void OnDisable()
    {
        move.Disable();
        look.Disable();
        fire.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
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
        transform.Rotate(Vector3.up * (lookPosition.x * Time.deltaTime) * xSens);
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        Debug.Log("fired");
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookPosition = ctx.ReadValue<Vector2>();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveDirection = ctx.ReadValue<Vector2>();
    }
}
