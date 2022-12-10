using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<WeaponBase> weapons = new List<WeaponBase>();
    public int equippedIndex = 0;
    private WeaponBase equipped_weapon;

    [SerializeField] GameObject weaponCam;
    [SerializeField] float minPickupDistance;
    [SerializeField] GameObject slot;

    WeaponBase pickableWeapon;
    Vector3 startPickUpPos;
    Vector3 startPickUpRot;
    private bool startPickUp;
    float pickUpTimer;


    void Update()
    {
        PickupWeapon();
    }

    private void PickupWeapon()
    {
        RaycastHit hit;
        if (Physics.Raycast(weaponCam.transform.position, weaponCam.transform.TransformDirection(Vector3.forward), out hit, minPickupDistance)
            && hit.transform.tag == "Weapon"
            && hit.transform.gameObject.GetComponent<WeaponBase>().isAvailable)
        {
            Debug.DrawRay(weaponCam.transform.position, weaponCam.transform.TransformDirection(Vector3.forward) * minPickupDistance, Color.red);

            if (Input.GetKeyDown(KeyCode.F))
            {
                pickableWeapon = hit.transform.gameObject.GetComponent<WeaponBase>();
                Destroy(pickableWeapon.transform.gameObject.GetComponent<Rigidbody>());
                pickableWeapon.isAvailable = false;
                AddWeapon(pickableWeapon);
                Destroy(pickableWeapon.rb);
                pickableWeapon.transform.SetParent(slot.transform);

                ///////TODO: preform hand reaching out and grabbing weapon handle.

                // rightHandIK.transform.position = weaponGrip_right.transform.position;

                startPickUp = true;
                startPickUpPos = pickableWeapon.transform.localPosition;
                startPickUpRot = pickableWeapon.transform.localEulerAngles;
            }
        }


        if (startPickUp)
        {
            if (PickupTransition())
            {
                startPickUp = false;
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

    public void AddWeapon(WeaponBase pickableItem)
    {
        pickableItem.isAvailable = false;
        pickableItem.isEquipped = true;
        weapons.Add(pickableItem);
        equippedIndex++;
    }

    public void DropWeapon()
    {
        GetEquipped().isAvailable = true;
        GetEquipped().isEquipped = false;
        GetEquipped().transform.SetParent(null);
        GetEquipped().rb = GetEquipped().transform.gameObject.AddComponent<Rigidbody>();
        GetEquipped().GetComponent<Rigidbody>().isKinematic = false;
        GetEquipped().GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        GetEquipped().GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        RemoveWeapon(GetEquippedIndex());
    }

    public void RemoveWeapon(int index)
    {
        weapons.RemoveAt(index);
        if (weapons.Count == 0) equippedIndex = -1;
    }

    public WeaponBase GetEquipped() => weapons[equippedIndex];
    public int GetEquippedIndex() => equippedIndex;
    public int GetPreviousIndex() => --equippedIndex < 0 ? weapons.Count : equippedIndex;
    public int GetNextIndex() => ++equippedIndex > weapons.Count ? 0 : equippedIndex;

    // public void EquipNextWeapon() => Equip(GetNextIndex());
    // public void EquipPreviousWeapon() => Equip(GetPreviousIndex());
    // private void Equip(int v)
    // {
    //     throw new NotImplementedException();
    // }


}

//     /// <summary>
//     /// Array of all weapons. These are gotten in the order that they are parented to this object.
//     /// </summary>
//     private WeaponBehaviour[] weapons;

//     /// <summary>
//     /// Currently equipped WeaponBehaviour.
//     /// </summary>
//     private WeaponBehaviour equipped;
//     /// <summary>
//     /// Currently equipped index.
//     /// </summary>
//     private int equippedIndex = -1;

//     #endregion

//     #region METHODS

//     public override void Init(int equippedAtStart = 0)
//     {
//         //Cache all weapons. Beware that weapons need to be parented to the object this component is on!
//         weapons = GetComponentsInChildren<WeaponBehaviour>(true);

//         //Disable all weapons. This makes it easier for us to only activate the one we need.
//         foreach (WeaponBehaviour weapon in weapons)
//             weapon.gameObject.SetActive(false);

//         //Equip.
//         Equip(equippedAtStart);
//     }

//     public override WeaponBehaviour Equip(int index)
//     {
//         //If we have no weapons, we can't really equip anything.
//         if (weapons == null)
//             return equipped;

//         //The index needs to be within the array's bounds.
//         if (index > weapons.Length - 1)
//             return equipped;

//         //No point in allowing equipping the already-equipped weapon.
//         if (equippedIndex == index)
//             return equipped;

//         //Disable the currently equipped weapon, if we have one.
//         if (equipped != null)
//             equipped.gameObject.SetActive(false);

//         //Update index.
//         equippedIndex = index;
//         //Update equipped.
//         equipped = weapons[equippedIndex];
//         //Activate the newly-equipped weapon.
//         equipped.gameObject.SetActive(true);

//         //Return.
//         return equipped;
//     }

//     #endregion

//     #region Getters

//     public override int GetLastIndex()
//     {
//         //Get last index with wrap around.
//         int newIndex = equippedIndex - 1;
//         if (newIndex < 0)
//             newIndex = weapons.Length - 1;

//         //Return.
//         return newIndex;
//     }

//     public override int GetNextIndex()
//     {
//         //Get next index with wrap around.
//         int newIndex = equippedIndex + 1;
//         if (newIndex > weapons.Length - 1)
//             newIndex = 0;

//         //Return.
//         return newIndex;
//     }

//     public override WeaponBehaviour GetEquipped() => equipped;
//     public override int GetEquippedIndex() => equippedIndex;
