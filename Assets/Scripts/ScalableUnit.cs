using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableUnit : Movement
{
    [Header("Formation")]
    public float formationWeightSeek;
    public float formationWeightArrive;

    Rigidbody2D rb;

    // Use this for initialization
    void Awake () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {

		
	}

    public void SetTarget(Vector3 target, Vector3 directionFromRadiusVector)
    {
        // Blend between dynamic seek and arrive
        Vector2 formationAccelerationSeek = formationWeightSeek * DynamicSeek(transform.position, target);
        Vector2 formationAccelerationArrive = formationWeightArrive * DynamicArrive(transform.position, target, rb.velocity);
        Vector2 acceleration = formationAccelerationSeek + formationAccelerationArrive;

        // Cap acceleration
        if (acceleration.magnitude > maxAcceleration)
        {
            acceleration = acceleration.normalized * maxAcceleration;
        }

        // Apply acceleration
        rb.velocity += acceleration;

        // Cap velocity
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }

        // Adjust orientation
        float unitRotation = Mathf.Atan2(-directionFromRadiusVector.x, directionFromRadiusVector.y);
        transform.eulerAngles = new Vector3(0, 0, unitRotation * Mathf.Rad2Deg);
    }
}
