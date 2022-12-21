using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{

    int hit;

    [SerializeField] Body body;


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            Destroy(other.gameObject);
            Debug.Log(++body.hits);
        }
    }

}
