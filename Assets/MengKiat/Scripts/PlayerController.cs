using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CinemachineFreeLook freeLookCam;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float lookRotationSmoothTime = 0.05f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minFOV = 30f;
    [SerializeField] private float maxFOV = 70f;

    private CharacterController cc;
    private Transform camTransform;
    private InputAction moveAction;
    private InputAction lookModeAction;
    private InputAction zoomAction;
    private InputAction jumpAction;

    private Vector2 moveInput;
    private Vector2 prevMoveInput;
    private Vector3 storedDir;
    private float rotationVelocityY;
    private float lookRotationVelocityY;
    private float verticalVelocity;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        camTransform = Camera.main.transform;

        moveAction = playerInput.actions["Move"];
        lookModeAction = playerInput.actions["LookMode"];
        zoomAction = playerInput.actions["Zoom"];
        jumpAction = playerInput.actions["Jump"];

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        prevMoveInput = Vector2.zero;
        storedDir = Vector3.zero;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookModeAction.Enable();
        zoomAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookModeAction.Disable();
        zoomAction.Disable();
        jumpAction.Disable();
    }

    private void Update()
    {
        // Handle Zoom
        Vector2 scroll = zoomAction.ReadValue<Vector2>();
        if (Mathf.Abs(scroll.y) > 0.01f)
        {
            float fov = freeLookCam.m_Lens.FieldOfView;
            fov -= scroll.y * zoomSpeed * Time.deltaTime;
            freeLookCam.m_Lens.FieldOfView = Mathf.Clamp(fov, minFOV, maxFOV);
        }

        moveInput = moveAction.ReadValue<Vector2>();
        bool isLookMode = lookModeAction.ReadValue<float>() > 0f;
        float camYaw = camTransform.eulerAngles.y;

        // Jump: if grounded and jump pressed, set verticalVelocity
        if (cc.isGrounded && jumpAction.triggered)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (isLookMode)
        {
            Vector3 camForward = camTransform.forward;
            camForward.y = 0f;
            camForward.Normalize();
            float camForwardYaw = Mathf.Atan2(camForward.x, camForward.z) * Mathf.Rad2Deg;

            float smoothYaw = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                camForwardYaw,
                ref lookRotationVelocityY,
                lookRotationSmoothTime
            );
            transform.rotation = Quaternion.Euler(0f, smoothYaw, 0f);

            if (moveInput.sqrMagnitude >= 0.01f)
            {
                Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
                Vector3 desiredDir = Quaternion.Euler(0f, camYaw, 0f) * inputDir;
                Vector3 horizontalMove = desiredDir * moveSpeed;
                ApplyGravity(ref horizontalMove);
                cc.Move(horizontalMove * Time.deltaTime);
            }
            else
            {
                ApplyGravityOnly();
            }

            storedDir = Vector3.zero;
        }
        else
        {
            if (moveInput.sqrMagnitude >= 0.01f)
            {
                if (!Approximately(moveInput, prevMoveInput))
                {
                    Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
                    storedDir = Quaternion.Euler(0f, camYaw, 0f) * inputDir;
                }

                float targetYaw = Mathf.Atan2(storedDir.x, storedDir.z) * Mathf.Rad2Deg;
                float smoothYaw = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    targetYaw,
                    ref rotationVelocityY,
                    rotationSmoothTime
                );
                transform.rotation = Quaternion.Euler(0f, smoothYaw, 0f);

                Vector3 horizontalMove = storedDir * moveSpeed;
                ApplyGravity(ref horizontalMove);
                cc.Move(horizontalMove * Time.deltaTime);
            }
            else
            {
                storedDir = Vector3.zero;
                ApplyGravityOnly();
            }
        }

        prevMoveInput = moveInput;
    }

    private void ApplyGravity(ref Vector3 moveVector)
    {
        if (cc.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        moveVector.y = verticalVelocity;
    }

    private void ApplyGravityOnly()
    {
        if (cc.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 fall = Vector3.up * verticalVelocity;
        cc.Move(fall * Time.deltaTime);
    }

    private bool Approximately(Vector2 a, Vector2 b, float threshold = 0.01f)
    {
        return (a - b).sqrMagnitude < threshold * threshold;
    }
}
