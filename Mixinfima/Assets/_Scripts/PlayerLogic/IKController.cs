using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System;
using RootMotion.FinalIK;

public class IKController : MonoBehaviour
{

    [Header("--- Misc")]
    #region Misc

    [SerializeField] bool enableIK;
    [SerializeField] Inventory inventory;
    [SerializeField] float lerpSpeed = 15;
    [SerializeField] Rig leftHandRig;
    [SerializeField] Rig rightHandRig;
    [SerializeField] FullBodyBipedIK fbbik;
    [SerializeField] GameObject bagPack;
    [SerializeField] GameObject slot;

    [SerializeField] GameObject weaponCam;
    [SerializeField] float minPickupDistance;

    // [SerializeField] List<GameObject> ragdollObjects;
    // [SerializeField] List<Rigidbody> ragdoll_bodies;
    // [SerializeField] List<Collider> ragdoll_Colliders;
    // [SerializeField] FullBodyBipedIK fullBodyBipedIK;
    // [SerializeField] GameObject weaponGrip_right;
    // [SerializeField] GameObject weaponGrip_left;


    public bool isArmedRight;
    public bool isArmedLeft;
    float timer;
    float start;

    private bool startPickUp;

    float pickUpTimer;

    WeaponBase pickableWeapon;

    Vector3 startPickUpPos;
    Vector3 startPickUpRot;

    #endregion

    [Header("--- Constraints")]
    #region Constraints
    [SerializeField] List<GameObject> constraints_list;
    [SerializeField] AnimationController animationController;
    [SerializeField] List<Rig> rig_list;
    [SerializeField] float aimLerpSpeed;
    [SerializeField] float sprintLerpSpeed;

    int idle_layer = 0;
    int aim_layer = 1;
    int sprint_layer = 2;
    int holster_layer = 3;

    #endregion

    [Header("--- Reloading")]
    #region Reloading
    // [SerializeField] List<Transform> reload_targets;
    public GameObject leftHandIK;
    public GameObject rightHandIK;
    public List<TwoBoneIKConstraint> fingerBones_left;
    // [SerializeField] GameObject oldClip;
    [SerializeField] GameObject weaponPrefab;
    // [SerializeField] bool emptyClip;
    // [SerializeField] int step = -1;
    // [SerializeField] bool isReloading = false;
    // float lerpTime = 0;
    #endregion

    [Header("--- Posing")]
    #region Posing
    [SerializeField] PlayerController playerController;
    public State currentState;

    MultiPositionConstraint idle_pos_constraint;
    MultiPositionConstraint aim_pos_constraint;

    public enum State
    {
        Stand_Idle,
        Stand_Aim,
        Crouch_Idle,
        Crouch_Aim,
    }
    #endregion 

    [Header("--- Shooting")]
    #region shooting

    [SerializeField] GameObject rifleSlider;
    [SerializeField] GameObject rifleSlot;
    [SerializeField] GameObject rifleBoltSlider;
    [SerializeField] GameObject bulletSpawnPoint;
    [SerializeField] ParticleSystem shellParticles;
    [SerializeField] GameObject bulletContainer;
    float elapsed_time;
    bool isShooting, hasEjectedShell, hasFired;
    float aim_flerp_elapsed, sprint_flerp_elapsed;

    #endregion

    private void Awake()
    {
        fbbik = GetComponent<FullBodyBipedIK>();
        if (fbbik == null)
        {
            Debug.Log("fbbik is null");
            return;
        }

        idle_pos_constraint = constraints_list[idle_layer].GetComponent<MultiPositionConstraint>();
        aim_pos_constraint = constraints_list[aim_layer].GetComponent<MultiPositionConstraint>();

        isArmedRight = isArmedLeft = false;
    }



