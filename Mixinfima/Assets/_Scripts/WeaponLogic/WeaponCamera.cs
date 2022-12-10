using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCamera : MonoBehaviour
{

    [SerializeField] AnimationController animationController;
    [SerializeField] Vector3 initialCamPosition;
    [SerializeField] Vector3 adsCamPosition;
    [SerializeField] Vector3 sprintCamPosition;
    [SerializeField] float lerpSpeed;
    [SerializeField] PlayerController controller;
    [SerializeField] GameObject temp;

    void Update()
    {
        CamAim();

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 3, Color.green);

    }

    private void CamAim()
    {
        if (animationController.isAiming) transform.localPosition = Vector3.Lerp(transform.localPosition, adsCamPosition, Time.deltaTime * lerpSpeed);
        else transform.localPosition = Vector3.Lerp(transform.localPosition, initialCamPosition, Time.deltaTime * lerpSpeed);

        if (animationController.isSprinting && !animationController.isAiming)
        {
            transform.localEulerAngles = new Vector3(controller.cameraPitch, 0, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, sprintCamPosition, Time.deltaTime * lerpSpeed);
        }
        else transform.localEulerAngles = new Vector3();



    }






}
