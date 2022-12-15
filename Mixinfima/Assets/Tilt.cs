using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tilt : MonoBehaviour
{
    Rigidbody rb;


    void Update()
    {
        // direction = new Vector3(Input.GetAxis(H), 0, Input.GetAxis(V));
    }
    private void FixedUpdate()
    {
        transform.localRotation = TiltCharacterTowardsVelocity(transform.localRotation, rb.velocity, 25);
    }

    Quaternion TiltCharacterTowardsVelocity(Quaternion cleanRotation, Vector3 vel, float maxAngle)
    {
        Vector3 rotAxis = Vector3.Cross(Vector3.up, vel);
        float tiltAngle = Mathf.Atan(vel.magnitude / maxAngle) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(tiltAngle, rotAxis) * cleanRotation;  //order matters
    }
}
