using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public Rigidbody rb;
    public WeaponData weapon;
    public float recoil_elapsed_time;
    public IKController iKController;
    public Inventory inventory;
    public GameObject slot;
    public GameObject weaponGripRight, weaponGripLeft;
    public bool isAvailable, isEquipped, isHolstered;
    public bool isClipEmpty, isShooting, hasFired, hasEjectedShell, isReloading = false;


    public WeaponAttachementManager activeAttachements;





}
