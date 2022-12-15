using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleMovement : MonoBehaviour
{

    [SerializeField] float speed, leanAmount, leanSpeed;
    [SerializeField] Transform pivot;
    CharacterController controller;
    Vector3 move;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        // pivot.transform.rotation = Quaternion.RotateTowards(pivot.transform.rotation, Quaternion.Euler(new Vector3(move.z, 0, -move.x) * leanAmount * controller.velocity.magnitude), Time.deltaTime * leanSpeed);
        // controller.Move(move * speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        controller.Move(move);
    }
}
