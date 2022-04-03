using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Based on Usain Bolt's avg speed of  37.58 km/h, 37580 m/h, 626.33333333 m/m, 10.4388888889 m/s
//https://en.wikipedia.org/wiki/Footspeed


public class RunningBehavior : MonoBehaviour
{
    [SerializeField]
    [Range(0.0f, 10.4f)]
    private float RunningSpeed;

    [Header("Position Data")]
    [SerializeField]
    private Vector3 startingPosition;

    [Header("Variables")]
    [SerializeField]
    private float Mass = 1.0f;

    [Header("Forces")]
    [SerializeField]
    private float gravityForce = 0.0f;
    [SerializeField]
    private Vector3 gravityDirection;
    [Space]
    [SerializeField]
    private Vector3 velocityVector = new Vector3();
    [SerializeField]
    private Vector3 netForceVector = new Vector3();

    private List<Vector3> forceVectorList = new List<Vector3>();


    void FixedUpdate()
    {

        //forceVectorList.Add(CalculateGravityForce());
        Vector3 runningForce = (CalculateRunningForce());
        //Debug.Log(runningForce.x + " | Velo: " + velocityVector.x + " | Run: " + RunningSpeed);
        forceVectorList.Add(runningForce);

        CalculateNetForces();

        transform.position += velocityVector * Time.deltaTime;
    }

    private Vector3 CalculateRunningForce()
    {

        if ((velocityVector.x < RunningSpeed) && (velocityVector.x <= RunningSpeed))
        {
            return new Vector3(velocityVector.x + .01f, 0, 0);
        }
        else if (RunningSpeed - velocityVector.x <= RunningSpeed)
        {
            return new Vector3(RunningSpeed - velocityVector.x, 0, 0);
        }
        else
            return Vector3.zero;
    }

    private Vector3 CalculateGravityForce()
    {
        // F = m * g
        gravityForce = Mass * Physics.gravity.magnitude;
        gravityDirection = Physics.gravity.normalized;

        return gravityForce * gravityDirection * Time.deltaTime;
    }

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
        Vector3 accelerationVector = (netForceVector / Mass);
        velocityVector += accelerationVector;
    }
}