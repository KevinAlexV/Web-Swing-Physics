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
    [SerializeField]
    private Vector3 netForceVector = new Vector3();

    private List<Vector3> forceVectorList = new List<Vector3>();
    private Vector3 bobDistanceVector;
    private bool isOnPendulum = false, applyTension = false;


    //Initialize basic pendulum behavior.
    private void Start()
    {
        startingPosition = bob.transform.position;

        isOnPendulum = true;

        ResetForces();
    }

    //Per physics calculation
    private void FixedUpdate()
    {
        Vector3 gravityVector = CalculateGravityForce();

        // Add gravity to the net forces
        forceVectorList.Add(gravityVector);

        //How far has it moved since velocity, which is only affected by gravity at the moment. Later, probably should be affected by net forces at this point.
        float distanceAfterGravity = Vector3.Distance(pivot.position, bob.position + gravityVector);

        if ((distanceAfterGravity > stringLength || Mathf.Approximately(distanceAfterGravity, stringLength)) && isOnPendulum)
        {
            // Calculate bob offsets
            bobDistanceVector = bob.position - pivot.position;

            applyTension = true;
        }
        else { applyTension = false; }


        CalculateNetForces();

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
        Vector3 newBobPosition = bob.position + velocityVector;

        // Calculate the raw length of the new web from the transformed position
        float newWebLength = Vector3.Distance(pivot.position, newBobPosition);

        //Calculate the correct web length, based on if the current length is longer than the max length.
        float correctWebLength = newWebLength <= stringLength ? newWebLength : stringLength;
        Vector3 vec3WebLength = newBobPosition - pivot.position;

        //Calculate bobs position by going from the pivot position, and adding the web's position
        bob.position = pivot.position + (correctWebLength * Vector3.Normalize(vec3WebLength));
    }

    //Calculate all forces acting on the object
    private void CalculateNetForces()
    {
        // Sum the forces and clear the list
        netForceVector = Vector3.zero;
        foreach (Vector3 forceVector in forceVectorList)
        {
            netForceVector = netForceVector + forceVector;
        }
        forceVectorList = new List<Vector3>();

        // Calculate position change due to net force
        Vector3 accelerationVector = (netForceVector / bobMass);
        velocityVector += accelerationVector * Time.deltaTime;


        //If the object isOnPendulum, account for tension and cent. force
        if (isOnPendulum && applyTension)
        {
            Vector3 tensionForce = CalculateTensionForce();

            // Account for rotational acceleration on the pendulum (Ft = Ft + Fc), when pendulum is moving, and not initial pendulum calcs.
            tensionForce += CalculateCentripetalForce(velocityVector);

            // Apply tension to the netforce vector 
            netForceVector += tensionForce;

            Vector3 tensionAcceleration = tensionForce / bobMass;
            velocityVector += tensionAcceleration * Time.deltaTime;
        }
    }


    //Calculate gravity at the current time, and return vector3 containing gravity.
    private Vector3 CalculateGravityForce()
    {
        // F = m * g
        gravityForce = bobMass * Physics.gravity.magnitude;
        gravityDirection = Physics.gravity.normalized;

        return gravityForce * gravityDirection * Time.deltaTime;
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

        float cosTheta;
    
        if (theta != 90)
            cosTheta = Mathf.Cos(Mathf.Deg2Rad * theta);
        else
            cosTheta = 0;

        // Ft = mg * cos(theta)
        tensionForce = bobMass * Physics.gravity.magnitude * cosTheta;

        return tensionForce * tensionDirection * Time.deltaTime;
    }

    private Vector3 CalculateCentripetalForce(Vector3 velocity)
    {
        Vector3 centripetalForce = Vector3.zero;

        // Fc = (m * v^2) / r
        centripetalForce.x = ((bobMass * Mathf.Pow(velocity.x, 2)) / stringLength);
        centripetalForce.y = ((bobMass * Mathf.Pow(velocity.y, 2)) / stringLength);
        centripetalForce.z = ((bobMass * Mathf.Pow(velocity.z, 2)) / stringLength);

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
