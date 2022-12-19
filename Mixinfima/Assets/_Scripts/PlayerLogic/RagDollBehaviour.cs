using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RootMotion.FinalIK;
using UnityEngine;

public class RagDollBehaviour : MonoBehaviour
{

    [SerializeField] Inventory inventory;
    [SerializeField] IKController iKController;
    [SerializeField] Animator animator;
    [SerializeField] FullBodyBipedIK fullBodyBipedIK;

    [SerializeField] List<GameObject> ragdollObjects;

    CharacterController characterController;
    PlayerController playerController;
    [SerializeField] List<Rigidbody> ragdoll_bodies;
    [SerializeField] List<Collider> ragdoll_Colliders;
    [SerializeField] Body body;


    private void Awake()
    {

        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();

        for (int i = 0; i < ragdollObjects.Count; i++)
        {
            ragdoll_bodies.Add(ragdollObjects[i].GetComponent<Rigidbody>());
            ragdoll_Colliders.Add(ragdollObjects[i].GetComponent<Collider>());
        }

        for (int i = 0; i < ragdollObjects.Count; i++) ragdoll_Colliders[i].isTrigger = true;

    }


    void Update()
    {
        if (body.hits > 3) Die();
        // if (Input.GetKey(KeyCode.K)) Die();
    }
    void Die()
    {

        if (inventory.GetEquippedIndex() != -1) inventory.DropWeapon();
        for (int i = 0; i < ragdollObjects.Count; i++) ragdoll_bodies[i].isKinematic = false;

        fullBodyBipedIK.enabled = false;
        characterController.enabled = false;
        playerController.enabled = false;
        animator.enabled = false;
    }

    void Alive()
    {
        for (int i = 0; i < ragdollObjects.Count; i++) ragdoll_bodies[i].isKinematic = true;

        fullBodyBipedIK.enabled = true;
        characterController.enabled = true;
        playerController.enabled = true;
        animator.enabled = true;
    }

    public void TriggerRagdoll(Vector3 force, Vector3 hitPoint)
    {
        Die();
        Rigidbody hitRigidbody = ragdoll_bodies.OrderBy(rigidbody => Vector3.Distance(rigidbody.position, hitPoint)).First();
        hitRigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
}
