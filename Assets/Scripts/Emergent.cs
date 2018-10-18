using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emergent : MonoBehaviour {
    public int amountFollowing;
    public int depth;
    public GameObject forwardUnit;
    public GameObject backLeft;
    public GameObject backRight;
    public Rigidbody2D rb;
    public bool canFollow;
    public bool destructable;
    public bool isLeft;
    public GameObject[] allBoids;
    public float seperationDistance;
    public float seperationAngle;
    public Vector2 destination;

    public float maxAcceleration;
    public float maxVelocity;
    public float slowRadius;
    public float timeToTarget;

    // Use this for initialization
    void Start () {
        allBoids = GameObject.FindGameObjectsWithTag("boid");
        rb = this.GetComponent<Rigidbody2D>();
        if (canFollow) {
            depth = int.MaxValue;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (canFollow&& forwardUnit == null) {
            var minLayer = int.MaxValue;
            var f = this.gameObject;
            foreach (GameObject g in allBoids) {
                if (g != this.gameObject && g.GetComponent<Emergent>().depth < minLayer && PositionOpen(g))
                {
                    minLayer = g.GetComponent<Emergent>().depth;
                   
                    f = g;
                }

            }
            int t = f.GetComponent<Emergent>().RequestEntry(this.gameObject);
            this.depth = f.GetComponent<Emergent>().depth + 1;
            if (t == 1)
            {
                this.forwardUnit = f;
                isLeft = true;
            }
            else
            {
                this.forwardUnit = f;
                isLeft = false;
            }
        }
        if (forwardUnit != null && canFollow)
        {
            var tempVect = new Vector3();
            if (isLeft)
            {
                tempVect = -forwardUnit.transform.right*seperationAngle;
            }
            else {
                tempVect = forwardUnit.transform.right * seperationAngle;
            }
            destination = forwardUnit.transform.position - forwardUnit.transform.up *seperationDistance + tempVect;
            
            Vector2 formationAccelerationSeek = DynamicSeek(transform.position, destination);
            Vector2 formationAccelerationArrive = DynamicArrive(this.transform.position, destination, rb.velocity);
            Vector2 acceleration = formationAccelerationSeek + formationAccelerationArrive;
            if (acceleration.magnitude > maxAcceleration)
            {
                acceleration = acceleration.normalized * maxAcceleration;
            }

            rb.velocity += acceleration;

            if (rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }
            transform.rotation = forwardUnit.transform.rotation;
        }
        else if (!canFollow)
        {
            //pathfind
        }
	}

    public int RequestEntry(GameObject g) {

        if (this.backLeft == null)
        {
            this.backLeft = g;
            g.GetComponent<Emergent>().forwardUnit = this.gameObject;
            return 1;
        }
        else if (this.backRight == null)
        {
            this.backRight = g;
            g.GetComponent<Emergent>().forwardUnit = this.gameObject;
            return 2;
        }
        else return 0;
    }
    Vector2 DynamicSeek(Vector3 position, Vector3 target)
    {
        Vector2 linearAcc = target - position;
        return maxAcceleration * linearAcc;

    }
    bool PositionOpen(GameObject g) {
        if (g.GetComponent<Emergent>().isLeft) {
            if (g.GetComponent<Emergent>().backLeft == null)
            {
                return true;
            }
            else
            {
                return false;

            }
        }
        else if (g.GetComponent<Emergent>().backLeft == null || g.GetComponent<Emergent>().backRight == null) {
            return true;
        }
        return false;
    }
    Vector2 DynamicArrive(Vector3 position, Vector3 target, Vector2 currentVelocity)
    {
        Vector2 SeekAcceleration = DynamicSeek(position, target);
        float targetSpeed = maxVelocity * (Vector3.Distance(position, target) / slowRadius);
        Vector2 directionVector = (target - position).normalized;
        Vector2 targetVelocity = directionVector * targetSpeed;
        Vector2 acceleration = (targetVelocity - currentVelocity) / timeToTarget;
        return maxAcceleration * acceleration;
    }
}
