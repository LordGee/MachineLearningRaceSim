using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float accelerate = 20f;
    private float rotationSpeed = 50f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

	void Start () {
		
	}



	void Update () {
  
	    if (Input.GetAxis("Fire1") > 0 || Input.GetAxis("Jump") > 0) {
	        float move = Input.GetAxis("Jump") * accelerate;
            Vector3 target = new Vector3(0f,0f,move);
	        // transform.Translate(Vector3.forward * move);
            rb.AddForce(transform.forward, ForceMode.Acceleration);
        }

        if (Input.GetAxis("Horizontal") != 0) {
	        transform.Rotate(new Vector3(0f, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0f));
        }
	    
	}
}
