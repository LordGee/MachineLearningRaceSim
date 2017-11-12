using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 target;
    private GameObject car;

	// Use this for initialization
	void Start () {
		car = GameObject.Find("RaceCar");
	}
	
	// Update is called once per frame
	void Update ()
	{
	    target = car.transform.position;
	    transform.position = target;
	}
}
