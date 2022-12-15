using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System;
using RootMotion.FinalIK;

public class IKController : MonoBehaviour
{

    [SerializeField] float armingMultiplier;
    // public float ArmRightLerpTimer { get; private set; }
    // public float ArmLeftLerpTimer { get; private set; }

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
    float timer;
    float start;
    [SerializeField] List<GameObject> constraints_list;
    [SerializeField] AnimationController animationController;
    [SerializeField] public List<Rig> rig_list;
    [SerializeField] float aimLerpSpeed;
    [SerializeField] float sprintLerpSpeed;
    public int idle_layer = 0, aim_layer = 1, sprint_layer = 2, holster_layer = 3;
    public GameObject leftHandIK_target;
    public GameObject rightHandIK_target;
    public List<TwoBoneIKConstraint> fingerBones_left;
    public WeaponBase weaponPrefab;
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
    float aim_flerp_elapsed, sprint_flerp_elapsed;


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

        if (ArmRight(isArmedRight)) Debug.Log("");
        else Debug.Log("running...");
        if (ArmLeft(isArmedLeft)) Debug.Log("");
        else Debug.Log("running...");


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
        //-------------------------testing


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


// void ArmRight(bool state)
// {
//     fbbik.solver.rightArmMapping.weight = Mathf.Lerp(fbbik.solver.rightArmMapping.weight, state ? 1 : 0, Time.deltaTime * lerpSpeed);
//     rightHandRig.weight = Mathf.Lerp(rightHandRig.weight, state ? 1 : 0, Time.deltaTime * lerpSpeed);
// }

// void ArmLeft(bool state)
// {
//     fbbik.solver.leftArmMapping.weight = Mathf.Lerp(fbbik.solver.leftArmMapping.weight, state ? 1 : 0, Time.deltaTime * lerpSpeed);
//     leftHandRig.weight = Mathf.Lerp(leftHandRig.weight, state ? 1 : 0, Time.deltaTime * lerpSpeed);
// }
//------------------------------------------------------------------------------------ testing
// void ArmRight(bool state)
// {
//     if (armedRight()) { Debug.Log("armed Right"); return; }
//     else ArmRightLerpTimer = 0;
//     bool armedRight()
//     {
//         Debug.Log("arming Right");
//         if (ArmRightLerpTimer > 1.0f) return true;
//         ArmRightLerpTimer += Time.deltaTime * armingMultiplier;
//         fbbik.solver.rightArmMapping.weight = Mathf.Lerp(fbbik.solver.rightArmMapping.weight, state ? 1 : 0, ArmRightLerpTimer);
//         rightHandRig.weight = Mathf.Lerp(rightHandRig.weight, state ? 1 : 0, ArmRightLerpTimer);
//         return false;
//     }
// }

// void ArmLeft(bool state)
// {
//     if (armedLeft()) { Debug.Log("armed Left"); return; }
//     else ArmLeftLerpTimer = 0;
//     bool armedLeft()
//     {
//         Debug.Log("arming Left");
//         if (ArmLeftLerpTimer > 1.0f) return true;
//         ArmLeftLerpTimer += Time.deltaTime * armingMultiplier;
//         fbbik.solver.leftArmMapping.weight = Mathf.Lerp(fbbik.solver.leftArmMapping.weight, state ? 1 : 0, ArmLeftLerpTimer);
//         leftHandRig.weight = Mathf.Lerp(leftHandRig.weight, state ? 1 : 0, ArmLeftLerpTimer);
//         return false;
//     }
// }
//------------------------------------------------------------------------------------ testing