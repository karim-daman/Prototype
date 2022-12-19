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


    WeaponBase pickableWeapon;

    [SerializeField] AnimationController animationController;

    // [SerializeField] LayerMask target_layer;
    // [SerializeField] GameObject crosshairTarget;
    // Ray ray;
    // RaycastHit hitInfo;


    void Update()
    {
        // ray.origin = transform.position;
        // ray.direction = transform.forward;
        // Physics.Raycast(ray, out hitInfo, target_layer);
        // crosshairTarget.transform.position = hitInfo.point;

        ScanforPickups();

        // transform.position = Vector3.Lerp(transform.position, pivotCam.transform.position, Time.deltaTime * 30);
        // Vector3 PlayerPos = Vector3.Lerp(oldPos, newPos, springCurve.Evaluate(lerp));
        // or
        // Vector3 PlayerPos = Vector3.LerpUnclamped(oldPos, newPos, springCurve.Evaluate(lerp));




    }

    private void LateUpdate()
    {
        if (!animationController.isSprinting) transform.position = weaponCam.transform.position;
    }


    private void ScanforPickups()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayLength, pickup_layer))
        {
            if (hit.transform.gameObject.GetComponent<WeaponBase>().isAvailable)
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.red);
                if (Input.GetKeyDown(KeyCode.F)) WeaponPickup(hit);
            }
        }
        else Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayLength, Color.green);
    }

    void WeaponPickup(RaycastHit hit)
    {

        if (inventory.GetEquippedIndex() != -1) inventory.DropWeapon();

        iKController.isBusyPickingUp = true;
        pickableWeapon = hit.transform.gameObject.GetComponent<WeaponBase>();
        pickableWeapon.iKController = iKController;
        pickableWeapon.inventory = inventory;
        pickableWeapon.slot = inventory.slot;

        iKController.weaponPrefab = pickableWeapon;
        Destroy(pickableWeapon.transform.gameObject.GetComponent<Rigidbody>());
        pickableWeapon.isAvailable = false;
        inventory.AddWeapon(pickableWeapon);
        Destroy(pickableWeapon.rb);
        iKController.startWeaponGrab = true;
    }

}
