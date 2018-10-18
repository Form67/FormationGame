using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct SlotAssignment{
	public GameObject character;
	public int slotNumber;

}


struct PositionOrientation{
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

	PositionOrientation getDriftOffset(List<SlotAssignment> slots){
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

	PositionOrientation getSlotLocation(int slotNumber){
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
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void updateSlotAssignments(){
	}





}
