using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Create Weapon Data", order = 1)]
public class WeaponData : ScriptableObject
{

    [Header("WeaponData:")]
    public int ID;
    public string Name;
    public GameObject WeaponPrefab;
    public GameObject BulletPrefab;
    public GameObject BulletShellPrefab;
    public GameObject MagazinePrefab;
    public Vector3 MagazineLocalSpawnPosition;
    public Vector3 MagazineLocalSpawnRotation;
    public Vector3 MagazineLocalSpawnScale;
    public float BulletVelocity;
    public LayerMask hitMask;
    public Vector3 Spread;
    public int ShootMode;
    public int MaxAmmoPerMag;
    public int MaxAmmoCapacity;
    public float ReloadSpeed;
    public float KickbackForce;
    public AnimationCurve ZkickBack, XrotationRecoil, boltSliderCurve;
    public float FireRate;


    public CamData camData;




    [Header("WeaponPoses:")]
    [Header("Stand_Idle")]

    public Vector3 Stand_Idle_up;
    public Vector3 Stand_Idle_forward;
    public Vector3 Stand_Idle_down;



    [Header("Stand_Aim")]

    public Vector3 Stand_Aim_up;
    public Vector3 Stand_Aim_forward;
    public Vector3 Stand_Aim_down;



    [Header("Crouch_Idle")]

    public Vector3 Crouch_Idle_up;
    public Vector3 Crouch_Idle_forward;
    public Vector3 Crouch_Idle_down;



    [Header("Crouch_Aim")]

    public Vector3 Crouch_Aim_up;
    public Vector3 Crouch_Aim_forward;
    public Vector3 Crouch_Aim_down;








}


