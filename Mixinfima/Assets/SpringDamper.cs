using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringDamper : MonoBehaviour
{
    [SerializeField] LayerMask layer;
    [SerializeField] GameObject rayOrigin;
    [SerializeField] GameObject restingPoint;

    Rigidbody rb;

    [SerializeField] float rayLength;
    [SerializeField] float offset;

    [SerializeField] float force, strength, velocity, damp;
    [SerializeField] float jumpPressure;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }



    void Update()
    {
        Ray ray = new Ray(rayOrigin.transform.position, -Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayLength, layer))
        {

            Debug.DrawRay(rayOrigin.transform.position, -Vector3.up * rayLength, Color.red);
            print($" hitPoint {hit.point.y} | plane.y {restingPoint.transform.position.y}");

            Vector3 worldVel = rb.GetPointVelocity(restingPoint.transform.position);

            offset = hit.point.y - restingPoint.transform.position.y;

            velocity = Vector3.Dot(Vector3.up, worldVel);

            force = (offset * strength) - (velocity * damp);

            rb.AddForceAtPosition(transform.up * force, transform.position);

            if (Input.GetKey(KeyCode.Space))
            {
                jumpPressure += Time.deltaTime * 1000;
                jumpPressure = Mathf.Clamp(jumpPressure, 0, 500);
                rb.AddForce(-transform.up * 10, ForceMode.Acceleration);
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                rb.AddForce(transform.up * jumpPressure, ForceMode.Impulse);
                jumpPressure = 0;
            }
        }
        else
        {
            Debug.DrawRay(rayOrigin.transform.position, -Vector3.up * rayLength, Color.green);
        }




    }
}
