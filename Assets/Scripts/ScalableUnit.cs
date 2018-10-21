using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableUnit : Movement
{
    [Header("Formation")]
    public float formationWeightSeek;
    public float formationWeightArrive;

    ScalableManager manager;
    Rigidbody2D rb;
    

    // Use this for initialization
    void Awake () {
        rb = GetComponent<Rigidbody2D>();
	}

    void Start()
    {
        manager = FindObjectOfType<ScalableManager>();
    }

    // Update is called once per frame
    void FixedUpdate () {

		
	}



    // zRotate is in degrees
    public void SetTarget(Vector3 target, float zRotate = 0f)
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
        
        //Adjust orientation
        transform.eulerAngles = new Vector3(0, 0, zRotate);
    }
    

    public void DestroySelf()
    {
        manager.RemoveBoid(gameObject);
        Destroy(gameObject);
    }
}
