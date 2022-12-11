using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamOverride : MonoBehaviour
{


    [SerializeField] GameObject weaponCam;



    [SerializeField] float rayLength = 3;
    [SerializeField] Inventory inventory;
    [SerializeField] LayerMask layer;
    WeaponBase pickableWeapon;
    float pickUpTimer;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ScanAndPickUp();
        // transform.position = Vector3.Lerp(transform.position, pivotCam.transform.position, Time.deltaTime * 30);


        // Vector3 PlayerPos = Vector3.Lerp(oldPos, newPos, springCurve.Evaluate(lerp));
        // or
        // Vector3 PlayerPos = Vector3.LerpUnclamped(oldPos, newPos, springCurve.Evaluate(lerp));



        transform.position = weaponCam.transform.position;

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.green);
    }


    private void ScanAndPickUp()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, rayLength, layer)
            && hit.transform.gameObject.GetComponent<WeaponBase>().isAvailable)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.red);
            if (Input.GetKeyDown(KeyCode.F)) WeaponPickup(hit);
        }

        if (inventory.startPickUp) PreformPickup();
        void PreformPickup()
        {
            if (PickupTransition())
            {
                inventory.startPickUp = false;
                pickUpTimer = 0;
            }

            bool PickupTransition()
            {
                if (pickUpTimer > 1.0f) return true;
                pickUpTimer += Time.deltaTime;
                // pickableWeapon.transform.localPosition = Vector3.Lerp(startPickUpPos, new Vector3(), pickUpTimer);
                // pickableWeapon.transform.localEulerAngles = Vector3.Lerp(startPickUpRot, new Vector3(), pickUpTimer);
                pickableWeapon.transform.localPosition = new Vector3();
                pickableWeapon.transform.localEulerAngles = new Vector3();
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

        ///////TODO: preform hand reaching out and grabbing weapon handle.

        // rightHandIK.transform.position = weaponGrip_right.transform.position;

        inventory.startPickUp = true;
        inventory.startPickUpPos = pickableWeapon.transform.localPosition;
        inventory.startPickUpRot = pickableWeapon.transform.localEulerAngles;
    }


}
