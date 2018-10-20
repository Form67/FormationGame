using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emergent : MonoBehaviour {
    public GameObject[] path;
    public bool arrived;
    public float closeEnoughDistance;
    public int currentIndex;
    public int amountFollowing;
    public int depth;
    public float fractionOfLineLookAhead;
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
    GameObject[] walls;
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
        walls = GameObject.FindGameObjectsWithTag("Wall");
	}

    // Update is called once per frame
    void Update()
    {
        if (canFollow && forwardUnit == null)
        {
            allBoids = GameObject.FindGameObjectsWithTag("boid");
            if (canFollow)
            {
                depth = int.MaxValue;
            }
            var minLayer = int.MaxValue;
            var f = this.gameObject;
            foreach (GameObject g in allBoids)
            {
                if (g != this.gameObject && g.GetComponent<Emergent>().depth < minLayer && PositionOpen(g) && g.gameObject != backLeft && g.gameObject != backRight)
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
                tempVect = -forwardUnit.transform.right * seperationAngle;
            }
            else
            {
                tempVect = forwardUnit.transform.right * seperationAngle;
            }
            destination = forwardUnit.transform.position - forwardUnit.transform.up * seperationDistance + tempVect;

            Vector2 formationAccelerationSeek = DynamicSeek(transform.position, destination);
            Vector2 formationAccelerationArrive = DynamicArrive(this.transform.position, destination, rb.velocity);
            Vector2 acceleration = formationAccelerationSeek + formationAccelerationArrive + coneCheck(this.gameObject)*.6f ;
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
            float zRotation = Mathf.Atan2(-GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y);
            transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * zRotation);
            if (Vector3.Distance(this.transform.position, path[currentIndex].transform.position) < .5f)
            {
                if (currentIndex < path.Length - 1)
                {
                    currentIndex++;
                    // transform.up = path[currentIndex].transform.position - transform.position;
                }
                else
                {
                    arrived = true;
                    rb.velocity = new Vector2(0, 0);
                }
            }
            Vector2 formationAccelerationSeek = Pathfind();
            Vector2 formationAccelerationArrive = DynamicArrive(this.transform.position, path[currentIndex].transform.position, rb.velocity);
            Vector2 acceleration = formationAccelerationSeek + formationAccelerationArrive + coneCheck(this.gameObject)*.2f;
            if (acceleration.magnitude > maxAcceleration)
            {
                acceleration = acceleration.normalized * maxAcceleration;
            }
            rb.velocity += acceleration;

            if (rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }
        }
    }

   public Vector2 coneCheck(GameObject b)
    {
        GameObject smallestDistance = null;
        float smallestDistanceAmount = 10000;
        Vector2 ourVelocity = b.GetComponent<Rigidbody2D>().velocity;
        foreach (GameObject g in walls)
        {
            if (g != b)
            {
                if (Vector3.Distance(g.transform.position, b.transform.position) < closeEnoughDistance)
                {
                    if (Vector3.Angle(g.transform.position, b.transform.position) < 90)
                    {
                        if (Vector3.Distance(g.transform.position, b.transform.position) < smallestDistanceAmount)
                        {
                            smallestDistance = g;
                            smallestDistanceAmount = Vector3.Distance(g.transform.position, b.transform.position);
                          //Vector2 theirVelocity = g.GetComponent<Rigidbody2D>().velocity;
                        }
                    }
                }
            }

        }
        if (smallestDistance != null)
        {

            Vector3 ourVelocity3D = ourVelocity;
            Vector3 predictedPosition = b.transform.position + smallestDistanceAmount * ourVelocity3D;
            // Vector3 theirVelocity3D = smallestDistance.GetComponent<Rigidbody2D>().velocity;
            Vector3 targetPredictedPosition = smallestDistance.transform.position; //smallestDistanceAmount * theirVelocity3D;
            //print(DynamicEvade(predictedPosition, targetPredictedPosition));
            return DynamicEvade(predictedPosition, targetPredictedPosition);
        }
        return Vector2.zero;
    }

Vector2 DynamicEvade(Vector3 position, Vector3 target)
{
    Vector2 linearAcc = position - target;

    return maxAcceleration * linearAcc.normalized;
}

public Vector2 Pathfind()
    {
            return DynamicSeek(transform.position, path[currentIndex].transform.position);
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
    public void MoveThroughTunnel(GameObject tunnelExit) {
        DynamicSeek(this.transform.position, tunnelExit.transform.position);
    }
}
