using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamOverride : MonoBehaviour
{


    [SerializeField] GameObject weaponCam;
    [SerializeField] float rayLength = 3;
    [SerializeField] Inventory inventory;
    [SerializeField] IKController iKController;
    [SerializeField] LayerMask pickup_layer;
    [SerializeField] LayerMask target_layer;

    WeaponBase pickableWeapon;
    [SerializeField][Range(0, 1)] float pickUpTimer, grabTimer;

    // [SerializeField] GameObject crosshairTarget;
    Ray ray;
    RaycastHit hitInfo;
    [SerializeField] AnimationController animationController;

    [SerializeField] bool startWeaponGrab, startWeaponPickup;

    void Update()
    {
        // ray.origin = transform.position;
        // ray.direction = transform.forward;
        // Physics.Raycast(ray, out hitInfo, target_layer);
        // crosshairTarget.transform.position = hitInfo.point;

        ScanAndPickUp();
        // transform.position = Vector3.Lerp(transform.position, pivotCam.transform.position, Time.deltaTime * 30);

        // Vector3 PlayerPos = Vector3.Lerp(oldPos, newPos, springCurve.Evaluate(lerp));
        // or
        // Vector3 PlayerPos = Vector3.LerpUnclamped(oldPos, newPos, springCurve.Evaluate(lerp));

        // if (!animationController.isSprinting) transform.position = Vector3.Lerp(transform.position, weaponCam.transform.position, Time.deltaTime * 15);
        if (!animationController.isSprinting) transform.position = weaponCam.transform.position;

    }


    private void ScanAndPickUp()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayLength, pickup_layer))
        {
            if (hit.transform.gameObject.GetComponent<WeaponBase>())
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.red);
                if (Input.GetKeyDown(KeyCode.F)) WeaponPickup(hit);
            }
            else
            {
                Debug.Log(hit.transform.gameObject.GetComponent<WeaponBase>());
            }
        }
        else Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.green);

        if (startWeaponGrab) PreformWeaponGrab();
        void PreformWeaponGrab()
        {
            if (GrabWeapon())
            {
                grabTimer = 0;
                startWeaponPickup = true;
                startWeaponGrab = false;
            }

            bool GrabWeapon()
            {
                if (grabTimer == 1.0f) return true;
                grabTimer += Time.deltaTime;
                grabTimer = Mathf.Clamp01(grabTimer);

                iKController.rightHandIK_target.transform.position = Vector3.Lerp(iKController.rightHandIK_target.transform.position, pickableWeapon.weaponGripRight.transform.position, grabTimer * .1f);
                iKController.leftHandIK_target.transform.position = Vector3.Lerp(iKController.leftHandIK_target.transform.position, pickableWeapon.weaponGripLeft.transform.position, grabTimer * .1f);
                iKController.rightHandIK_target.transform.rotation = Quaternion.Lerp(iKController.rightHandIK_target.transform.rotation, pickableWeapon.weaponGripRight.transform.rotation, grabTimer * .1f);
                iKController.leftHandIK_target.transform.rotation = Quaternion.Lerp(iKController.leftHandIK_target.transform.rotation, pickableWeapon.weaponGripLeft.transform.rotation, grabTimer * .1f);

                return false;
            }
        }

        if (startWeaponPickup) PreformWeaponPickup();
        void PreformWeaponPickup()
        {
            if (Pickup())
            {
                pickUpTimer = 0;
                startWeaponPickup = false;
            }
            bool Pickup()
            {
                if (pickUpTimer == 1.0f) return true;
                pickUpTimer += Time.deltaTime;
                pickUpTimer = Mathf.Clamp01(pickUpTimer);

                pickableWeapon.transform.localPosition = Vector3.Lerp(pickableWeapon.transform.localPosition, new Vector3(), pickUpTimer * .1f);
                pickableWeapon.transform.localRotation = Helpers.Lerp(pickableWeapon.transform.localRotation, Quaternion.identity, pickUpTimer * .1f, true);

                iKController.rightHandIK_target.transform.position = Vector3.Lerp(iKController.rightHandIK_target.transform.position, pickableWeapon.weaponGripRight.transform.position, pickUpTimer);
                iKController.leftHandIK_target.transform.position = Vector3.Lerp(iKController.leftHandIK_target.transform.position, pickableWeapon.weaponGripLeft.transform.position, pickUpTimer);
                iKController.rightHandIK_target.transform.rotation = Quaternion.Lerp(iKController.rightHandIK_target.transform.rotation, pickableWeapon.weaponGripRight.transform.rotation, pickUpTimer);
                iKController.leftHandIK_target.transform.rotation = Quaternion.Lerp(iKController.leftHandIK_target.transform.rotation, pickableWeapon.weaponGripLeft.transform.rotation, pickUpTimer);

                return false;
            }
        }

    }

    void WeaponPickup(RaycastHit hit)
    {
        pickableWeapon = hit.transform.gameObject.GetComponent<WeaponBase>();
        iKController.weaponPrefab = pickableWeapon;
        Destroy(pickableWeapon.transform.gameObject.GetComponent<Rigidbody>());
        pickableWeapon.isAvailable = false;
        inventory.AddWeapon(pickableWeapon);
        Destroy(pickableWeapon.rb);
        pickableWeapon.transform.SetParent(inventory.slot.transform);
        startWeaponGrab = true;
    }

}
