using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelBehavior : MonoBehaviour {
    public GameObject[] boids;
    public GameObject tunnelEntrance;
    public GameObject tunnelExit;
    public SortedList<float, GameObject> enterOrder;
    public float timeBetween;
    public float maxTimeBetween;
	// Use this for initialization

	void Start () {
        boids = GameObject.FindGameObjectsWithTag("boid");
        foreach (GameObject b in boids) {
            float dist = Vector2.Distance(b.transform.position, tunnelEntrance.transform.position);
            enterOrder.Add(dist, b);
        }
	}
	
	// Update is called once per frame
	void Update () {
        int i = 0;
        timeBetween -= Time.deltaTime;
        if (timeBetween <= 0) {
           
        }	
	}
}
