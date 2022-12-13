using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondOrderDynamics : MonoBehaviour
{
    [SerializeField] GameObject follower;
    [SerializeField] float frequency = 1, damp = .5f, response = 2;
    SecondOrderClass motion;

    private void Awake()
    {
        motion = new SecondOrderClass(frequency, damp, response, transform.position);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W)) transform.position += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.S)) transform.position += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.A)) transform.position += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.D)) transform.position += new Vector3(1, 0, 0);
        if (Input.GetKey(KeyCode.Space)) transform.position += new Vector3(0, 1, 0);
        if (Input.GetKey(KeyCode.LeftControl)) transform.position += new Vector3(0, -1, 0);

        follower.transform.position = motion.Update(Time.deltaTime, transform.position, new Vector3());
    }

}
