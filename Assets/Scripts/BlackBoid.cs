using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBoid : MonoBehaviour {
    public bool mouseControl;
    public float maxSpeed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            mouseControl = true;

        }
        if (Input.GetKeyDown(KeyCode.S)) {
            mouseControl = false;
            transform.position -= new Vector3(0, maxSpeed);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            mouseControl = false;
            transform.position += new Vector3(0, maxSpeed);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            mouseControl = false;
            transform.position -= new Vector3(maxSpeed, 0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            mouseControl = false;
            transform.position += new Vector3(maxSpeed, 0);
        }
        if (mouseControl) {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "boid") {
            Destroy(collision.gameObject);
        }
    }

}
