using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CapsuleController : MonoBehaviour
{



    [Header("Setup")]
    Rigidbody rb;
    [SerializeField] Transform cam;
    [SerializeField] Transform Character;
    // [SerializeField] GroundChecker checker;

    [Header("Settings")]
    [SerializeField] bool enableTilt;
    // [SerializeField] float jumpHeight = 10;
    [SerializeField] float minimumSpeed = 10, MaximumSpeed = 20;
    [SerializeField] ForceMode forceMode = ForceMode.Force;
    [SerializeField] float sensitivity = 500;
    [SerializeField] float TiltAngle = 25f;
    bool jumpAction;
    public bool isRunning = false, isParachuting = false;


    Vector2 input;
    float movementForce = 1f;
    bool runningAction, parachutingAction;
    float pitch, yaw;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startFixedDeltaTime = Time.fixedDeltaTime;
        startTimeScale = Time.timeScale;
    }

    void Update()
    {

        #region slow motion
        ///--------time scale for slow motion stuff
        if (Input.GetKey(KeyCode.F)) StartSlowMotion();
        else StopSlowMotion();
        ///--------time scale for slow motion stuff
        #endregion

        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        look_and_tilt();

        // if (!jumpAction) jumpAction = Input.GetKeyDown(KeyCode.Space);

        runningAction = Input.GetKey(KeyCode.LeftShift);
        parachutingAction = Input.GetKey(KeyCode.G);

    }

    public float slowMotionTimeScale;
    float startTimeScale, startFixedDeltaTime;

    private void StartSlowMotion()
    {
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimeScale;
    }
    private void StopSlowMotion()
    {
        Time.timeScale = startTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime;
    }



    void look_and_tilt()
    {
        pitch = Mathf.Clamp(pitch - (Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime), -90f, 90f);
        yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        //--------------------------------------------------------------------uncomment
        cam.localRotation = Quaternion.Euler(pitch, 0, 0);
        // rotate around mouse and rotate around towards velocity
        Character.transform.localRotation = Quaternion.Euler(0, yaw, 0);
        //--------------------------------------------------------------------uncomment

        if (enableTilt)
        {
            Character.transform.localRotation = TiltCharacterTowardsVelocity(Character.transform.localRotation, rb.velocity, TiltAngle);
            //try cancel out the character tilt applied on camera pov.
        }


    }

    Quaternion TiltCharacterTowardsVelocity(Quaternion cleanRotation, Vector3 vel, float velMagFor45Degree)
    {
        Vector3 rotAxis = Vector3.Cross(Vector3.up, vel);
        float tiltAngle = Mathf.Atan(vel.magnitude / velMagFor45Degree) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(tiltAngle, rotAxis) * cleanRotation;  //order matters
    }

    private void FixedUpdate()
    {
        playerMove();
        // playerJump();

        #region functions
        void playerMove()
        {
            Vector3 direction = (Character.forward * input.y) + (Character.right * input.x);
            Vector3 clampedDirection = Vector3.ClampMagnitude(direction, 1);

            isRunning = runningAction ? true : false;
            if (isRunning) movementForce += 1;
            else movementForce -= 1;

            isParachuting = parachutingAction ? true : false;
            if (isParachuting) rb.drag = 10;
            else rb.drag = 1;

            movementForce = Mathf.Clamp(movementForce, minimumSpeed, MaximumSpeed);
            rb.AddForce(clampedDirection * movementForce, forceMode);
        }

        // void playerJump()
        // {
        //     if (checker.isGrounded() && jumpAction)
        //     {
        //         // prepare to jump 
        //         rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        //     }
        //     jumpAction = false;
        // }
        #endregion
    }


}


