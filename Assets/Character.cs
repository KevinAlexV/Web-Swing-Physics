using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public Vector3 velocity;
    public float gravity = 20f;
    public Vector3 gravityDirection = new Vector3(0,1,0);
    public float drag;
    public float maxSpeed;

    private Vector3 dampingDirection;

    public void ApplyGravity()
    {
        velocity -= gravityDirection * gravity * Time.deltaTime;
    }

    public void ApplyDamping() {

        dampingDirection = -velocity;
        dampingDirection *= drag;

        dampingDirection += dampingDirection;

    }

    public void ApplyMaxSpeedCap()
    {
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);




    }
}
