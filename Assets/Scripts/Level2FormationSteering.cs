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

	public GameObject[] path;

	public float coneCheckRadius;

	public float leadUnitMaxVelocity;
	public float leadUnitMaxAcceleration;

	public float formationUnitMaxVelocity;
	public float formationUnitMaxAcceleration;

	FormationPattern pattern;
	List<SlotAssignment> slotAssignments;

	PositionOrientation anchorPoint;

	public float offsetConstant;

	public float maxDistanceConstant;

	public float coneCheckConstant;
	GameObject[] walls;
	int currentIndex;
	bool completed;
	Rigidbody2D rbody;
	// Use this for initialization
	void Start () {
		pattern = new FormationPattern (characterRadius, numberOfSlots);
		slotAssignments = new List<SlotAssignment> ();
		for (int i = 0; i < numberOfSlots; i++) {
			SlotAssignment slot = new SlotAssignment ();
			slot.slotNumber = i;
			slot.character = Instantiate (unitPrefab);
			slotAssignments.Add (slot);
		}
		anchorPoint = pattern.getDriftOffset (slotAssignments);
		currentIndex = 0;
		rbody = GetComponent<Rigidbody2D> ();
		completed = false;
		walls = GameObject.FindGameObjectsWithTag("Wall");

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 averageVelocity = getAverageVelocity();
		Vector3 anchorPosition = offsetConstant * averageVelocity + getAverageUnitPosition ();
		foreach (SlotAssignment slot in slotAssignments) {
			Rigidbody2D rb = slot.character.GetComponent<Rigidbody2D> ();

			Vector3 position = slot.character.transform.position;
			Vector3 target = pattern.getSlotLocation (slot.slotNumber).position + anchorPosition;
			Vector2 formationSeekAcceleration = DynamicSeek (position, target, formationUnitMaxAcceleration);
			Vector2 formationArriveAcceleration = DynamicArrive (position, target, rb.velocity, formationUnitMaxVelocity, formationUnitMaxAcceleration);

			target = transform.position;

			Vector2 targetSeekAcceleration = DynamicSeek (position, target, formationUnitMaxAcceleration);
			Vector2 targetArriveAcceleration = DynamicArrive (position, target, rb.velocity, formationUnitMaxVelocity, formationUnitMaxAcceleration);

			Vector2 coneCheckAcceleration = coneCheckConstant * coneCheck(slot.character);


			Vector2 acceleration = coneCheckAcceleration + formationSeekAcceleration + formationArriveAcceleration + targetSeekAcceleration + targetArriveAcceleration;

			if (acceleration.magnitude > formationUnitMaxAcceleration) {
				acceleration = formationUnitMaxAcceleration * acceleration.normalized;
			}

			rb.velocity += acceleration;

			if (rb.velocity.magnitude > formationUnitMaxVelocity) {
				rb.velocity = formationUnitMaxVelocity * rb.velocity.normalized;
			}

			float angle = Mathf.Rad2Deg * Mathf.Atan2 (-rb.velocity.x, rb.velocity.y);

			slot.character.transform.eulerAngles = new Vector3 (0, 0, angle);



		}

	
		if (Vector3.Distance(this.transform.position, path[currentIndex].transform.position) < .5f)
		{
			if (currentIndex < path.Length - 1)
			{
				print (currentIndex);
				currentIndex++;
			}
			else
			{
				completed = true;
				rbody.velocity = new Vector2(0, 0);
			}
		}

		if (!completed) {
			Vector2 leadAcceleration = Pathfind (transform.position);
			//leadAcceleration += maxDistanceConstant * DynamicSeek (transform.position, anchorPoint.position, leadUnitMaxAcceleration);
			if (leadAcceleration.magnitude > leadUnitMaxAcceleration) {
				leadAcceleration = leadUnitMaxAcceleration * leadAcceleration.normalized;
			}

			rbody.velocity += leadAcceleration;

			if (rbody.velocity.magnitude > leadUnitMaxVelocity) {
				rbody.velocity = leadUnitMaxVelocity * rbody.velocity.normalized;
			}

			if (Vector3.Distance (transform.position, getAverageUnitPosition()) > maxDistanceConstant) {
				rbody.velocity *= 0.3f;

			}
			float leadAngle = Mathf.Rad2Deg * Mathf.Atan2 (-rbody.velocity.x, rbody.velocity.y);
		}

	}

	Vector2 DynamicSeek(Vector3 position, Vector3 target, float maxAcceleration){
		Vector2 linearAcc = target - position;
		return maxAcceleration * linearAcc.normalized;

	}

	Vector2 DynamicArrive(Vector3 position, Vector3 target, Vector2 currentVelocity, float maxVelocity, float maxAcceleration){

		float targetSpeed = maxVelocity * (Vector3.Distance (position, target) / slowRadius);
		Vector2 directionVector = (target - position).normalized;
		Vector2 targetVelocity = directionVector * targetSpeed;
		Vector2 acceleration = (targetVelocity - currentVelocity) / timeToTarget;
		return maxAcceleration * acceleration.normalized;
	}

	Vector2 DynamicEvade(Vector3 position, Vector3 target, float maxAcceleration){
		Vector2 linearAcc = position - target;
		return maxAcceleration * linearAcc.normalized;
	}
	/*
	Vector2 ConeCheck(Vector3 position, Vector3 orientation){
		Collider2D[] colliders = Physics2D.OverlapCircleAll(position, coneCheckRadius);
		Vector3 characterToCollider;
		float dot;

		// Cone check
		foreach (Collider2D collider in colliders)
		{
			characterToCollider = (collider.transform.position - position);
			dot = Vector3.Dot(characterToCollider, position);
			if (dot >= Mathf.Cos(100 * Mathf.Deg2Rad) && collider.tag != "boid")
			{

				return DynamicEvade(position, collider.transform.position, formationUnitMaxAcceleration);
			}
		}
		return Vector2.zero;
	}
	*/
	public Vector2 coneCheck(GameObject b)
	{
		GameObject smallestDistance = null;
		float smallestDistanceAmount = 10000;
		Vector2 ourVelocity = b.GetComponent<Rigidbody2D>().velocity;
		foreach (GameObject g in walls)
		{
			if (g != b)
			{
				if (Vector3.Distance(g.transform.position, b.transform.position) < coneCheckRadius)
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
			return DynamicEvade(predictedPosition, targetPredictedPosition, formationUnitMaxAcceleration);
		}
		return Vector2.zero;
	}

	public Vector2 Pathfind(Vector3 position) {
		return DynamicSeek(position, path[currentIndex].transform.position,  leadUnitMaxAcceleration);
	}

	public Vector3 getAverageUnitPosition(){
		Vector3 position = Vector3.zero;
		foreach (SlotAssignment slot in slotAssignments) {
			position += slot.character.transform.position;
		}
		position *= 1f / (float)(slotAssignments.Count);
		return position;
	}

	public Vector2 getAverageVelocity(){
		Vector2 velocity = Vector3.zero;
		foreach (SlotAssignment slot in slotAssignments) {
			Rigidbody2D rb = slot.character.GetComponent<Rigidbody2D> ();
			velocity += rb.velocity;
		}
		velocity *= 1f / (float)(slotAssignments.Count);
		return velocity;
	}



}
