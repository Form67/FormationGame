using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationScript : MonoBehaviour {
	public float startRadius;
	public int startingNumberOfUnits;
	public GameObject unitPrefab;

	public float maxAcceleration;
	public float maxVelocity;

	public float formationWeight;
	float currentRadius;
	int currentNumberOfUnits;
	

	List<GameObject> unitsInFormation;

	Rigidbody2D rbody;

	// Use this for initialization
	void Start () {
		unitsInFormation = new List<GameObject>();
		for (int i = 0; i < startingNumberOfUnits - 1; i++) {
			unitsInFormation.Add(Instantiate (unitPrefab));
		}
		rbody = GetComponent<Rigidbody2D> ();
		currentRadius = startRadius;
		currentNumberOfUnits = startingNumberOfUnits;

	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject unit in unitsInFormation) {
			if (unit == null) {
				unitsInFormation.Remove (unit);
				currentNumberOfUnits--;
			}
		}
		currentRadius = (currentNumberOfUnits * startRadius) / startingNumberOfUnits;
		float angleDivision = 360.0f / (currentNumberOfUnits);

		float leadRotation = transform.eulerAngles.z;

		Vector3 centerVector = (-currentRadius)  * new Vector3 (Mathf.Sin (leadRotation * Mathf.Deg2Rad), 
			Mathf.Cos (leadRotation * Mathf.Deg2Rad), 0) + transform.position;
		for (int i = 0; i < startingNumberOfUnits - 1; i++) {
			Vector3 position = unitsInFormation [i].transform.position;
			Rigidbody2D rb = unitsInFormation [i].GetComponent<Rigidbody2D> ();
			Vector3 target = centerVector + new Vector3 (Mathf.Sin ((leadRotation + angleDivision * (i + 1)) * Mathf.Deg2Rad), 
				Mathf.Cos ((leadRotation + angleDivision * (i + 1)) * Mathf.Deg2Rad));
			Vector2 formationAcceleration = formationWeight * DynamicSeek (position, target);

			Vector2 acceleration= formationAcceleration;
			if (acceleration.magnitude > maxAcceleration) {
				acceleration = acceleration.normalized * maxAcceleration;
			}

			rb.velocity += acceleration;

			if (rb.velocity.magnitude > maxVelocity) {
				rb.velocity = rb.velocity.normalized * maxVelocity;
			}
		}
			
	}

	Vector2 DynamicSeek(Vector3 position, Vector3 target)
	{
		Vector2 linearAcc = target - position;
		return maxAcceleration * linearAcc;

	}

}
