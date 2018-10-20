using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBoid : MonoBehaviour
{
    public bool mouseControl;
    public float maxSpeed;

    public float stoppingOffset = 0.05f;

    Rigidbody2D rb;

	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody2D>();		
	}
	
	// Update is called once per frame
	void Update () {
        rb.velocity = Vector2.zero;
        if (Input.GetMouseButtonDown(0))
        {
            mouseControl = true;
        }
        if (Input.GetKey(KeyCode.S)) {
            mouseControl = false;
            rb.velocity = Vector2.down * maxSpeed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            mouseControl = false;
            rb.velocity = Vector2.up * maxSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            mouseControl = false;
            rb.velocity = Vector2.left * maxSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            mouseControl = false;
            rb.velocity = Vector2.right * maxSpeed;
        }
        if (mouseControl) {
            Vector3 rawPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mousePos = new Vector3(rawPos.x, rawPos.y, 0);

            if (Vector3.Distance(transform.position, mousePos) > stoppingOffset)
            {
                // Seek mouse
                Vector2 acceleration = (mousePos - transform.position) * maxSpeed;
                rb.velocity += acceleration;

                // Cap velocity
                if (rb.velocity.magnitude > maxSpeed)
                {
                    rb.velocity = rb.velocity.normalized * maxSpeed;
                }
            }
        }

        // Update orientation
        if (rb.velocity != Vector2.zero)
            transform.up = Vector3.Slerp(transform.up, rb.velocity.normalized, Time.deltaTime * 30f);
        else
            rb.angularVelocity = 0;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "boid") {
            collision.gameObject.SendMessage("DestroySelf");
        }
    }

}
