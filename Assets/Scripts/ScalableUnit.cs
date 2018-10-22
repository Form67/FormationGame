using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableUnit : Movement
{
    [Header("Weights")]
    public float formationWeightSeek;
    public float formationWeightArrive;
    public float coneWeight;

    [Header("Cone Check")]
    public float radius;

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
    

    public void SetTarget(Vector3 target, float zRotate = 0f) // zRotate is in degrees
    {
        // Blend between dynamic seek and arrive
        Vector2 formationAccelerationSeek = formationWeightSeek * DynamicSeek(transform.position, target);
        Vector2 formationAccelerationArrive = formationWeightArrive * DynamicArrive(transform.position, target, rb.velocity);
        Vector2 coneCheckAvoid = coneWeight * ConeCheck();
        Vector2 acceleration = formationAccelerationSeek + formationAccelerationArrive;

        if (coneCheckAvoid != Vector2.zero)
            acceleration = acceleration * (1-coneWeight) + coneCheckAvoid;

        UpdateKinematics(acceleration, zRotate);
    }
    
    void UpdateKinematics(Vector2 acceleration, float zRotate = 0f) // zRotate is in degrees
    {
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

    Vector2 ConeCheck()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        Vector2 forward = rb.velocity.normalized;

        // Cone check
        foreach (Collider2D collider in colliders)
        {
            Vector3 targetDirection = collider.transform.position - transform.position;
            float angle = Vector2.Angle(targetDirection, forward);
            if (angle < 120 && collider.gameObject.tag == "Wall")
            {
                return DynamicEvade(transform.position, collider.gameObject.transform.position);
            }
        }

        return Vector2.zero;
    }
    

    public void DestroySelf()
    {
        manager.RemoveBoid(gameObject);
        Destroy(gameObject);
    }
}
