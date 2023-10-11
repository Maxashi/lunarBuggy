using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public partial class PlayerVehicleController : MonoBehaviour
{
    [SerializeField] private List<AxleInfo> Axles;

    #region propulsion
    [SerializeField, Range(0, 1000)] private float maxEngineTorque = 270;
    [SerializeField, Range(0, 100)] private float engineResponsiveness = 15f;
    [SerializeField, Range(0, 15)] private float flyWheelMass = 3f;
    private float _accelerationInput = 0.0f;
    private float _engineLoad = 0f;

    #endregion

    [SerializeField] private float _steeringRange = 60;

    /// <summary>
    /// Maximum break torque applicable
    /// </summary>
    [SerializeField] private float _breakingPower = 200;

    /// <summary>
    /// Controls how fast max break torque can be applied
    /// </summary>
    [SerializeField, Range(0, 100)] private float breakingResponsiveness = 15f;

    /// <summary>
    /// The current amount of torque applied to the brakes
    /// </summary>
    private float _breakTorque = 0f;


    private float _steeringInput = 0f;

    private float _vehicleTorque;

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

        _steeringInput = Input.GetAxis("Horizontal") * _steeringRange;


        foreach (AxleInfo axleInfo in Axles)
        {
            axleInfo.Apply(_engineLoad, _steeringInput, _breakTorque);
        }

        CalculateVehicleTorque();
    }

    private void CalculateVehicleTorque()
    {
        float temp = 0f;

        foreach (AxleInfo axleInfo in Axles)
        {
            temp += axleInfo.GetTorque();
        }

        _vehicleTorque = temp;
    }

    private void HandleAcceleration(float verticalAxis)
    {
        if (verticalAxis > deadZone)
        {
            float targetTorque = verticalAxis * maxEngineTorque;
            // Smoothly interpolate towards the target acceleration
            _engineLoad = Mathf.Lerp(_engineLoad, targetTorque, Time.deltaTime * engineResponsiveness);
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
            float targetBrakeTorque = Mathf.Abs(verticalAxis * _breakingPower);

            _breakTorque = Mathf.Lerp(_breakTorque, targetBrakeTorque, Time.deltaTime * breakingResponsiveness);
        }
        else
        {
            _breakTorque = 0f;
        }
    }


    private void OnGUI()
    {
        float vehiclePower = 0f;

        foreach (var axle in Axles)
            vehiclePower += axle.GetTorque();

        // Display acceleration bar
        DrawBar("Engine Load", _engineLoad, 0, maxEngineTorque, 20);

        // Display steering bar
        DrawBar("SteeringInput", _steeringInput, -60, +60, 40);

        // Display breaking power bar
        DrawBar("BreakForce", Mathf.Abs(_breakTorque), 0, 100, 60);

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
}
