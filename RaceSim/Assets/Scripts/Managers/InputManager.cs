using UnityEngine;

/// <summary>
/// Manages all raycast and speed inputs that is required for the 
/// Machine Learning and Loaded AI agents.
/// </summary>
public class InputManager : MonoBehaviour {

    /// <summary>
    /// Struct for the ray cast information to be held until needed
    /// </summary>
    public struct RaycastInfo {
        public Vector3 position;
        public float distance;
    }

    private RaycastInfo[] raycastInfo;
    private float acceleration;

    /// <summary>
    /// Constructor
    /// </summary>
    void Start()
    {
        raycastInfo = new RaycastInfo[(int)ConstantManager.NNInputs.INPUT_COUNT];
        for (int i = 0; i < (int)ConstantManager.NNInputs.INPUT_COUNT; i++) {
            raycastInfo[i].position = Vector3.zero;
            raycastInfo[i].distance = 0;
        }
        GetDirection();
    }

    /// <summary>
    /// This function is called when required to Cast All Rays and 
    /// update the info struct.
    /// </summary>
    public void UpdateInputs() {
        GetDirection();
        CastAllRays();
    }

    /// <summary>
    /// The following function works out the direction for each raycast input.
    /// </summary>
    public void GetDirection()
    {
        float radianOrientation = (-transform.rotation.eulerAngles.y + 90f) * Mathf.PI / 180;
        raycastInfo = new RaycastInfo[(int)ConstantManager.NNInputs.INPUT_COUNT];
        float radian = radianOrientation;                       //Front
        raycastInfo[(int)ConstantManager.NNInputs.RAYCAST_FORWARD].position = 
            new Vector3(CosX(radian, ConstantManager.RAY_LENGTH), 0f, SinX(radian, ConstantManager.RAY_LENGTH));
        radian = radianOrientation - GetRadian(0.08f);          //FrontFrontRight
        raycastInfo[(int)ConstantManager.NNInputs.RAYCAST_FORWARD_FORWARD_RIGHT].position =
            new Vector3(CosX(radian, ConstantManager.RAY_LENGTH), 0, SinX(radian, ConstantManager.RAY_LENGTH));
        radian = radianOrientation + GetRadian(0.08f);          //FrontFrontLeft
        raycastInfo[(int)ConstantManager.NNInputs.RAYCAST_FORWARD_FORWARD_LEFT].position =
            new Vector3(CosX(radian, ConstantManager.RAY_LENGTH), 0, SinX(radian, ConstantManager.RAY_LENGTH));
        radian = radianOrientation - GetRadian(0.25f);          //FrontRight
        raycastInfo[(int)ConstantManager.NNInputs.RAYCAST_FORWARD_RIGHT].position = 
            new Vector3(CosX(radian, ConstantManager.RAY_LENGTH), 0, SinX(radian, ConstantManager.RAY_LENGTH));
        radian = radianOrientation + GetRadian(0.25f);          //FrontLeft
        raycastInfo[(int)ConstantManager.NNInputs.RAYCAST_FORWARD_LEFT].position = 
            new Vector3(CosX(radian, ConstantManager.RAY_LENGTH), 0, SinX(radian, ConstantManager.RAY_LENGTH));
        radian = radianOrientation - GetRadian(1.5708f);        //Right
        raycastInfo[(int)ConstantManager.NNInputs.RAYCAST_RIGHT].position =
            new Vector3(CosX(radian, ConstantManager.RAY_LENGTH), 0, SinX(radian, ConstantManager.RAY_LENGTH));
        radian = radianOrientation + GetRadian(1.5708f);        //Left
        raycastInfo[(int)ConstantManager.NNInputs.RAYCAST_LEFT].position =
            new Vector3(CosX(radian, ConstantManager.RAY_LENGTH), 0, SinX(radian, ConstantManager.RAY_LENGTH));
    }

    /// <summary>
    /// Iterates through the required Inputs  and executes the rays casts
    /// The -1 value is to account for the speed input which is not handled by the raycasts
    /// </summary>
    private void CastAllRays() {
        for (int i = 0; i < (int)ConstantManager.NNInputs.INPUT_COUNT - 1; i++) {
            RayCast(i);
        }
    }
    
    /// <summary>
    /// The following casts the rays and populates the distances to the info struct.
    /// Debug line is drawn for cosmetics, and colour of the line changes depending on distance
    /// </summary>
    /// <param name="_index">Which input to be cast</param>
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
        if (hit.distance < 2f) { col = Color.red; }
        else { col = Color.green; }
        Debug.DrawLine(transform.position, hit.point, col);
    }

    /// <summary>
    /// Getter for all the input values, if the value being requested is the speed then 
    /// another function deal with this
    /// </summary>
    /// <param name="_index">Which input index is required</param>
    /// <returns>Returns the input value</returns>
    public float GetInputByIndex(int _index) {
        if (_index != (int)ConstantManager.NNInputs.ACCELERATION) {
            return raycastInfo[_index].distance;
        } else if (_index == (int)ConstantManager.NNInputs.ACCELERATION) {
            GetAcceleration();
            return acceleration;
        }
        return 0;
    }

    /// <summary>
    /// Getter that returns the velocity magnitude of the agent.
    /// If the agent is moving too slow then 0f is returned.
    /// </summary>
    /// <returns>Velocity magnitude</returns>
    public float GetAcceleration() {
        if (GetComponent<Rigidbody>().velocity.magnitude > 0.2) {
            acceleration = GetComponent<Rigidbody>().velocity.magnitude;
        } else {
            acceleration = 0f;
        }
        return acceleration;
    }

    /* reusable math functions */
    private float CosX(float _radian, float _length) {
        return Mathf.Cos(_radian) * _length;
    }

    private float SinX(float _radian, float _length) {
        return Mathf.Sin(_radian) * _length;
    }

    float GetRadian(float _percentage) {
        return Mathf.PI * _percentage;
    }
}
