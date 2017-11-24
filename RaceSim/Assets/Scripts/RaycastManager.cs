using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    public struct RaycastInfo {
        public Vector3 position;
        public float distance;
    }

    private RaycastInfo[] raycastInfo;
    private float acceleration;

    void Start()
    {
        raycastInfo = new RaycastInfo[(int)ConstantManager.NNInputs.INPUT_COUNT];
        for (int i = 0; i < (int)ConstantManager.NNInputs.INPUT_COUNT; i++) {
            raycastInfo[i].position = Vector3.zero;
            raycastInfo[i].distance = 0;
        }
        GetDirection();
    }

	void FixedUpdate () {
        UpdateInputs();
	}

    public void UpdateInputs() {
        GetDirection();
        GetAcceleration();
        CastAllRays();
    }

    

    public void GetDirection()
    {
        float radianOrientation = (-transform.rotation.eulerAngles.y + 90f) * Mathf.PI / 180;
        
        //Front
        float radian = radianOrientation;

        raycastInfo[(int)ConstantManager.NNInputs.RAYCAST_FORWARD].position = new Vector3(CosX(radian, ConstantManager.RAY_LENGTH), 0f, SinX(radian, ConstantManager.RAY_LENGTH));

        //FrontRight
        radian = radianOrientation - GetRadian(0.25f);
        raycastInfo[(int)ConstantManager.NNInputs.RAYCAST_FORWARD_RIGHT].position = new Vector3(CosX(radian, ConstantManager.RAY_LENGTH), 0, SinX(radian, ConstantManager.RAY_LENGTH));

        //FrontLeft
        radian = radianOrientation + GetRadian(0.25f);
        raycastInfo[(int)ConstantManager.NNInputs.RAYCAST_FORWARD_LEFT].position = new Vector3(CosX(radian, ConstantManager.RAY_LENGTH), 0, SinX(radian, ConstantManager.RAY_LENGTH));


        /*
        //Right
        radian = radianOrientation - GetRadian(0.5f);
        _raycastData[(int)Direction.Right].coordinates = new Vector3(CosX(radian, lenght), 0, SinX(radian, lenght));

        //FrontFrontRight
        radian = radianOrientation - GetRadian(0.04175f);
        _raycastData[(int)Direction.FrontFrontRight].coordinates = new Vector3(CosX(radian, lenght), 0, SinX(radian, lenght));

        //FrontFrontLeft
        radian = radianOrientation + GetRadian(0.25f);
        _raycastData[(int)Direction.FrontFrontLeft].coordinates = new Vector3(CosX(radian, lenght), 0, SinX(radian, lenght));

        //Left
        radian = radianOrientation + GetRadian(0.5f);
        _raycastData[(int)Direction.Left].coordinates = new Vector3(CosX(radian, lenght), 0, SinX(radian, lenght));
        */
    }

    private void GetAcceleration()
    {
        if (GetComponent<Rigidbody>().velocity.magnitude > 0.1)
        {
            acceleration = GetComponent<Rigidbody>().velocity.magnitude;
        }
        else
        {
            acceleration = 0f;
        }
    }

    private void CastAllRays()
    {
        for (int i = 0; i < (int)ConstantManager.NNInputs.INPUT_COUNT - 1; i++)
        {
            RayCast(i);
        }
    }

    public void RayCast(int _index) {
        RaycastHit hit;

        /* Ref: https://docs.unity3d.com/Manual/Layers.html */
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;
        // This would cast rays only against colliders in layer 8, so we just inverse the mask.
        layerMask = ~layerMask;

        // Physics.Raycast(_sensor.transform.position, _sensor.up, out hit, 10f, layerMask);
        Physics.Raycast(transform.position, raycastInfo[_index].position, out hit, ConstantManager.RAY_LENGTH, layerMask);

        raycastInfo[_index].distance = hit.distance;

        Color col;
        if (hit.distance < 2f)
            col = Color.red;
        else
            col = Color.green;
        Debug.DrawLine(transform.position, hit.point, col);

        // Debug.Log(hit.transform.name + _index);
    }

    /* reusable math functions */

    private float CosX(float _radian, float _length) {
        return Mathf.Cos(_radian) * _length;
    }

    private float SinX(float _radian, float _length) {
        return Mathf.Sin(_radian) * _length;
    }

    // percentage of pi
    float GetRadian(float _percentage) {
        return Mathf.PI * _percentage;
    }
}
