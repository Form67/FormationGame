using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeModesScript : MonoBehaviour {
	string[] modes = {"Cone Check", "Collision Prediction", "Both", "Neither"};
	int currIndex;
	Text text;

	public GameObject[] pathFollowingAgents;
	// Use this for initialization
	void Start () {
		text = GetComponentInChildren<Text> ();
		currIndex = 0;
		text.text = modes [currIndex];
	}
	
	public void ChangeMode() {
		currIndex = currIndex == 3 ? 0 : currIndex + 1;
		text.text = modes [currIndex];

		foreach (GameObject a in pathFollowingAgents) {
			PathFollowingLeadFlock script = a.GetComponent<PathFollowingLeadFlock> ();
			switch (currIndex) {
				case 0:
					script.isConeCheck = true;
					script.isCollisionPrediction = false;
					break;
				case 1:
					script.isConeCheck = false;
					script.isCollisionPrediction = true;
					break;
				case 2:
					script.isConeCheck = true;
					script.isCollisionPrediction = true;
					break;
				case 3:
					script.isConeCheck = false;
					script.isCollisionPrediction = false;
					break;
				default:
					break;

			}
		}  

	}
}
