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
    float pickUpTimer, grabTimer;

    // [SerializeField] GameObject crosshairTarget;
    Ray ray;
    RaycastHit hitInfo;
    [SerializeField] AnimationController animationController;

    bool startWeaponGrab, startWeaponPickup;

    void Start()
    {

    }

    // Update is called once per frame
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
                Debug.Log("weapon grabbed");
                startWeaponGrab = false;
                grabTimer = 0;
                startWeaponPickup = true;
            }

            bool GrabWeapon()
            {
                if (grabTimer > 1.0f) return true;
                grabTimer += Time.deltaTime;

                iKController.rightHandIK_target.transform.position = Vector3.Lerp(iKController.rightHandIK_target.transform.position, pickableWeapon.weaponGripRight.transform.position, grabTimer);
                iKController.rightHandIK_target.transform.rotation = Quaternion.Lerp(iKController.rightHandIK_target.transform.rotation, pickableWeapon.weaponGripRight.transform.rotation, grabTimer);
                iKController.leftHandIK_target.transform.position = Vector3.Lerp(iKController.leftHandIK_target.transform.position, pickableWeapon.weaponGripLeft.transform.position, grabTimer);
                iKController.leftHandIK_target.transform.rotation = Quaternion.Lerp(iKController.leftHandIK_target.transform.rotation, pickableWeapon.weaponGripLeft.transform.rotation, grabTimer);
                return false;
            }
        }

        if (startWeaponPickup) PreformWeaponPickup();
        void PreformWeaponPickup()
        {
            if (Pickup())
            {
                Debug.Log("weapon picked");
                startWeaponPickup = false;
                pickUpTimer = 0;
            }
            bool Pickup()
            {
                if (pickUpTimer > 1.0f) return true;
                pickUpTimer += Time.deltaTime;

                pickableWeapon.transform.localPosition = Vector3.Lerp(pickableWeapon.transform.localPosition, new Vector3(), pickUpTimer);
                pickableWeapon.transform.rotation = Quaternion.Lerp(pickableWeapon.transform.rotation, new Quaternion(), pickUpTimer);

                iKController.rightHandIK_target.transform.position = Vector3.Lerp(iKController.rightHandIK_target.transform.position, pickableWeapon.weaponGripRight.transform.position, pickUpTimer);
                iKController.rightHandIK_target.transform.rotation = Quaternion.Lerp(iKController.rightHandIK_target.transform.rotation, pickableWeapon.weaponGripRight.transform.rotation, pickUpTimer);
                iKController.leftHandIK_target.transform.position = Vector3.Lerp(iKController.leftHandIK_target.transform.position, pickableWeapon.weaponGripLeft.transform.position, pickUpTimer);
                iKController.leftHandIK_target.transform.rotation = Quaternion.Lerp(iKController.leftHandIK_target.transform.rotation, pickableWeapon.weaponGripLeft.transform.rotation, pickUpTimer);


                return false;
            }
        }


    }

    void WeaponPickup(RaycastHit hit)
    {
        pickableWeapon = hit.transform.gameObject.GetComponent<WeaponBase>();
        Destroy(pickableWeapon.transform.gameObject.GetComponent<Rigidbody>());
        pickableWeapon.isAvailable = false;
        inventory.AddWeapon(pickableWeapon);
        Destroy(pickableWeapon.rb);
        pickableWeapon.transform.SetParent(inventory.slot.transform);
        startWeaponGrab = true;
    }

}
