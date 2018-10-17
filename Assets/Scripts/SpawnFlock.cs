using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFlock : MonoBehaviour {
	public int flockSize;
	public GameObject flockPrefab;
	public float initialSeperation;
	public Vector3 origin;


	public List<GameObject> flockList;

	// Use this for initialization
	void Awake () {
		flockList = new List<GameObject> ();
		for(int i = 0; i < flockSize; i++){
			GameObject flockInstance = Instantiate (flockPrefab);
			Vector3 position = origin;
			position.x += i * initialSeperation;
			flockInstance.transform.position = position;
			flockList.Add (flockInstance);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
