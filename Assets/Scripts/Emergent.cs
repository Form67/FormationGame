using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emergent : MonoBehaviour {
    public int amountFollowing;
    public GameObject forwardUnit;
    public GameObject backLeft;
    public GameObject backRight;
    public bool canFollow;
    public bool destructable;
    public bool isLeft;
    public GameObject[] allBoids;
    public float seperationDistance;
    public float seperationAngle;
    public Vector2 destination;
	// Use this for initialization
	void Start () {
        allBoids = GameObject.FindGameObjectsWithTag("boid");
	}
	
	// Update is called once per frame
	void Update () {
        if (canFollow&& forwardUnit == null) {
            foreach (GameObject g in allBoids) {
                if (g != this.gameObject)
                {
                    int t = g.GetComponent<Emergent>().RequestEntry(this.gameObject);
                    if (t == 0)
                    {
                        continue;
                    }
                    else if (t == 1)
                    {
                        this.forwardUnit = g;
                        isLeft = true;
                        break;
                    }
                    else {
                        this.forwardUnit = g;
                        isLeft = false;
                        break;
                    }
                }
            }
        }
        if (forwardUnit != null && canFollow)
        {
            var tempVect = new Vector3();
            if (isLeft)
            {
                tempVect = -forwardUnit.transform.right*seperationAngle;
            }
            else {
                tempVect = forwardUnit.transform.right * seperationAngle;
            }
            destination = forwardUnit.transform.position - forwardUnit.transform.forward *seperationDistance + tempVect;
        }
        else if (!canFollow)
        {
            //pathfind
        }
	}

    public int RequestEntry(GameObject g) {

        if (this.backLeft == null)
        {
            this.backLeft = g;
            g.GetComponent<Emergent>().forwardUnit = this.gameObject;
            return 1;
        }
        else if (this.backRight == null)
        {
            this.backRight = g;
            g.GetComponent<Emergent>().forwardUnit = this.gameObject;
            return 2;
        }
        else return 0;
    }

}
