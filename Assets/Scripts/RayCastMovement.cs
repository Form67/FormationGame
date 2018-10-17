using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastMovement : MonoBehaviour {
    //Using arbitration to deal with corner trap
    public GameObject[] path;
    public float arbitrationCooldown;
    public float avoidDistance;
    public int arbitrationWinner;
    public float maxArbitrationCooldown;
    public float raycastAngle;
    public Vector2 target;
    public float raycastDistance;
    public float max_acc;
    public float curr_speed;
    public float max_speed;
    public float linear_acc;
    public bool arrived;
    public int currentIndex;
    public Rigidbody2D rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        bool NoCollisions = false;
        Debug.DrawRay(transform.position, raycastDistance * new Vector2(Mathf.Sin(Mathf.Asin(transform.up.x) + .3f), Mathf.Cos(Mathf.Acos(transform.up.y) + .3f)), Color.red);
        Debug.DrawRay(transform.position, raycastDistance * new Vector2(Mathf.Sin(Mathf.Asin(transform.up.x) - .3f), Mathf.Cos(Mathf.Acos(transform.up.y) - .3f)), Color.red);
        if (arbitrationCooldown <= 0 &&arbitrationWinner <= 1)
        {
            RaycastHit2D hit1 = Physics2D.Raycast(transform.position,new Vector2(Mathf.Sin(Mathf.Asin(transform.up.x) + .3f), Mathf.Cos(Mathf.Acos(transform.up.y) + .3f)), raycastDistance);
            if (hit1.collider != null)
            {
                arbitrationCooldown = maxArbitrationCooldown;
                arbitrationWinner = 1;
                target = hit1.point + hit1.normal * avoidDistance;
            }
            else
            {
                NoCollisions = true;
            }
        }
        if (arbitrationCooldown <= 0 && (arbitrationWinner == 2)|| arbitrationWinner ==0) {
           // Debug.DrawRay(transform.position, new Vector2(-raycastAngle, 1) * raycastDistance, Color.red);
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position, new Vector2(Mathf.Sin(Mathf.Asin(transform.up.x) - .3f), Mathf.Cos(Mathf.Acos(transform.up.y) - .3f)), raycastDistance);
            if (hit2.collider != null)
            {
                if (NoCollisions) {
                    NoCollisions = false;
                }
                arbitrationWinner = 2;
                arbitrationCooldown = maxArbitrationCooldown;
                target = hit2.point + hit2.normal * avoidDistance;
            }
            else { NoCollisions = true;
                arbitrationCooldown = 1f;
                arbitrationWinner = 0;
            }
           
        }
        arbitrationCooldown -= Time.deltaTime;
        if (arbitrationCooldown < 0) {
            arbitrationWinner = 0;
        }
        if (Vector2.Distance(this.transform.position, target) < .1f) {
           arbitrationWinner = 0;
            arbitrationCooldown = 0;
        }

        if (arbitrationWinner == 0 || NoCollisions)
        {
            Pathfind();
        }
        else {
            Seek();
        }

        float zRotation = Mathf.Atan2(-GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y);
        transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * zRotation);
        if (Vector3.Distance(this.transform.position, path[currentIndex].transform.position) < .5f)
        {
            if (currentIndex < path.Length - 1)
            {
                currentIndex++;
                // transform.up = path[currentIndex].transform.position - transform.position;
            }
            else
            {
                arrived = true;
                Destroy(this.gameObject);
            }
        }

    }
    public void Seek() 
        {
            //linear_acc = Vector3.Distance(transform.position, target);
            var magnitude = target- new Vector2(transform.position.x, transform.position.y);
            magnitude = magnitude.normalized;
            //transform.up = magnitude;
            if (magnitude.magnitude > max_acc)
            {
                magnitude = max_acc*magnitude.normalized;
            }
        rb.velocity += new Vector2(magnitude.x, magnitude.y);
        if (rb.velocity.magnitude > max_speed)
          {
              rb.velocity = max_speed * rb.velocity.normalized;
              }

        }

    public void Pathfind()
    {
        if (!arrived)// seeks to each node on path
        { 
            var magnitude = -transform.position + path[currentIndex].transform.position;
            magnitude = magnitude.normalized;
            //transform.up = magnitude;
            if (magnitude.magnitude > max_acc)
            {
                magnitude = max_acc * magnitude.normalized;
            }
            rb.velocity += new Vector2( magnitude.x,magnitude.y);
            if (rb.velocity.magnitude > max_speed)
            {
                rb.velocity = max_speed * rb.velocity.normalized;
            }
           
        }
    }
}
