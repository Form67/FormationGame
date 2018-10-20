using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationScript : Movement {
	

    [Header("Formation")]
	public float formationWeightSeek;
	public float formationWeightArrive;

    [Header("Scalable")]
    public float startRadius;
    public int startingNumberOfUnits;
    public GameObject unitPrefab;

    public float characterRadius = 0.45f;
	float currentRadius;
	int currentNumberOfUnits;

	List<GameObject> unitsInFormation;
	Rigidbody2D rbody;

	// Use this for initialization
	void Start () {
        rbody = GetComponent<Rigidbody2D>();

        // Instantiate our list of units
        unitsInFormation = new List<GameObject>();
		for (int i = 0; i < startingNumberOfUnits - 1; i++) {
			unitsInFormation.Add(Instantiate (unitPrefab));
		}
        
		currentRadius = startRadius;
		currentNumberOfUnits = startingNumberOfUnits;
	}
	
	// Update is called once per frame
	void Update () {

        // Maintain our list of units
		foreach (GameObject unit in unitsInFormation) {
			if (unit == null) {
				unitsInFormation.Remove (unit);
				currentNumberOfUnits--;
			}
		}

        // 
		currentRadius = characterRadius / Mathf.Sin(Mathf.PI / startingNumberOfUnits);
		float angleDivision = 360.0f / (currentNumberOfUnits);
 
        float leadRotation = transform.eulerAngles.z;
		Vector3 centerVector = (-currentRadius)  * new Vector3 (-Mathf.Sin (leadRotation * Mathf.Deg2Rad), 
			Mathf.Cos (leadRotation * Mathf.Deg2Rad), 0) + transform.position;

        // Move all units
		for (int i = 0; i < startingNumberOfUnits - 1; i++) {

            // Get attributes
			Vector3 position = unitsInFormation [i].transform.position;
			Rigidbody2D rb = unitsInFormation [i].GetComponent<Rigidbody2D> ();

			Vector3 directionFromRadiusVector = new Vector3 (-Mathf.Sin ((leadRotation + angleDivision * (i + 1)) * Mathf.Deg2Rad), 
				                                    Mathf.Cos ((leadRotation + angleDivision * (i + 1)) * Mathf.Deg2Rad));
   
			Vector3 target = centerVector + currentRadius * directionFromRadiusVector;

            // Blend between dynamic seek and arrive
			Vector2 formationAccelerationSeek = formationWeightSeek * DynamicSeek (position, target);
			Vector2 formationAccelerationArrive = formationWeightArrive * DynamicArrive (position, target, rb.velocity);
			Vector2 acceleration= formationAccelerationSeek + formationAccelerationArrive;

            // Cap acceleration
            if (acceleration.magnitude > maxAcceleration) {
				acceleration = acceleration.normalized * maxAcceleration;
			}

            // Apply acceleration
			rb.velocity += acceleration;

            // Cap velocity
			if (rb.velocity.magnitude > maxVelocity) {
				rb.velocity = rb.velocity.normalized * maxVelocity;
			}

            // Adjust orientation
			float unitRotation = Mathf.Atan2 (-directionFromRadiusVector.x, directionFromRadiusVector.y);
			unitsInFormation [i].transform.eulerAngles = new Vector3 (0, 0, unitRotation * Mathf.Rad2Deg);
		}
			
	}
}
