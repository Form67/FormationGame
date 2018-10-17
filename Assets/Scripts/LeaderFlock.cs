using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderFlock : MonoBehaviour {
    protected GameObject[] flock;
	protected Vector2 instantaneousVelocity;

	public float seperationConstant;
	public float closeEnoughDistance;
	public float maxSpeed;
	public float maxAcceleration;


	// Use this for initialization
	void Start () {
		flock = GetComponent<SpawnFlock> ().flockList.ToArray();
		instantaneousVelocity = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
		manageFlock ();
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mousePosition.z = 0;
		instantaneousVelocity = mousePosition - transform.position;
		changeLeadOrientation ();
		transform.position = mousePosition;

	}
	public void manageFlock(){
		foreach (GameObject a in flock) {
			manageIndividualUnit (a);
		}
	}

	public void manageIndividualUnit(GameObject a){
		Rigidbody2D rb = a.GetComponent<Rigidbody2D> ();
		Vector2 acceleration = getAcceleration(a);
		if (acceleration.magnitude > maxAcceleration) {
			acceleration = maxAcceleration * acceleration.normalized;
		}
		rb.velocity += acceleration;
		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = maxSpeed * rb.velocity.normalized;
		}
		changeOrientationOfFollower (a);
	}

	public virtual Vector2 getAcceleration(GameObject a) {
		Vector2 strength1 = avoid_collisions(a);
		Vector2 strength2 = match_velocity(a);
		Vector2 strength3 = flock_to_center(a);
		return strength1 + strength2 + strength3;
	}


    public Vector2 avoid_collisions(GameObject b) {
		Vector3 seperation = Vector2.zero;
		float amountOfCloseAgents = 0;
		foreach (GameObject a in flock){
			float distance = Vector3.Distance (a.transform.position, b.transform.position);
			if (a != b && distance < closeEnoughDistance) {
				float strength = Mathf.Min (seperationConstant / distance, maxAcceleration);
				seperation += strength * (b.transform.position - a.transform.position).normalized;
				amountOfCloseAgents++;
			}
		}
		float distanceToLead = Vector3.Distance(transform.position, b.transform.position);
		if(distanceToLead < closeEnoughDistance){
			float strengthFromLeader = Mathf.Min (seperationConstant / distanceToLead, maxAcceleration);
			seperation += strengthFromLeader * (b.transform.position - transform.position).normalized;
			amountOfCloseAgents++;
		}
		if (amountOfCloseAgents > 0) {
			amountOfCloseAgents = 1 / amountOfCloseAgents;
			seperation = amountOfCloseAgents * seperation;
		}
		return new Vector2(seperation.x, seperation.y);

    }
    public Vector2 match_velocity(GameObject b)
    {
		return instantaneousVelocity;

    }
    public Vector2 flock_to_center(GameObject b)
    {
		
		return (transform.position - b.transform.position);

    }

	public void changeLeadOrientation(){
		float zRotation = Mathf.Atan2 (-instantaneousVelocity.x, instantaneousVelocity.y);
		transform.eulerAngles = new Vector3 (0, 0, Mathf.Rad2Deg * zRotation);

	}

	public void changeOrientationOfFollower(GameObject b){
		float totalAngle = 0;
		int amountOfCloseAgents = 0;
		foreach (GameObject a in flock) {
			if (a != b && Vector3.Distance (a.transform.position, b.transform.position) < closeEnoughDistance) {
				totalAngle += a.transform.eulerAngles.z;
				amountOfCloseAgents++;
				if (a.transform.eulerAngles.z > 180) {
					totalAngle -= 360.0f;
				}
			}
		}
		if (Vector3.Distance (transform.position, b.transform.position) < closeEnoughDistance) {	
			totalAngle += transform.eulerAngles.z;
			amountOfCloseAgents++;
			if (transform.eulerAngles.z > 180) {
				totalAngle -= 360.0f;
			}
		}
		if (amountOfCloseAgents > 0) {
			totalAngle /= (float)amountOfCloseAgents;
			b.transform.eulerAngles = new Vector3 (0, 0, totalAngle);
		}
	}

    
}
