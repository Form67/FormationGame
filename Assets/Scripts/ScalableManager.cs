using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableManager : MonoBehaviour {

    [Header("Scalable")]
    public float startRadius;
    public int startingNumberOfUnits;
    public GameObject unitPrefab;

    public float characterRadius = 0.45f;
    float currentRadius;
    int currentNumberOfUnits;

    public Color leaderColor;
    GameObject leader;
    List<GameObject> unitsInFormation;

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
        // Compute values used to determine the target
        currentRadius = characterRadius / Mathf.Sin(Mathf.PI / startingNumberOfUnits);
        float angleDivision = 360.0f / (currentNumberOfUnits);

        float leadRotation = leader.transform.eulerAngles.z;
        Vector3 centerVector = (-currentRadius) * new Vector3(-Mathf.Sin(leadRotation * Mathf.Deg2Rad),
            Mathf.Cos(leadRotation * Mathf.Deg2Rad), 0) + leader.transform.position;

        // Move all units to the target
        for (int i = 0; i < currentNumberOfUnits - 1; i++)
        {
            // Get attributes
            Vector3 position = unitsInFormation[i].transform.position;
            Rigidbody2D rb = unitsInFormation[i].GetComponent<Rigidbody2D>();

            Vector3 directionFromRadiusVector = new Vector3(-Mathf.Sin((leadRotation + angleDivision * (i + 1)) * Mathf.Deg2Rad),
                                                    Mathf.Cos((leadRotation + angleDivision * (i + 1)) * Mathf.Deg2Rad));

            Vector3 target = centerVector + currentRadius * directionFromRadiusVector;
            unitsInFormation[i].GetComponent<ScalableUnit>().SetTarget(target, directionFromRadiusVector);
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

    public Vector3 ComputeCenter()
    {
        if (unitsInFormation.Count == 1)
            return leader.transform.position;

        Vector3 center = Vector3.zero;
        foreach (GameObject b in unitsInFormation)
        {
            // Don't keep the leader in mind
            if (b == leader)
                continue;

            center += b.transform.position;

        }

        center /= unitsInFormation.Count;
        return center;
    }
}
