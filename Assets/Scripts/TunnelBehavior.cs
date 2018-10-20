using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelBehavior : MonoBehaviour {
    public GameObject[] boids;
    public bool waitingForTurn;
	// Use this for initialization
	void Start () {
        boids = GameObject.FindGameObjectsWithTag("boid");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void tunnel1() {

    }
}
