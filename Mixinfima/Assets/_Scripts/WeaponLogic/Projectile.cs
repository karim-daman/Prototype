using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{

    Vector3 shootDirection;
    float bulletVelocity;
    [SerializeField] float lifeSpan;
    Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void Setup(Vector3 direction, float velocity)
    {
        shootDirection = direction;
        bulletVelocity = velocity;
        // Debug.Break();
    }

    private void FixedUpdate()
    {
        if (rb == null) this.gameObject.AddComponent<Rigidbody>();
        rb.AddForce(shootDirection * bulletVelocity, ForceMode.Impulse);
        // rb.velocity += shootDirection * bulletVelocity;
        Destroy(gameObject, lifeSpan);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Target")
        {
            // Destroy(gameObject);
            other.GetComponent<Target>().TakeDamage();



        }
    }






}
