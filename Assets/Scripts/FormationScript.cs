using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationScript : MonoBehaviour {
	public float startRadius;
	public int startingNumberOfUnits;
	public GameObject unitPrefab;
	public float formationWeight;
	float currentRadius;
	int currentNumberOfUnits;
	

	List<GameObject> unitsInFormation;

	Rigidbody2D rbody;

	// Use this for initialization
	void Start () {
		unitsInFormation = new List<GameObject>(startingNumberOfUnits - 1);
		for (int i = 0; i < startingNumberOfUnits - 1; i++) {
			unitsInFormation [i] = Instantiate (unitPrefab);
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
		float angleDivision = 360.0f / currentNumberOfUnits;
		Vector3 centerOfGroup = transform.position;
		for (int i = 0; i < currentNumberOfUnits; i++) {
			centerOfGroup += unitsInFormation [i].transform.position;
		}
		centerOfGroup *= 1 / (currentNumberOfUnits + 1.0f);
			
	}

	Vector2 DynamicSeek(Vector3 position, Vector3 target)
	{
		Vector2 linearAcc = target - position;
		return linearAcc;

	}

}
