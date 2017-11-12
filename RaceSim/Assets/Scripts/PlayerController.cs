using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float accelerate = 75f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

	void Start () {
		
	}
	
	void Update () {
	    if (Input.GetAxis("Fire1") > 0 || Input.GetAxis("Jump") > 0)
	    {
            rb.AddForce(transform.forward * accelerate, ForceMode.Acceleration);
	    }
	}
}
