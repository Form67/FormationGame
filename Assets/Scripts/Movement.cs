using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    [Header("Basic Movement")]
    public float maxAcceleration;
    public float maxVelocity;

    [Header("Dynamic Arrive")]
    public float slowRadius;
    public float timeToTarget;

    protected Vector2 DynamicSeek(Vector3 position, Vector3 target)
    {
        Vector2 linearAcc = target - position;
        return maxAcceleration * linearAcc.normalized;
    }

    protected Vector2 DynamicEvade(Vector3 position, Vector3 target)
    {
        Vector2 linearAcc = position - target;
        return maxAcceleration * linearAcc.normalized;
    }

    protected Vector2 DynamicArrive(Vector3 position, Vector3 target, Vector2 currentVelocity)
    {
        float targetSpeed = maxVelocity * (Vector3.Distance(position, target) / slowRadius);
        Vector2 directionVector = (target - position).normalized;
        Vector2 targetVelocity = directionVector * targetSpeed;
        Vector2 acceleration = (targetVelocity - currentVelocity) / timeToTarget;
        return maxAcceleration * acceleration;
    }

    // Orientation is expected to be in degrees
    public Vector2 OrientToVector(float orientation)
    {
        Vector2 orient = new Vector2(-Mathf.Sin(orientation * Mathf.Deg2Rad), Mathf.Cos(orientation * Mathf.Deg2Rad));
        return orient.normalized;
    }
}
