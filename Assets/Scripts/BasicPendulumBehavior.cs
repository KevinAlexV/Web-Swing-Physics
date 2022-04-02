using UnityEngine;

public class BasicPendulumBehavior : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField]
    private Transform pivot;
    [SerializeField]
    private Transform bob;

    [Header("Position Data")]
    [SerializeField]
    private Vector3 startingPosition;

    [Header("Variables")]
    [SerializeField]
    private float stringLength = 3.0f;
    [SerializeField]
    private float bobMass = 1.0f;
    
    [Header("Forces")]
    [SerializeField]
    private float gravityForce = 0.0f;
    [SerializeField]
    private Vector3 gravityDirection;
    [SerializeField]    
    private float tensionForce = 0.0f;
    [SerializeField]
    private Vector3 tensionDirection;
    [Space]
    [SerializeField]
    private Vector3 velocityVector = new Vector3();

    private Vector3 bobDistanceVector;

    private void Start()
    { 
        startingPosition = bob.transform.position;

        ResetForces();
    }

    private void FixedUpdate()
    {
        Vector3 gravityVector = CalculateGravityForce();

        // Apply gravity to the velocity vector
        velocityVector += gravityVector * Time.deltaTime;

        Vector3 movement = velocityVector * Time.deltaTime;
        float distanceAfterGravity = Vector3.Distance(pivot.position, bob.position + movement);

        if (distanceAfterGravity > stringLength || Mathf.Approximately(distanceAfterGravity, stringLength))
        {
            // Calculate bob offsets
            bobDistanceVector = bob.position - pivot.position;

            Vector3 tensionForce = CalculateTensionForce();

            // Account for rotational acceleration on the pendulum (Ft = Ft + Fc)
            tensionForce += CalculateCentripetalForce();

            // Apply tension to the velocity vector 
            velocityVector += tensionForce * Time.deltaTime;
        }

        /**
         * Note on normalizing vectors:
         *   formula = sqrt(x^2 + y^2 + z^2) 
         *   - Resulting vector has a length a 1
         *   - Vector retains positional data
         *
         * Variables:
         *   - correctWebLength: Raw distance/length the web should be
         *   - vec3WebLength: Vector containing length data of each axis 
         *
         * Set bob's position to be the pivot position offset by the web's length
         *   - Dispersing directional data (Normalize) to the web length
         */

        // Retrieve bob's position with velocity vector applied
        Vector3 newBobPosition = bob.position + (velocityVector * Time.deltaTime);

        // Calculate the raw length of the new web from the transformed position
        float newWebLength = Vector3.Distance(pivot.position, newBobPosition);

        float correctWebLength = newWebLength <= stringLength ? newWebLength : stringLength;
        Vector3 vec3WebLength = newBobPosition - pivot.position;

        bob.position = pivot.position + (correctWebLength * Vector3.Normalize(vec3WebLength));
    }

    private Vector3 CalculateGravityForce()
    {
        // F = m * g
        gravityForce = bobMass * Physics.gravity.magnitude;
        gravityDirection = Physics.gravity.normalized;

        return gravityForce * gravityDirection;
    }

    private Vector3 CalculateTensionForce()
    {
        tensionDirection = (pivot.position - bob.position).normalized;

        /** 
         * Account for gravity:
         *  Tension changes during the bob's arc due to gravity
         *  Calculate the theta between the string and gravity
         *                                                      /*
         *                                                     / |
         *                                                    /  |
         *                                                   /   |
         *                                                  /    |
         * ex: Visualise (/ = string, | = gravity     ->   0--x--|
         *                0 = bob, * = axis, x = theta)
         */

        float theta = Vector3.Angle(bobDistanceVector, gravityDirection);

        // Ft = mg * cos(theta)
        tensionForce = bobMass * Physics.gravity.magnitude * Mathf.Cos(Mathf.Deg2Rad * theta);

        return tensionForce * tensionDirection;
    }

    private Vector3 CalculateCentripetalForce() 
    {
        Vector3 centripetalForce = Vector3.zero;

        // Fc = (m * v^2) / r
        centripetalForce.x = ((bobMass * Mathf.Pow(velocityVector.x, 2)) / stringLength);
        centripetalForce.y = ((bobMass * Mathf.Pow(velocityVector.y, 2)) / stringLength);
        centripetalForce.z = ((bobMass * Mathf.Pow(velocityVector.z, 2)) / stringLength);

        return centripetalForce;
    }

    [ContextMenu("Reset Position")]
    private void ResetPosition() => bob.position = startingPosition;

    [ContextMenu("Reset Forces")]
    private void ResetForces() => velocityVector = Vector3.zero;

    /**
     * Gizmos Reference
     *
     * - White: Web
     * - Yellow: Gravity
     * - Orange: Tension
     * - Red: Resulting Force
     */
    void OnDrawGizmos()
    {
        float rayScaleFactor = 0.3f;

        Gizmos.color = new Color(0.5f, 0.0f, 0.5f);
        Gizmos.DrawWireSphere(pivot.position, stringLength);

        // White: Web
        Gizmos.color = new Color(0.85f, 0.95f, 0.9f);
        Vector3 web = stringLength * new Vector3(0.0f, 1.0f, 0.0f);
        Gizmos.DrawRay(bob.position, web);

        // Yellow: Gravity
        Gizmos.color = new Color(1.0f, 1.0f, 0.1f);
        Vector3 gravity = rayScaleFactor * gravityForce * gravityDirection;
        Gizmos.DrawRay(bob.position, gravity);

        // Orange: Tension
        Gizmos.color = new Color(1.0f, 0.5f, 0.2f);
        Vector3 tension = rayScaleFactor * tensionForce * tensionDirection;
        Gizmos.DrawRay(bob.position, tension);

        // Red: Resulting Force
        Gizmos.color = new Color(1.0f, 0.3f, 0.3f);
        Vector3 result = gravity + tension;
        Gizmos.DrawRay(bob.position, result);
    }
}