    void Update()
    {
        if (!enableIK) return;
        if (Input.GetKey(KeyCode.Tab)) Time.timeScale = .1f;
        else Time.timeScale = 1;



        if (inventory.GetEquippedIndex() == -1)
        {
            ArmRight(false);
            ArmLeft(false);
            return;
        }


        #region Holstering

        if (Input.GetKeyDown(KeyCode.X)) HandleHolsterTransition();

        void HandleHolsterTransition()
        {
            weaponPrefab.transform.parent = slot.transform;
            weaponPrefab.transform.localPosition = new Vector3();
            weaponPrefab.transform.localEulerAngles = new Vector3();

            inventory.GetEquipped().isEquipped = !inventory.GetEquipped().isEquipped;
            start = rig_list[holster_layer].weight;
            timer = 0.0f;
            if (inventory.GetEquipped().isEquipped) isArmedRight = true;
            else
            {
                isArmedRight = true;
                isArmedLeft = false;
            }
        }

        bool HolsterTransition()
        {
            if (timer > 1.0f) return true;
            timer += Time.deltaTime * 2;
            rig_list[holster_layer].weight = Mathf.Lerp(start, inventory.GetEquipped().isEquipped ? 0 : 1, timer);
            return false;
        }

        // if (inventory.GetEquipped().isEquipped && HolsterTransition()) isArmedLeft = isArmedRight = true;
        if (inventory.GetEquipped().isEquipped)
        {
            if (HolsterTransition())
            {
                isArmedLeft = isArmedRight = true;
            }
        }
        else
        {
            if (HolsterTransition())
            {
                isArmedRight = isArmedLeft = false;
                weaponPrefab.transform.parent = bagPack.transform;
                weaponPrefab.transform.localPosition = new Vector3();
                weaponPrefab.transform.localEulerAngles = new Vector3();
            }
        }

        ArmRight(isArmedRight);
        ArmLeft(isArmedLeft);

        void ArmRight(bool state)
        {
            fbbik.solver.rightArmMapping.weight = Mathf.Lerp(fbbik.solver.rightArmMapping.weight, state ? 1 : 0, Time.deltaTime * lerpSpeed);
            rightHandRig.weight = Mathf.Lerp(rightHandRig.weight, state ? 1 : 0, Time.deltaTime * lerpSpeed);
        }

        void ArmLeft(bool state)
        {
            fbbik.solver.leftArmMapping.weight = Mathf.Lerp(fbbik.solver.leftArmMapping.weight, state ? 1 : 0, Time.deltaTime * lerpSpeed);
            leftHandRig.weight = Mathf.Lerp(leftHandRig.weight, state ? 1 : 0, Time.deltaTime * lerpSpeed);
        }
        #endregion

        #region constraints

        if (animationController.isAiming) aim_flerp_elapsed = Mathf.Clamp01(aim_flerp_elapsed + (Time.deltaTime * aimLerpSpeed));
        else aim_flerp_elapsed = Mathf.Clamp01(aim_flerp_elapsed - (Time.deltaTime * aimLerpSpeed));

        if (!animationController.isCrouching && !animationController.isAiming && animationController.isSprinting) sprint_flerp_elapsed = Mathf.Clamp01(sprint_flerp_elapsed + (Time.deltaTime * sprintLerpSpeed));
        else sprint_flerp_elapsed = Mathf.Clamp01(sprint_flerp_elapsed - (Time.deltaTime * sprintLerpSpeed));

        rig_list[aim_layer].weight = aim_flerp_elapsed;
        rig_list[sprint_layer].weight = sprint_flerp_elapsed;

        #endregion



        #region Posing
        if (!animationController.isCrouching && !animationController.isAiming) currentState = State.Stand_Idle;
        if (!animationController.isCrouching && animationController.isAiming) currentState = State.Stand_Aim;
        if (animationController.isCrouching && !animationController.isAiming) currentState = State.Crouch_Idle;
        if (animationController.isCrouching && animationController.isAiming) currentState = State.Crouch_Aim;

        switch (currentState)
        {

            case State.Stand_Idle:
                CalculatePosition(idle_layer, inventory.GetEquipped().weapon.Stand_Idle_up, inventory.GetEquipped().weapon.Stand_Idle_forward, inventory.GetEquipped().weapon.Stand_Idle_down);
                break;
            case State.Stand_Aim:
                CalculatePosition(aim_layer, inventory.GetEquipped().weapon.Stand_Aim_up, inventory.GetEquipped().weapon.Stand_Aim_forward, inventory.GetEquipped().weapon.Stand_Aim_down);
                break;
            case State.Crouch_Idle:
                CalculatePosition(idle_layer, inventory.GetEquipped().weapon.Crouch_Idle_up, inventory.GetEquipped().weapon.Crouch_Idle_forward, inventory.GetEquipped().weapon.Crouch_Idle_down);
                break;
            case State.Crouch_Aim:
                CalculatePosition(aim_layer, inventory.GetEquipped().weapon.Crouch_Aim_up, inventory.GetEquipped().weapon.Crouch_Aim_forward, inventory.GetEquipped().weapon.Crouch_Aim_down);
                break;
            default:
                Debug.Log("!problem!");
                break;
        }

        void CalculatePosition(int layer_id, Vector3 upPos, Vector3 forwardPos, Vector3 downPos)
        {
            float currentPositiveAngle = playerController.cameraPitch + 90;
            float lerpAmount = currentPositiveAngle / 180;
            Vector3 upLerp = Vector3.Lerp(upPos, forwardPos, lerpAmount);
            Vector3 downLerp = Vector3.Lerp(forwardPos, downPos, lerpAmount);
            Vector3 newPos = Vector3.Lerp(upLerp, downLerp, lerpAmount);
            Vector3 oldPos;

            switch (layer_id)
            {
                case 0:
                    oldPos = idle_pos_constraint.data.offset;
                    idle_pos_constraint.data.offset = Vector3.Lerp(oldPos, newPos, Time.deltaTime * lerpSpeed);
                    break;
                case 1:
                    oldPos = aim_pos_constraint.data.offset;
                    aim_pos_constraint.data.offset = Vector3.Lerp(oldPos, newPos, Time.deltaTime * lerpSpeed);
                    break;
                default:
                    Debug.Log("CalculatePosition!problem!");
                    break;
            }

        }

        #endregion

    }

    bool flerp(float flerp_timer, float weight, float start, float finish, float multiplier)
    {
        if (flerp_timer > 1.0f) return true;
        flerp_timer += Time.deltaTime;
        weight = Mathf.Lerp(start, finish, flerp_timer * multiplier);
        return false;
    }

}