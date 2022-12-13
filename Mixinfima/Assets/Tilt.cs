using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tilt : MonoBehaviour
{
    Rigidbody rb;
    private void Awake() => rb = GetComponent<Rigidbody>();

    void Update()
    {
        transform.localRotation = TiltCharacterTowardsVelocity(transform.localRotation, rb.velocity, 25);
    }

    Quaternion TiltCharacterTowardsVelocity(Quaternion cleanRotation, Vector3 vel, float velMagFor45Degree)
    {
        Vector3 rotAxis = Vector3.Cross(Vector3.up, vel);
        float tiltAngle = Mathf.Atan(vel.magnitude / velMagFor45Degree) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(tiltAngle, rotAxis) * cleanRotation;  //order matters
    }
}
