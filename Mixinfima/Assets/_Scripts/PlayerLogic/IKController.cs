using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System;
using RootMotion.FinalIK;

public class IKController : MonoBehaviour
{

    public bool isBusyPickingUp, startWeaponGrab, startWeaponPickup;
    float grabTimer, pickUpTimer;


    [SerializeField][Range(0, 1)] float ArmRightLerpTimer;
    [SerializeField][Range(0, 1)] float ArmLeftLerpTimer;

    [SerializeField] bool enableIK;
    [SerializeField] Inventory inventory;
    [SerializeField] float lerpSpeed = 15;
    [SerializeField] Rig leftHandRig;
    [SerializeField] Rig rightHandRig;
    [SerializeField] FullBodyBipedIK fbbik;
    [SerializeField] GameObject bagPack;
    [SerializeField] GameObject slot;
    public bool isArmedRight;
    public bool isArmedLeft;
    float holster_timer;
    float start;
    [SerializeField] List<GameObject> constraints_list;
    [SerializeField] AnimationController animationController;
    [SerializeField] public List<Rig> rig_list;
    [SerializeField] float aimLerpSpeed;
    [SerializeField] float sprintLerpSpeed;
    int idle_layer = 0, aim_layer = 1, sprint_layer = 2, holster_layer = 3, unarmed = 4;
    public GameObject leftHandIK_target;
    public GameObject rightHandIK_target;
    public List<TwoBoneIKConstraint> fingerBones_left;
    public WeaponBase weaponPrefab;
    PlayerController playerController;
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
    float aim_flerp_elapsed, sprint_flerp_elapsed;

    [SerializeField] GameObject readyToThrow;


    private void Awake()
    {

        playerController = GetComponentInParent<PlayerController>();

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

        if (inventory.GetEquippedIndex() == -1)
        {
            ArmRight(false);
            ArmLeft(false);

            return;
        }

        if (Input.GetKey(KeyCode.J)) rightHandIK_target.transform.position = Vector3.Lerp(rightHandIK_target.transform.position, readyToThrow.transform.position, Time.deltaTime * 10);
        else rightHandIK_target.transform.position = Vector3.Lerp(rightHandIK_target.transform.position, weaponPrefab.weaponGripRight.transform.position, Time.deltaTime * 10);

        #region Holstering
        // if (Input.GetKeyDown(KeyCode.X) && !isBusyPickingUp) HandleHolsterTransition();
        if (Input.GetKeyDown(KeyCode.X)) HandleHolsterTransition();

        void HandleHolsterTransition()
        {
            weaponPrefab.transform.parent = slot.transform;
            weaponPrefab.transform.localPosition = new Vector3();
            weaponPrefab.transform.localEulerAngles = new Vector3();

            inventory.GetEquipped().isEquipped = !inventory.GetEquipped().isEquipped;
            start = rig_list[holster_layer].weight;
            holster_timer = 0.0f;
            if (inventory.GetEquipped().isEquipped) isArmedRight = true;
            else
            {
                isArmedRight = true;
                isArmedLeft = false;
            }
        }

        bool HolsterTransition()
        {
            if (holster_timer > 1.0f) return true;
            holster_timer += Time.deltaTime * 2;
            rig_list[holster_layer].weight = Mathf.Lerp(start, inventory.GetEquipped().isEquipped ? 0 : 1, holster_timer);
            return false;
        }

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
        #endregion

        #region Anim constraints

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

        #region Pickups

        if (startWeaponGrab) PreformWeaponGrab();
        void PreformWeaponGrab()
        {
            if (GrabWeapon())
            {
                grabTimer = 0;
                startWeaponGrab = false;
                weaponPrefab.transform.SetParent(inventory.slot.transform);
                startWeaponPickup = true;
            }

            bool GrabWeapon()
            {
                if (grabTimer == 1.0f) return true;
                grabTimer += Time.deltaTime * 10;
                grabTimer = Mathf.Clamp01(grabTimer);

                rightHandIK_target.transform.position = Vector3.Lerp(rightHandIK_target.transform.position, weaponPrefab.weaponGripRight.transform.position, grabTimer * .1f);
                leftHandIK_target.transform.position = Vector3.Lerp(leftHandIK_target.transform.position, weaponPrefab.weaponGripLeft.transform.position, grabTimer * .1f);
                rightHandIK_target.transform.rotation = Quaternion.Lerp(rightHandIK_target.transform.rotation, weaponPrefab.weaponGripRight.transform.rotation, grabTimer * .1f);
                leftHandIK_target.transform.rotation = Quaternion.Lerp(leftHandIK_target.transform.rotation, weaponPrefab.weaponGripLeft.transform.rotation, grabTimer * .1f);

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
                isBusyPickingUp = false;

            }
            bool Pickup()
            {
                if (pickUpTimer == 1.0f) return true;
                pickUpTimer += Time.deltaTime;
                pickUpTimer = Mathf.Clamp01(pickUpTimer);

                weaponPrefab.transform.localPosition = Vector3.Lerp(weaponPrefab.transform.localPosition, new Vector3(), pickUpTimer * .1f);
                weaponPrefab.transform.localRotation = Helpers.qLerp(weaponPrefab.transform.localRotation, Quaternion.identity, pickUpTimer * .1f, true);

                rightHandIK_target.transform.position = Vector3.Lerp(rightHandIK_target.transform.position, weaponPrefab.weaponGripRight.transform.position, pickUpTimer);
                leftHandIK_target.transform.position = Vector3.Lerp(leftHandIK_target.transform.position, weaponPrefab.weaponGripLeft.transform.position, pickUpTimer);
                rightHandIK_target.transform.rotation = Quaternion.Lerp(rightHandIK_target.transform.rotation, weaponPrefab.weaponGripRight.transform.rotation, pickUpTimer);
                leftHandIK_target.transform.rotation = Quaternion.Lerp(leftHandIK_target.transform.rotation, weaponPrefab.weaponGripLeft.transform.rotation, pickUpTimer);

                return false;
            }
        }
        #endregion

        #region Arming/Disarming

        if (ArmRight(isArmedRight)) Debug.Log("done.");
        else Debug.Log(" running...");
        if (ArmLeft(isArmedLeft)) Debug.Log("done.");
        else Debug.Log(" running...");


        bool ArmRight(bool state)
        {
            float goal = state ? 1 : 0;
            if (rightHandRig.weight == goal) return true;
            if (state) ArmRightLerpTimer += Time.deltaTime;
            else ArmRightLerpTimer -= Time.deltaTime;

            ArmRightLerpTimer = Mathf.Clamp01(ArmRightLerpTimer);
            fbbik.solver.rightArmMapping.weight = Mathf.Lerp(fbbik.solver.rightArmMapping.weight, goal, ArmRightLerpTimer);
            rightHandRig.weight = Mathf.Lerp(rightHandRig.weight, goal, ArmRightLerpTimer);
            return false;
        }

        bool ArmLeft(bool state)
        {
            float goal = state ? 1 : 0;
            if (leftHandRig.weight == goal) return true;
            if (state) ArmLeftLerpTimer += Time.deltaTime;
            else ArmLeftLerpTimer -= Time.deltaTime;

            ArmLeftLerpTimer = Mathf.Clamp01(ArmLeftLerpTimer);
            fbbik.solver.leftArmMapping.weight = Mathf.Lerp(fbbik.solver.leftArmMapping.weight, goal, ArmLeftLerpTimer);
            leftHandRig.weight = Mathf.Lerp(leftHandRig.weight, goal, ArmLeftLerpTimer);
            return false;
        }

        #endregion

    }
}
