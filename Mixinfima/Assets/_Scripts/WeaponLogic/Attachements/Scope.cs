using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope : MonoBehaviour
{
    [SerializeField] GameObject camScopeOffset;
    public GameObject GetCamOffset() => camScopeOffset;

}
