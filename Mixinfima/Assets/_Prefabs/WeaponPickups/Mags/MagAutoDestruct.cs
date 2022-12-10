using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagAutoDestruct : MonoBehaviour
{
    [SerializeField] float timer;
    new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rigidbody == null) return;
        if (!rigidbody.isKinematic) Destroy(gameObject, timer);
    }
}
