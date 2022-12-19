using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using RootMotion.FinalIK;

public class AnimationController : MonoBehaviour
{


    Animator _animator;
    float crouchingInput;
    [HideInInspector] public int speedMultiplier = 2;
    [SerializeField] float lerpSpeed = 10;
    [HideInInspector] public bool isStanding, isCrouching, isAiming, isSprinting, isMovingX, isMovingY, isAirborn;
    CharacterController controller;
    float currentMouseX;
    [SerializeField] bool enableSideStepping;




    public bool forceAim = false, forceSprint = false;

    // float layerWeightLerpValue = 0;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        controller = GetComponentInParent<CharacterController>();
    }



    void Update()
    {

        if (isMovingX || isMovingY) _animator.SetBool("isMoving", true);
        else _animator.SetBool("isMoving", false);

        if (forceAim) isAiming = true;
        else isAiming = Input.GetMouseButton(1);

        if (forceSprint) isSprinting = true;
        else isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (applySprintMotion)
        {
            if (isSprinting) SprintSway();
        }




        isAirborn = !controller.isGrounded ? true : false;

        isMovingX = (Input.GetAxis("Horizontal") != 0) ? true : false;
        isMovingY = (Input.GetAxis("Vertical") != 0) ? true : false;
        isStanding = (!isAiming && !isSprinting && !isMovingX && !isMovingY && !isCrouching) ? true : false;

        if (isAiming) speedMultiplier = 1;
        else if (isSprinting && !isAiming) speedMultiplier = 3;
        else speedMultiplier = 2;

        if (Input.GetKeyDown(KeyCode.C)) isCrouching = !isCrouching;


        if (isCrouching) crouchingInput = Mathf.Lerp(crouchingInput, 1, lerpSpeed);
        else crouchingInput = Mathf.Lerp(crouchingInput, 0, lerpSpeed);

        _animator.SetBool("isAirborn", isAirborn);
        _animator.SetFloat("inputCrouch", crouchingInput, .1f, Time.deltaTime);
        _animator.SetFloat("inputx", Input.GetAxis("Horizontal") * speedMultiplier, .1f, Time.deltaTime);
        _animator.SetFloat("inputy", Input.GetAxis("Vertical") * speedMultiplier, .1f, Time.deltaTime);


        if (!enableSideStepping) return;
        //if charachter is !moving show side stepping


        if (controller.velocity.magnitude != 0) return;

        currentMouseX = Mathf.Lerp(currentMouseX, Input.GetAxis("Mouse X"), lerpSpeed * Time.deltaTime);
        _animator.SetFloat("inputTurn", currentMouseX);







    }

    float elapsed_time;
    [SerializeField] float duration;
    [SerializeField] GameObject weaponPivot;
    [SerializeField] AnimationCurve xSway, ySway;
    [SerializeField] bool applySprintMotion;

    void SprintSway()
    {
        elapsed_time += Time.deltaTime;
        if (elapsed_time >= duration) elapsed_time = 0;
        float lerpRatio = elapsed_time / duration;
        weaponPivot.transform.localPosition = Vector3.LerpUnclamped(weaponPivot.transform.localPosition, new Vector3(EvaluateCurve(xSway, lerpRatio), EvaluateCurve(ySway, lerpRatio), weaponPivot.transform.localPosition.z), Time.deltaTime * 100);

    }
    float EvaluateCurve(AnimationCurve curve, float t) => curve.Evaluate(t);






}
