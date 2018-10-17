using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour {

	public Vector3 origin;
	public float amplitude;
	public float frequency;
	public float endX;
	LineRenderer lineR;
	public int lineSegments;

	public List<Vector3> linePoints;
	// Use this for initialization
	void Start () {
		lineR = GetComponent<LineRenderer> ();
		lineR.positionCount = lineSegments + 1;
		linePoints = new List<Vector3> ();
		for (int i = 0; i <= lineSegments; ++i) {

			Vector3 linePoint = getPoint (i / (float)lineSegments);
			linePoints.Add (linePoint);
			lineR.SetPosition (i, linePoint);
		}

	}
	
	// Update is called once per frame
	void Update () {
		lineR = GetComponent<LineRenderer> ();
		lineR.positionCount = lineSegments + 1;
		linePoints = new List<Vector3> ();
		for (int i = 0; i <= lineSegments; ++i) {

			Vector3 linePoint = getPoint (i / (float)lineSegments);
			linePoints.Add (linePoint);
			lineR.SetPosition (i, linePoint);
		}
	}

	public void Reset() {

		origin = Vector3.zero;
	}

	public Vector3 getPoint(float t){
		float y = amplitude * Mathf.Cos (frequency * t) + origin.y;
		float x = (endX - origin.x) * t + origin.x;
		return new Vector3 (x, y, 0);
	}

	/*public Vector3 getBezierPoint(float t) {
		return Vector3.Lerp (Vector3.Lerp (Vector3.Lerp (bezierPoints [0], bezierPoints [1], t), Vector3.Lerp (bezierPoints [1], bezierPoints [2], t), t), 
			Vector3.Lerp (Vector3.Lerp (bezierPoints [1], bezierPoints [2], t), Vector3.Lerp (bezierPoints [2], bezierPoints [3], t), t), t);
	}*/
}
