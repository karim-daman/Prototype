using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Body body;
    public void TakeDamage()
    {
        body.hits += 1;
    }



}
