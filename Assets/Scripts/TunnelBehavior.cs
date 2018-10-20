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
    public float endTimer = 10f;
    public int i = 0;
	// Use this for initialization

	void Start () {
        enterOrder = new SortedList<float, GameObject>();
        boids = GameObject.FindGameObjectsWithTag("boid");
        foreach (GameObject b in boids) {
            float dist = Vector2.Distance(b.transform.position, tunnelEntrance.transform.position);
            enterOrder.Add(dist, b);
           b.GetComponent<Emergent>().goingThroughTunnel = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
        timeBetween -= Time.deltaTime;
        if (timeBetween <= 0 && i < enterOrder.Count)
        {
            enterOrder.Values[i].GetComponent<Emergent>().tunnelEntrance = tunnelEntrance;
            enterOrder.Values[i].GetComponent<Emergent>().tunnelExit = tunnelExit;
            enterOrder.Values[i].GetComponent<Emergent>().myTurn = true;
            timeBetween = maxTimeBetween;
            i++;
        }
        else if (i >= enterOrder.Count)
        {
            endTimer -= Time.deltaTime;
            if (endTimer <= 0)
            {
                foreach (GameObject b in enterOrder.Values)
                {
                    b.GetComponent<Emergent>().goingThroughTunnel = false;
                    b.GetComponent<Emergent>().atTunnelEntrance = false;
                }
                this.GetComponent<TunnelBehavior>().enabled = false;
            }
        }
	}
}
