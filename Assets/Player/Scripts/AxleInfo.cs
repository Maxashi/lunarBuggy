using UnityEngine;

public partial class PlayerVehicleController
{
    [System.Serializable]
    public class AxleInfo
    {
        [SerializeField] public WheelCollider _leftWheel;
        [SerializeField] public WheelCollider _rightWheel;
        [SerializeField] public Transform leftWheelMeshTransform;
        [SerializeField] public Transform rightWheelMeshTransform;
        [SerializeField] public bool motor;
        [SerializeField] public bool steering;

        public void Initialize()
        {
            leftWheelMeshTransform = _leftWheel.GetComponentInChildren<MeshRenderer>().transform;
            rightWheelMeshTransform = _rightWheel.GetComponentInChildren<MeshRenderer>().transform;
        }

        public void Apply(float torque, float steeringInput, float breakForce)
        {

            if (_leftWheel == null || _rightWheel == null)
            {
                return;
            }
            else
            {
                if (motor)
                {
                    _leftWheel.motorTorque = torque;
                    _rightWheel.motorTorque = torque;
                }

                if (steering)
                {
                    _leftWheel.steerAngle = steeringInput;
                    _rightWheel.steerAngle = steeringInput;
                }

                _leftWheel.brakeTorque = breakForce;
                _rightWheel.brakeTorque = breakForce;

            }

            if (leftWheelMeshTransform)
            {
                _leftWheel.GetWorldPose(out Vector3 pos, out Quaternion rot);
                leftWheelMeshTransform.SetPositionAndRotation(pos, rot);
            }

            if (rightWheelMeshTransform)
            {
                _rightWheel.GetWorldPose(out Vector3 pos, out Quaternion rot);
                rightWheelMeshTransform.SetPositionAndRotation(pos, rot);
            }
        }

        public float GetHorspower()
        {
            float powerHorsepower;

            if (_leftWheel != null && _rightWheel != null)
            {
                // Get motor torque from WheelCollider
                float motorTorqueLeft = _leftWheel.motorTorque;
                float motorTorqueRight = _rightWheel.motorTorque;
                // Get RPM from WheelCollider
                float rpmLeft = _leftWheel.rpm;
                float rpmRight = _rightWheel.rpm;
                // Convert RPM to rad/s
                float angularVelocityLeft = rpmLeft * 2 * Mathf.PI / 60;
                float angularVelocityRight = rpmRight * 2 * Mathf.PI / 60;

                // Calculate power in watts
                float powerWattsLeft = (motorTorqueLeft * angularVelocityLeft) +
                 (motorTorqueRight * angularVelocityRight);

                // Convert power to horsepower
                powerHorsepower = powerWattsLeft / 745.7f;

                return powerHorsepower;
            }
            return 0f;
        }

    }

}
