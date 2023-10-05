using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerVehicleController : MonoBehaviour
{
    [SerializeField] private List<AxleInfo> Axles;

    #region propulsion properties
    [SerializeField, Range(0, 1000)] private float maxEnginePower = 150;
    [SerializeField, Range(0, 100)] private float engineResponsiveness = 15f;
    [SerializeField, Range(0, 15)] private float flyWheelMass = 3f;
    #endregion

    [SerializeField] private float steeringRange;
    [SerializeField] private float breakingPower;

    public float AccelerationInput
    {
        get
        {
            return _accelerationInput;
        }
    }
    public float SteeringInput
    {
        get
        {
            return _steeringInput;
        }
    }
    public float BreakForce
    {
        get
        {
            return _breakforce;
        }
    }


    private float _accelerationInput = 0.0f;

    private float _engineLoad = 0f;
    private float _steeringInput = 0f;
    private float _breakforce = 0f;

    private const float deadZone = 0.001f;

    void Start()
    {
        foreach (AxleInfo axleInfo in Axles)
        {
            axleInfo.Initialize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        var vertical = Input.GetAxis("Vertical");

        HandleAcceleration(vertical);
        HandleBreaking(vertical);


        _steeringInput = Input.GetAxis("Horizontal") * steeringRange;


        foreach (AxleInfo axleInfo in Axles)
        {
            axleInfo.Apply(_engineLoad, _steeringInput, _breakforce);
        }
    }

    private void HandleAcceleration(float verticalAxis)
    {
        if (verticalAxis > deadZone)
        {
            float targetAcceleration = verticalAxis * maxEnginePower;
            // Smoothly interpolate towards the target acceleration
            _engineLoad = Mathf.Lerp(_engineLoad, targetAcceleration, Time.deltaTime * engineResponsiveness);
        }
        else
        {
            _engineLoad = Mathf.Lerp(_engineLoad, 0f, Time.deltaTime * flyWheelMass);
        }

    }

    private void HandleBreaking(float verticalAxis)
    {
        if (verticalAxis < -deadZone)
        {
            _breakforce = verticalAxis;
        }
        else
        {
            _breakforce = 0f;
        }
    }


    private void OnGUI()
    {
        // Display acceleration bar
        DrawBar("Engine Load", _engineLoad, 0, maxEnginePower, 20);

        // Display steering bar
        DrawBar("SteeringInput", SteeringInput, -60, +60, 40);

        // Display breaking power bar
        DrawBar("BreakForce", BreakForce, 0, 100, 60);

        DrawBar("Acceleration Input", _accelerationInput, 0, 1, 80);
    }

    private void DrawBar(string label, float value, float minValue, float maxValue, float yPosition)
    {
        float barWidth = 200f;
        float barHeight = 20f;
        float labelWidth = 100f;

        // Bar background
        GUI.Box(new Rect(10, yPosition, barWidth, barHeight), "");

        // Calculate normalized value for the bar
        float normalizedValue = Mathf.InverseLerp(minValue, maxValue, value);

        // Bar foreground based on normalized value
        GUI.Box(new Rect(10, yPosition, barWidth * normalizedValue, barHeight), "");

        // Text displaying the label next to the bar
        GUI.Label(new Rect(220, yPosition, labelWidth, barHeight), label);

        // Text displaying the input value inside the bar
        GUI.Label(new Rect(10, yPosition, barWidth, barHeight), value.ToString("F2"), new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = Color.white } });
    }

    [System.Serializable]
    public class AxleInfo
    {
        [SerializeField] public WheelCollider leftWheel;
        [SerializeField] public WheelCollider rightWheel;
        [SerializeField] public Transform leftWheelMeshTransform;
        [SerializeField] public Transform rightWheelMeshTransform;
        [SerializeField] public bool motor;
        [SerializeField] public bool steering;

        public void Initialize()
        {
            leftWheelMeshTransform = leftWheel.GetComponentInChildren<MeshRenderer>().transform;
            rightWheelMeshTransform = rightWheel.GetComponentInChildren<MeshRenderer>().transform;
        }

        public void Apply(float torque, float steeringInput, float breakForce)
        {

            if (motor)
            {
                leftWheel.motorTorque = torque;
                rightWheel.motorTorque = torque;
            }

            if (steering)
            {
                leftWheel.steerAngle = steeringInput;
                rightWheel.steerAngle = steeringInput;
            }

            leftWheel.brakeTorque = breakForce;
            rightWheel.brakeTorque = breakForce;

            if (leftWheelMeshTransform)
            {
                leftWheel.GetWorldPose(out Vector3 pos, out Quaternion rot);
                leftWheelMeshTransform.SetPositionAndRotation(pos, rot);
            }

            if (rightWheelMeshTransform)
            {
                rightWheel.GetWorldPose(out Vector3 pos, out Quaternion rot);
                rightWheelMeshTransform.SetPositionAndRotation(pos, rot);
            }

        }
    }

}
