using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SlotAssignment{
	public GameObject character;
	public int slotNumber;

}


public struct PositionOrientation{
	public Vector3 position;
	public Vector3 orientation;

}

public class FormationPattern {

	float characterRadius;
	int numberOfSlots;

	public FormationPattern(float cR, int nOS){
		characterRadius = cR;
		numberOfSlots = nOS;
	}

	public PositionOrientation getDriftOffset(List<SlotAssignment> slots){
		PositionOrientation center = new PositionOrientation ();


		foreach (SlotAssignment slot in slots) {
			PositionOrientation location = getSlotLocation (slot.slotNumber);
			center.position += location.position;
			center.orientation += location.orientation;
		}

		center.position *= 1f / (float)slots.Count;
		center.orientation *= 1f / (float)slots.Count;

		return center;
	}

	public PositionOrientation getSlotLocation(int slotNumber){
		float angleAroundCircle = (slotNumber / (float) numberOfSlots) * 2f * Mathf.PI;
		float radius = characterRadius / Mathf.Sin (Mathf.PI / numberOfSlots);

		PositionOrientation location = new PositionOrientation ();
		location.position = new Vector3 (radius * -Mathf.Sin (angleAroundCircle), radius * Mathf.Cos (angleAroundCircle), 0);
		location.orientation = new Vector3(0, 0, angleAroundCircle * Mathf.Rad2Deg);

		return location;
	}
}

public class Level2FormationSteering : MonoBehaviour {

	public int numberOfSlots;
	public float characterRadius;
	public GameObject unitPrefab;
	public float anchorOffset;
	public float timeToTarget;
	public float slowRadius;


	public float coneCheckRadius;

	public float leadUnitMaxVelocity;
	public float leadUnitMaxAcceleration;

	public float formationUnitMaxVelocity;
	public float formationUnitMaxAcceleration;

	FormationPattern pattern;
	List<SlotAssignment> slotAssignments;

	PositionOrientation centerOfMass;

	// Use this for initialization
	void Start () {
		pattern = new FormationPattern (characterRadius, numberOfSlots);
		for (int i = 0; i < numberOfSlots; i++) {
			SlotAssignment slot = new SlotAssignment ();
			slot.slotNumber = i;
			slot.character = Instantiate (unitPrefab);
			slotAssignments.Add (slot);
		}
		centerOfMass = pattern.getDriftOffset (slotAssignments);

	}
	
	// Update is called once per frame
	void Update () {
		Vector2 averageVelocity = Vector2.zero;
		foreach (SlotAssignment slot in slotAssignments) {
			Rigidbody2D rb = slot.character.GetComponent<Rigidbody2D> ();

			Vector3 position = slot.character.transform.position;
			Vector3 target = pattern.getSlotLocation (slot.slotNumber).position + centerOfMass.position;
			Vector2 formationSeekAcceleration = DynamicSeek (position, target, formationUnitMaxAcceleration);
			Vector2 formationArriveAcceleration = DynamicArrive (position, target, rb.velocity, formationUnitMaxVelocity, formationUnitMaxAcceleration);

			target = transform.position;

			Vector2 targetSeekAcceleration = DynamicSeek (position, target, formationUnitMaxAcceleration);
			Vector2 targetArriveAcceleration = DynamicArrive (position, target, rb.velocity, formationUnitMaxVelocity, formationUnitMaxAcceleration);

			Collider2D[] colliders = Physics2D.OverlapCircleAll(position, coneCheckRadius);


			Vector2 acceleration = formationSeekAcceleration + formationArriveAcceleration + targetSeekAcceleration + targetArriveAcceleration;

			if (acceleration.magnitude > formationUnitMaxAcceleration) {
				acceleration = formationUnitMaxAcceleration * acceleration.normalized;
			}

			rb.velocity += acceleration;

			if (rb.velocity.magnitude > formationUnitMaxVelocity) {
				rb.velocity = formationUnitMaxVelocity * rb.velocity.normalized;
			}



			averageVelocity += rb.velocity;

		}
		averageVelocity *= 1f / (float)slotAssignments.Count;

	}

	Vector2 DynamicSeek(Vector3 position, Vector3 target, float maxAcceleration){
		Vector2 linearAcc = target - position;
		return maxAcceleration * linearAcc;

	}

	Vector2 DynamicArrive(Vector3 position, Vector3 target, Vector2 currentVelocity, float maxVelocity, float maxAcceleration){

		float targetSpeed = maxVelocity * (Vector3.Distance (position, target) / slowRadius);
		Vector2 directionVector = (target - position).normalized;
		Vector2 targetVelocity = directionVector * targetSpeed;
		Vector2 acceleration = (targetVelocity - currentVelocity) / timeToTarget;
		return maxAcceleration * acceleration;
	}

	Vector2 ConeCheck(Vector3 position, Vector3 orientation){
        return Vector2.zero;
	}





}
