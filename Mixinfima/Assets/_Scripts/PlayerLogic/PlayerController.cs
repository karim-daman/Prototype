using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] AnimationController animationController;
    [SerializeField] float moveSpeed = 4;
    [SerializeField] float walkSpeed = 2.0f;
    [SerializeField] float runSpeed = 4f;
    [SerializeField] float sprintSpeed = 6.0f;
    [SerializeField] float gravity = -13.0f;
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] bool lockCursor = true;
    [HideInInspector] public float cameraPitch = 0.0f;
    [HideInInspector] public float cameraYaw = 0.0f;
    float velocityY = 0.0f;
    CharacterController controller = null;
    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;
    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    public float gravityScale = 1;
    public float jumpHeight = 1;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (animationController.speedMultiplier == 1) moveSpeed = walkSpeed;
        else if (animationController.speedMultiplier == 2) moveSpeed = runSpeed;
        else moveSpeed = sprintSpeed;
        if (animationController.isCrouching) moveSpeed = walkSpeed;

        UpdateMouseLook();
        UpdateMovement();
    }

    void UpdateMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);
        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);
        playerCamera.localEulerAngles = new Vector3(cameraPitch, 0, 0);
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement()
    {
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded) velocityY = Mathf.Sqrt(jumpHeight * -2f * (gravity * gravityScale));
        velocityY += gravity * gravityScale * Time.deltaTime;

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * moveSpeed + Vector3.up * velocityY;
        controller.Move(velocity * Time.deltaTime);
    }




}

