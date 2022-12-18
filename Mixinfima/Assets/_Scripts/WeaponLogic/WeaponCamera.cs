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
    [SerializeField] bool isTps;
    [SerializeField] Vector3 tpsCam;

    void LateUpdate()
    {
        CamAim();
    }

    private void CamAim()
    {

        if (animationController.isSprinting && !animationController.isAiming)
        {
            transform.localEulerAngles = new Vector3(controller.cameraPitch, 0, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, sprintCamPos, Time.deltaTime * lerpSpeed);
        }

        if (animationController.isAiming) transform.localPosition = Vector3.Lerp(transform.localPosition, adsCamPos, Time.deltaTime * lerpSpeed);
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, isTps ? tpsCam : hipCamPos, Time.deltaTime * lerpSpeed);
        }
    }









}
