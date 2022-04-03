using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPendulumBehavior : MonoBehaviour
{
    [Header("GameObjects")]
    public Transform pivot;
    public Transform bob;

    [Header("Position Data")]
    [SerializeField]
    private Vector3 startingPosition;

    [Header("Variables")]
    [SerializeField]
    private float stringLength = 3.0f;
    [SerializeField]
    private float mass = 1.0f;
    
    [Header("Forces")]
    [SerializeField]
    private float gravityForce = 0.0f;
    [SerializeField]
    private Vector3 gravityForceVector;
    [SerializeField]
    private Vector3 gravityDirection;
    [SerializeField]    
    private float tensionForce = 0.0f;
    [SerializeField]
    private Vector3 tensionDirection;
    [SerializeField]
    private Vector3 tensionForceVector;
    [Space]
    [SerializeField]
    private Vector3 velocityVector = new Vector3();
    
    private Vector3 netForceVector;  // N [kg m s^-2]
    private List<Vector3> forces = new List<Vector3>();

    private Vector3 currentPosition;

    private void Start()
    { 
        startingPosition = bob.transform.position;
        currentPosition = bob.transform.position;

        ResetForces();
    }

    private void FixedUpdate()
    {
        currentPosition = PendulumUpdate();

        bob.position = currentPosition;
    }

    private Vector3 PendulumUpdate()
    {
        // Apply gravity to the velocity vector
        gravityForceVector = CalculateGravityForce();

        forces.Add(gravityForceVector);
        velocityVector += gravityForceVector;

        Vector3 auxillaryMovementVector = gravityForceVector * Time.fixedDeltaTime;
        float distanceAfterGravity = Vector3.Distance(pivot.position, bob.position + auxillaryMovementVector);

        if (distanceAfterGravity > stringLength || Mathf.Approximately(distanceAfterGravity, stringLength))
        {
            // Calculate bob offsets
            tensionDirection = (pivot.position - bob.position).normalized;

            // Sets local tension force
            CalculateTensionForce();

            // Account for rotational acceleration on the pendulum (Ft = Ft + Fc)
            tensionForce += CalculateCentripetalForce();

            // Apply tension to the velocity vector 
            tensionForceVector = tensionDirection * tensionForce * Time.fixedDeltaTime;

            forces.Add(tensionForceVector);
            velocityVector += tensionForceVector;
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

        //CalculateNetForces();

        // Store the bob's new position
        Vector3 bobNewPosition = bob.position + (velocityVector * Time.fixedDeltaTime);

        // Calculate the raw length of the new web from the transformed position
        float newWebLength = Vector3.Distance(pivot.position, bobNewPosition);

        float correctWebLength = newWebLength <= stringLength ? newWebLength : stringLength;

        // end - start
        Vector3 vec3WebLength = bobNewPosition - pivot.position;

        return pivot.position + (correctWebLength * Vector3.Normalize(vec3WebLength));
    }

    private void CalculateNetForces()
    {
        // Sum the forces and clear the list
        netForceVector = Vector3.zero;
        foreach (Vector3 forceVector in forces)
        {
            netForceVector = netForceVector + forceVector;
        }
        forces = new List<Vector3>();

        // Calculate position change due to net force
        Vector3 accelerationVector = netForceVector;// / mass;
        velocityVector += accelerationVector * Time.deltaTime;

    }

    private Vector3 CalculateGravityForce()
    {
        // F = m * g
        gravityForce = mass * Physics.gravity.magnitude;
        gravityDirection = Physics.gravity.normalized;

        return gravityDirection * gravityForce * Time.fixedDeltaTime;
    }

    private void CalculateTensionForce()
    {
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

        float theta = Vector3.Angle(bob.position - pivot.position, gravityDirection);

        // Ft = mg * cos(theta)
        tensionForce = mass * Physics.gravity.magnitude * Mathf.Cos(Mathf.Deg2Rad * theta);
    }

    private float CalculateCentripetalForce() 
    {
        float centripetalForce = 0.0f;

        // Fc = (m * v^2) / r
        centripetalForce = ((mass * Mathf.Pow(velocityVector.magnitude, 2)) / stringLength);

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
        Vector3 web = pivot.position - bob.position;
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
