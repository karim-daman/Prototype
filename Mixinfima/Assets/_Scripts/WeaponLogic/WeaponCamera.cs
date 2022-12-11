using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCamera : MonoBehaviour
{

    [SerializeField] AnimationController animationController;
    [SerializeField] Vector3 hipCamPos, adsCamPos, sprintCamPos;
    [SerializeField] float lerpSpeed;
    [SerializeField] PlayerController controller;



    void Update()
    {
        CamAim();
    }

    private void CamAim()
    {
        if (animationController.isAiming) transform.localPosition = Vector3.Lerp(transform.localPosition, adsCamPos, Time.deltaTime * lerpSpeed);
        else transform.localPosition = Vector3.Lerp(transform.localPosition, hipCamPos, Time.deltaTime * lerpSpeed);

        if (animationController.isSprinting && !animationController.isAiming)
        {
            transform.localEulerAngles = new Vector3(controller.cameraPitch, 0, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, sprintCamPos, Time.deltaTime * lerpSpeed);
        }
        else transform.localEulerAngles = new Vector3();
    }









}
