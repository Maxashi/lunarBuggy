using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteInEditMode]
public partial class PlayerVehicleController : MonoBehaviour
{
    [SerializeField] private List<AxleInfo> Axles;

    #region VehicleCharacteristics
    [SerializeField, Range(0, 1000)] private const float _maxEngineTorque = 270;
    [SerializeField, Range(0, 100)] private const float engineResponsiveness = 15f;
    [SerializeField, Range(0, 15)] private const float flyWheelMass = 3f;

    [SerializeField] private const float _steeringRange = 60;

    /// <summary>
    /// Maximum break torque applicable
    /// </summary>
    [SerializeField] private const float _breakingPower = 200;

    /// <summary>
    /// Controls how fast max break torque can be applied
    /// </summary>
    [SerializeField, Range(0, 100)] private float breakingResponsiveness = 15f;

    #endregion

    #region propulsion status
    private float _accelerationInput = 0.0f;
    private float _engineLoad = 0f;


    /// <summary>
    /// The current amount of torque applied to the brakes
    /// </summary>
    private float _breakTorque = 0f;


    private float _steeringInput = 0f;

    private float _vehicleTorque;

    private const float deadZone = 0.001f;

    #endregion

    public float MaxEngineTorque { get => _maxEngineTorque; }
    public float EngineResponsiveness { get => engineResponsiveness; }
    public float AccelerationInput { get => _accelerationInput; set => _accelerationInput = value; }
    public float EngineLoad { get => _engineLoad; set => _engineLoad = value; }

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
        _accelerationInput = Input.GetAxis("Vertical");

        HandleAcceleration(_accelerationInput);
        HandleBreaking(_accelerationInput);

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
            float targetTorque = verticalAxis * _maxEngineTorque;
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
    private float GetTorque()
    {
        float vehiclePower = 0f;

        foreach (var axle in Axles)
            vehiclePower += axle.GetTorque();

        return vehiclePower;
    }
    private void SetGuiSkin()
    {
        GUISkin _gUIskin = new GUISkin();

        // Create a custom label style with white color and centered alignment
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.normal.textColor = Color.white;
        labelStyle.alignment = TextAnchor.MiddleCenter;

        // Assign the custom style to the GUISkin
        _gUIskin.label = labelStyle;

        // Set the GUISkin
        GUI.skin = _gUIskin;
    }

    private void OnGUI()
    {
        int i = 0;
        float verticalOffset = 20;

        // Display acceleration bar
        DrawBar("Acceleration Iput", _accelerationInput, 0, MaxEngineTorque, verticalOffset * i++);

        // Display acceleration bar
        DrawBar("Engine Load", _engineLoad, 0, MaxEngineTorque, verticalOffset * i++);

        // Display steering bar
        DrawBar("SteeringInput", _steeringInput, -60, +60, verticalOffset * i++);

        // Display breaking power bar
        DrawBar("BreakForce", Mathf.Abs(_breakTorque), 0, 100, 60);

        DrawBar("Acceleration Input", _accelerationInput, 0, 1, 80);
    }


    [Space(20f)]
    [SerializeField] float barWidth = 200f;
    [SerializeField] float barHeight = 20f;
    float labelWidth = 100f;
    float marginLeft = 10f;

    public void DrawBar(string label, float value, float minValue, float maxValue, float yPosition)
    {


        float xPos = barWidth + 20f;

        float zerostate = Application.isPlaying ? 0 : 0.05f;

        // Bar background
        GUI.Box(new Rect(marginLeft, yPosition, barWidth, barHeight), label);

        // Calculate normalized value for the bar
        float normalizedValue = Mathf.InverseLerp(minValue, maxValue, value) + zerostate;

        // Bar foreground based on normalized value
        GUI.Box(new Rect(marginLeft, yPosition, barWidth * normalizedValue, barHeight), "");

        // Text displaying the label next to the bar
        // GUI.Label(new Rect(xPos, yPosition, labelWidth, barHeight), label);

        // Text displaying the input value inside the bar
        GUI.Label(new Rect(marginLeft, yPosition, barWidth, barHeight), value.ToString("F2"), new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = Color.white } });
    }
}
