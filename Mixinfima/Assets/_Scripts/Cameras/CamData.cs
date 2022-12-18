using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Camera", menuName = "Create Cam Data", order = 2)]
public class CamData : ScriptableObject
{


    [Header("Stand_Idle")]

    public Vector3 Stand_Idle_up;
    public Vector3 Stand_Idle_forward;
    public Vector3 Stand_Idle_down;


    [Header("Stand_Aim")]

    public Vector3 Stand_Aim_up;
    public Vector3 Stand_Aim_forward;
    public Vector3 Stand_Aim_down;


    [Header("Crouch_Idle")]

    public Vector3 Crouch_Idle_up;
    public Vector3 Crouch_Idle_forward;
    public Vector3 Crouch_Idle_down;


    [Header("Crouch_Aim")]

    public Vector3 Crouch_Aim_up;
    public Vector3 Crouch_Aim_forward;
    public Vector3 Crouch_Aim_down;



}
