using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class DontGoThroughThings : MonoBehaviour
{
    public LayerMask layerMask;
    private Vector3 previousPosition;
    private Vector3 currentPositionRayTo;
    private Rigidbody rb;
    private RaycastHit hit;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        previousPosition = rb.position;
    }

    void FixedUpdate()
    {
        currentPositionRayTo = (transform.position - previousPosition);
        if (Physics.Raycast(previousPosition, currentPositionRayTo, out hit, Vector3.Distance(transform.position, previousPosition), layerMask.value))
        {
            // we hit something, do stuff here (for example stop the force/speed/velocity, otherwise we would keep moving..)

            if (hit.collider.GetComponentInParent<Target>() != null)
            {
                // float duration = Time.time - timeMouseButtonDown;
                // float forcePercent = duration / maxForce;
                // float magnitude = Mathf.Lerp(1, maxForce, forcePercent);

                Vector3 direction = hit.collider.GetComponentInParent<Target>().transform.position - transform.position;
                // currentPositionRayTo.y = 1;
                // currentPositionRayTo.Normalize();

                Vector3 force = -direction * 10;
                hit.collider.GetComponentInParent<RagDollBehaviour>().TriggerRagdoll(force, hit.point);
            }

            //constantForce.enabled = false;
            rb.position = hit.point;
            Destroy(gameObject);
        }
        previousPosition = rb.position;
    }
}

//https://github.com/omgwtfgames/unity-bowerbird/blob/master/Scripts/General/DontGoThroughThings.cs