using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCamera : MonoBehaviour
{

    [SerializeField] AnimationController animationController;
    [SerializeField] Vector3 camPos, hipCamPos, adsCamPos, sprintCamPos;
    [SerializeField] float lerpSpeed;
    [SerializeField] PlayerController controller;
    [SerializeField] bool isTps;
    [SerializeField] Vector3 tpsCam;

    [SerializeField] Inventory inventory;


    public State currentCamState;
    public enum State
    {
        HipFire, AimDownSight, SprintCam
    }



    void LateUpdate()
    {
        CamAim();
    }

    private void CamAim()
    {



        if (animationController.isSprinting && !animationController.isAiming) currentCamState = State.SprintCam;
        else if (animationController.isAiming && !animationController.isSprinting) currentCamState = State.AimDownSight;
        else currentCamState = State.HipFire;

        switch (currentCamState)
        {

            case State.SprintCam:
                transform.localEulerAngles = new Vector3(controller.cameraPitch, 0, 0);
                transform.localPosition = Vector3.Lerp(transform.localPosition, sprintCamPos, Time.deltaTime * lerpSpeed);
                break;

            case State.AimDownSight:
                transform.localPosition = Vector3.Lerp(transform.localPosition, CheckCamOffset(), Time.deltaTime * lerpSpeed);
                break;

            case State.HipFire:
                transform.localPosition = Vector3.Lerp(transform.localPosition, isTps ? tpsCam : hipCamPos, Time.deltaTime * lerpSpeed);
                break;

            default:
                Debug.Log("WeaponCamera script error ");
                break;
        }




        // if (animationController.isSprinting && !animationController.isAiming)
        // {
        //     transform.localEulerAngles = new Vector3(controller.cameraPitch, 0, 0);
        //     transform.localPosition = Vector3.Lerp(transform.localPosition, sprintCamPos, Time.deltaTime * lerpSpeed);
        // }

        // // adsCamPos = CheckCamOffset();

        // if (animationController.isAiming) transform.localPosition = Vector3.Lerp(transform.localPosition, adsCamPos, Time.deltaTime * lerpSpeed);
        // else
        // {
        //     transform.localPosition = Vector3.Lerp(transform.localPosition, isTps ? tpsCam : hipCamPos, Time.deltaTime * lerpSpeed);
        // }
    }


    Vector3 CheckCamOffset()
    {
        Vector3 offset;
        // if (scopeManager.activeScope) adsCamPos = transform.InverseTransformPoint(scopeManager.activeScope.GetCamOffset().transform.position);

        if (inventory.GetEquippedIndex() == -1) return adsCamPos;

        if (inventory.GetEquipped().activeAttachements.scopeManager.activeScope)
        {
            offset = inventory.GetEquipped().activeAttachements.scopeManager.activeScope.GetCamOffset().transform.position;
            return transform.InverseTransformPoint(offset);
        }

        return adsCamPos;

    }









}
