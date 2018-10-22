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

    enum MovementMode { Normal, OneByOne, Row };
    public int moveIndex = 0;

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

        // Special Behavior: Object 2
        if (currentIndex == 3 || (currentIndex == 4 && moveIndex != unitsInFormation.Count)) {

            if (currentIndex == 3)
            {
                leader.GetComponent<ScalableUnit>().SetTarget(path[currentIndex].transform.position, Mathf.Infinity, 20f);
                MoveUnits(0, path[2].transform.position, 0.1f);
            }
            else if (currentIndex == 4)
            {
                leader.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                MoveAroundObj2();
            }

        }
        // Special Behavior: Object 6
        else if (currentIndex == 7 || (currentIndex == 8 && moveIndex != unitsInFormation.Count))
        {
            if (currentIndex == 7)
            {
                leader.GetComponent<ScalableUnit>().SetTarget(path[currentIndex].transform.position, Mathf.Infinity, 20f);
                MoveUnits(0, path[6].transform.position, 0.1f);
            }
            else if (currentIndex == 8)
            {
                leader.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                MoveAroundObj6();
            }

        }
        // Normal behavior
        else
        {
            leader.GetComponent<ScalableUnit>().SetTarget(path[currentIndex].transform.position);
            MoveUnits();
            CheckLeaderSpeed();
            
            if (currentIndex == 6)
            {
                moveIndex = 0; // reset before being used in object 6 manuvering
            }
        }
    }

    // Team members need to go through the tunnels one by one
    void MoveAroundObj2()
    {
        MoveUnits(moveIndex+1, path[2].transform.position, 0.1f);

        if (Vector3.Distance(unitsInFormation[moveIndex].transform.position, path[3].transform.position) < pathAcceptanceRange)
        {
            unitsInFormation[moveIndex].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            moveIndex++;
        }

        if(moveIndex < unitsInFormation.Count)
            unitsInFormation[moveIndex].GetComponent<ScalableUnit>().SetTarget(path[3].transform.position, Mathf.Infinity, 20f);
    }

    // Team members need to go through the tunnels two or three in a row
    void MoveAroundObj6()
    {
        StopUnits(moveIndex + 3);

        if (Vector3.Distance(unitsInFormation[moveIndex].transform.position, path[7].transform.position) < pathAcceptanceRange)
        {
            unitsInFormation[moveIndex].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            moveIndex++;
        }

        if (moveIndex < unitsInFormation.Count)
            MoveUnits(moveIndex, moveIndex + 2, path[7].transform.position, 0.1f);
    }


    void MoveUnits(int startIndex, Vector3 target, float acceptRange)
    {
        for (int i = startIndex; i < currentNumberOfUnits - 1; i++)
        {
            if (Vector3.Distance(unitsInFormation[i].transform.position, target) < acceptRange)
            {
                unitsInFormation[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                continue;
            }
            unitsInFormation[i].GetComponent<ScalableUnit>().SetTarget(target, Mathf.Infinity, 20f);
        }
    }

    void MoveUnits(int startIndex, int stopIndex, Vector3 target, float acceptRange)
    {
        for (int i = startIndex; i < stopIndex && stopIndex <= currentNumberOfUnits - 1; i++)
        {
            if (Vector3.Distance(unitsInFormation[i].transform.position, target) < acceptRange)
            {
                unitsInFormation[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            unitsInFormation[i].GetComponent<ScalableUnit>().SetTarget(target, Mathf.Infinity, 20f);
        }
    }

    void StopUnits(int startIndex)
    {
        for (int i = startIndex; i < currentNumberOfUnits - 1; i++)
        {
            unitsInFormation[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    void MoveUnits(Vector3 target)
    {
        for (int i = 0; i < currentNumberOfUnits - 1; i++)
        {
            unitsInFormation[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
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
