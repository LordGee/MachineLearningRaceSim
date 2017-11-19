using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    private GameObject[] sensor;

    void Start()
    {
        Debug.Log(GameObject.FindGameObjectsWithTag("Sensor").Length);
        GetSensors();
    }

	void FixedUpdate () {
        for (int i = 0; i < sensor.Length; i++)
	    {
	        if (sensor[i] == null) {
	            GetSensors();
	        }
            RayCast(sensor[i].transform);
	    }
	}

    void GetSensors()
    {
        sensor = GameObject.FindGameObjectsWithTag("Sensor");
    }

    public void RayCast(Transform _sensor) {
        RaycastHit hit;

        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;
        // This would cast rays only against colliders in layer 8, so we just inverse the mask.
        layerMask = ~layerMask;

        Physics.Raycast(_sensor.position, _sensor.up, out hit, 10f, layerMask);
        Color col;
        if (hit.distance < 2f)
            col = Color.red;
        else
            col = Color.green;
        Debug.DrawLine(_sensor.position, hit.point, col);
        Debug.Log(hit.distance + " " + hit.transform.tag);
    }
}
