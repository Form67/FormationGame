using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowingLeadFlock : LeaderFlock {

	public GameObject pathObject;
	public float fractionOfLineLookAhead;
	Path path;

	Rigidbody2D rbody;

	public bool isConeCheck;
	public bool isCollisionPrediction;
	public float evasionWeight;
    GameObject[] otherAgents;
	List<Vector3> originalPositions;

	void Start(){
		flock = GetComponent<SpawnFlock> ().flockList.ToArray();
		path = pathObject.GetComponent<Path> ();
		rbody = GetComponent<Rigidbody2D> ();
		isConeCheck = true;
		isCollisionPrediction = false;
		originalPositions = new List<Vector3> ();
		originalPositions.Add (transform.position);
		foreach (GameObject f in flock) {
			originalPositions.Add (f.transform.position);
		}
        otherAgents = GameObject.FindGameObjectsWithTag("BOID");
	}

	void Update(){
		manageFlock ();
		Vector2 acceleration = PathFollowing (path) + getLeadAcceleration();

		if (acceleration.magnitude > maxAcceleration) {
			acceleration = acceleration * maxAcceleration;
		}
		rbody.velocity += acceleration;
		if (rbody.velocity.magnitude > maxSpeed) {
			rbody.velocity = rbody.velocity.normalized * maxSpeed;
		}
		transform.eulerAngles = new Vector3 (0, 0, GetNewOrientation () * Mathf.Rad2Deg);
		instantaneousVelocity = rbody.velocity;
	}

	Vector2 PathFollowing(Path p){
		Vector3 closestPoint = p.linePoints[0];
		int closestIndex = 0;

		for (int i = 1; i < p.linePoints.Count; ++i) {
			if (Vector3.Distance (p.linePoints [i], transform.position) < Vector3.Distance (closestPoint, transform.position)) {
				closestPoint = p.linePoints [i];
				closestIndex = i;
			}
		}

		int targetIndex = closestIndex +(int) (fractionOfLineLookAhead * (float)p.linePoints.Count); 
		if (targetIndex >= p.linePoints.Count) {
			targetIndex = p.linePoints.Count - 1;
		}

		return DynamicSeek (p.linePoints [targetIndex]);

	}

	Vector2 DynamicSeek(Vector3 target)
	{
		Vector2 linearAcc = target - transform.position;
		return linearAcc;

	}

	float GetNewOrientation() {
		if (rbody.velocity.magnitude > 0f) {
			return Mathf.Atan2 (-rbody.velocity.x, rbody.velocity.y);
		}
		return transform.eulerAngles.z * Mathf.Deg2Rad;
	}

	public void resetToOriginalPosition(){
		transform.position = originalPositions [0];
		for (int i = 1; i < originalPositions.Count; ++i) {
			flock [i - 1].transform.position = originalPositions [i];
		}
	}

	Vector2 getLeadAcceleration() {
		Vector2 acceleration = Vector2.zero;
		if (isConeCheck) {
			acceleration += coneCheck (gameObject) /2.2f;
		}
		if (isCollisionPrediction) {
			acceleration += collisionPrediction (gameObject);
		}
		return acceleration;
	}

	public override Vector2 getAcceleration(GameObject a){
		Vector2 acceleration = base.getAcceleration (a);
		if (isConeCheck) {
			acceleration += coneCheck (a);
		}
		if (isCollisionPrediction) {
			acceleration += collisionPrediction (a);
		}
		return acceleration;

	}

    public Vector2 coneCheck(GameObject b) {
        GameObject smallestDistance = null;
        float smallestDistanceAmount = 10000;
        Vector2 ourVelocity = b.GetComponent<Rigidbody2D>().velocity;
        foreach (GameObject g in otherAgents) {
            if (g != b) {
                if (Vector3.Distance(g.transform.position, b.transform.position) < closeEnoughDistance)
                {
                    if (Vector3.Angle(g.transform.position, b.transform.position) < 50) {
                        if (Vector3.Distance(g.transform.position, b.transform.position) < smallestDistanceAmount) {
                            smallestDistance = g;
                            smallestDistanceAmount = Vector3.Distance(g.transform.position, b.transform.position);
                            Vector2 theirVelocity = g.GetComponent<Rigidbody2D>().velocity;
                        }
                    }
                }
            }
           
        }
        if (smallestDistance != null)
        {

            Vector3 ourVelocity3D = ourVelocity;
            Vector3 predictedPosition = b.transform.position + smallestDistanceAmount * ourVelocity3D;
            Vector3 theirVelocity3D = smallestDistance.GetComponent<Rigidbody2D>().velocity;
            Vector3 targetPredictedPosition = smallestDistance.transform.position + smallestDistanceAmount * theirVelocity3D;

            return DynamicEvade(predictedPosition, targetPredictedPosition);
        }
            return Vector2.zero;
    }

	public Vector2 collisionPrediction(GameObject b) {
		

		GameObject closestCollidingAgent = null;
		float tClosest = float.MaxValue;

		Vector2 ourVelocity = b.GetComponent<Rigidbody2D> ().velocity;

		foreach (GameObject agent in otherAgents) {
			if (Vector3.Distance (agent.transform.position, b.transform.position) < closeEnoughDistance && b != agent) {
				Vector2 dp = agent.transform.position - b.transform.position;

				Vector2 theirVelocity = agent.GetComponent<Rigidbody2D> ().velocity;
				Vector2 dv = theirVelocity - ourVelocity;
				if (theirVelocity.magnitude == 0) {
					continue;
				}

				float t = (-Vector2.Dot (dp, dv) / Mathf.Pow (dv.magnitude, 2));

				if (t < tClosest && t > 0) {
					tClosest = t;
					closestCollidingAgent = agent;
				}
			} 
		}
		if (closestCollidingAgent != null) {

			Vector3 ourVelocity3D = ourVelocity;
			Vector3 predictedPosition = b.transform.position + tClosest * ourVelocity3D;
			Vector3 theirVelocity3D = closestCollidingAgent.GetComponent<Rigidbody2D> ().velocity;
			Vector3 targetPredictedPosition = closestCollidingAgent.transform.position + tClosest * theirVelocity3D;

			return DynamicEvade (predictedPosition, targetPredictedPosition);

		}

		return Vector2.zero;
	}


	Vector2 DynamicEvade(Vector3 position, Vector3 target) {
		Vector2 linearAcc = position - target;

		return maxAcceleration * linearAcc.normalized;
	}
}
