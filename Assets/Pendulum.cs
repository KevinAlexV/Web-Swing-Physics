using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pendulum { 

    public Transform chr_trasnform;
    public Tether tether;
    public Arm arm;
    public Character character;

    private Vector3 previousPosition;
 
    public void Initialize()
    {

        chr_trasnform.parent = tether.tether_tr;
        arm.length = Vector3.Distance(chr_trasnform.transform.localPosition, tether.position);

    }

    public Vector3 moveChar(Vector3 pos, float time)
    {
        character.velocity += getConstrainedVelo(pos, previousPosition, time);

        character.ApplyGravity();
        character.ApplyDamping();

        pos += character.velocity * time;

        if (Vector3.Distance(pos, tether.position) < arm.length)
        { 
            pos = Vector3.Normalize(pos - tether.position) * arm.length;
            arm.length = (Vector3.Distance(pos, tether.position));
            return pos;
        }

        previousPosition = pos;

        return pos;
    }

    public Vector3 getConstrainedVelo(Vector3 currentPos, Vector3 previousPos, float time)
    {
        float distanceToTether;
        Vector3 constrainedPosition;
        Vector3 predictedPosition;

        distanceToTether = Vector3.Distance(currentPos, tether.position);

        if(distanceToTether > arm.length)
        { 
            constrainedPosition = Vector3.Normalize(currentPos - tether.position) * arm.length;
            predictedPosition = (constrainedPosition - previousPos) / time;

            return predictedPosition;
        }

        return Vector3.zero;
    }

}
