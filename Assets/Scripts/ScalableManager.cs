using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableManager : MonoBehaviour {

    [Header("Scalable")]
    public float startRadius;
    public int startingNumberOfUnits;
    public GameObject unitPrefab;
    
    float currentRadius;
    int currentNumberOfUnits;

    public Color leaderColor;

    [Header("Path")]
    public GameObject[] path;
    public float pathAcceptanceRange = 0.5f;
    public int currentIndex = 0;

    [Header("Leader Reduce Speed")]
    public float maxDrift;
    public float maxDistToFormationCenter;
    
    GameObject leader;
    List<GameObject> unitsInFormation;
    Vector3 centerVector;

    // Use this for initialization
    void Awake () {
        // Instantiate our list of units
        unitsInFormation = new List<GameObject>();
        //int xOffset = 0;
        for (int i = 0; i < startingNumberOfUnits; i++)
        {
            //Vector3 instantiatePos = new Vector3(transform.position.x + xOffset, transform.position.y, 0);
            //unitsInFormation.Add(Instantiate(unitPrefab, instantiatePos, Quaternion.identity));
            //xOffset += 2;
            unitsInFormation.Add(Instantiate(unitPrefab));
        }

        currentRadius = startRadius;
        currentNumberOfUnits = startingNumberOfUnits;

        // Set the first leader unit
        AssignLeader();
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        MoveLeader();
        MoveUnits();
        CheckLeaderSpeed();
    }


    // Reduce speed when leader is too far away from the other units
    protected void CheckLeaderSpeed()
    {
        float speedReduction = 1.0f;

        for (int i = 0; i < currentNumberOfUnits - 1; i++)
        {
            float distFromCenter = Vector3.Distance(unitsInFormation[i].transform.position, centerVector);
            if (distFromCenter  - 0.1f > maxDistToFormationCenter)
            {
                speedReduction =  maxDrift / maxDistToFormationCenter;
                break;
            }
        }

        leader.GetComponent<Rigidbody2D>().velocity = leader.GetComponent<Rigidbody2D>().velocity * speedReduction;
    }

    void MoveLeader()
    {
        //print("Leader position: " + transform.position + " " + path[currentIndex].transform.position);
        if (Vector3.Distance(leader.transform.position, path[currentIndex].transform.position) < pathAcceptanceRange)
        {
            // Leader should check if there is enough space for the entire formation to move around the corner with obj 1
            if (currentIndex == 0)
            {
                float distToObj1 = Vector3.Distance(centerVector, path[0].transform.position);
                if (distToObj1 < maxDistToFormationCenter)
                    currentIndex++;
            }

            // Default path finding behavior
            if (currentIndex < path.Length - 1)
            {
                currentIndex++;
            }
        }
        leader.GetComponent<ScalableUnit>().SetTarget(path[currentIndex].transform.position);
    }


    // Team members need to go through the tunnels one by one
    void MoveAroundObj2()
    {

    }

    // Team members need to go through the tunnels two or three in a row
    void MoveAroundObj6()
    {

    }

    void MoveUnits()
    {
        // Compute values used to determine the target
        currentRadius = startRadius / Mathf.Sin(Mathf.PI / startingNumberOfUnits);
        float angleDivision = 360.0f / (currentNumberOfUnits);

        float leadRotation = leader.transform.eulerAngles.z;
        centerVector = (-currentRadius) * new Vector3(-Mathf.Sin(leadRotation * Mathf.Deg2Rad),
            Mathf.Cos(leadRotation * Mathf.Deg2Rad), 0) + leader.transform.position;
        

        // Move all units to the target
        for (int i = 0; i < currentNumberOfUnits - 1; i++)
        {
            // Get attributes
            Vector3 position = unitsInFormation[i].transform.position;
            Rigidbody2D rb = unitsInFormation[i].GetComponent<Rigidbody2D>();

            Vector3 directionFromRadiusVector = new Vector3(-Mathf.Sin((leadRotation + angleDivision * (i + 1)) * Mathf.Deg2Rad),
                                                    Mathf.Cos((leadRotation + angleDivision * (i + 1)) * Mathf.Deg2Rad));
            float zRotate = Mathf.Atan2(-directionFromRadiusVector.x, directionFromRadiusVector.y) * Mathf.Rad2Deg;

            Vector3 target = centerVector + currentRadius * directionFromRadiusVector;
            unitsInFormation[i].GetComponent<ScalableUnit>().SetTarget(target, zRotate);
        }
    }


    public int GetBoidCount()
    {
        return unitsInFormation.Count;
    }

    public void RemoveBoid(GameObject b)
    {
        FormationScript bScript = b.GetComponent<FormationScript>();
        // Save path parameter
        //int pathParam = bScript.currentParam;

        // Leader has been removed
        if (b == leader)
            AssignLeader();

        unitsInFormation.Remove(b);
        currentNumberOfUnits--;
    }

    public void AssignLeader()
    {
        // Assign first unit in our list as the new leader
        if (unitsInFormation.Count > 0)
        {
            leader = unitsInFormation[0];
            leader.GetComponent<SpriteRenderer>().color = leaderColor;
            unitsInFormation.RemoveAt(0);
        }
        else
            enabled = false; // no more units left -> do nothing
    }


}
