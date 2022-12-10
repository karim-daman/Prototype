using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public WeaponData weapon;
    public bool isAvailable, isEquipped, isHolstered;
    public Rigidbody rb;
    public bool isClipEmpty, isShooting, hasFired, hasEjectedShell, isReloading = false;
    public float recoil_elapsed_time;












}
