using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerVehicleController playerVehiceController;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float rotationSpeedY = 5f;
 [SerializeField]  public float rotationSpeedX = 5f;

    /// <summary>
    /// Direction from target to camera.
    /// </summary>
    private Vector3 camDirection;

    private float camDistance;
    [SerializeField] private Vector3 lastPlayerPosition;
  
    [SerializeField, Range(-80f, 80f)]
    private float minYRotation = -80f;

    [SerializeField, Range(-80f, 80f)]
    private float maxYRotation = 80f;


    private float currentRotationX = 0f;
    private float currentRotationY = 0f;

    void Start()
    {
        CheckForPlayer();

        var delta = transform.position - playerTransform.position;
        camDistance = delta.magnitude;
        camDirection = delta.normalized;
    }
void Update()
{
    if (playerTransform == null)
        return;

    float mouseX = Input.GetAxis("Mouse X") * rotationSpeedX;
    float mouseY = Input.GetAxis("Mouse Y") * rotationSpeedY;

    // Incrementally update the rotation angles
    horizontalRotation += mouseX;
    verticalRotation = Mathf.Clamp(verticalRotation - mouseY, minYRotation, maxYRotation);

    // Create a quaternion based on the rotation angles
    Quaternion rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);

    // Use the rotation to set the camera direction
    Vector3 camDirection = rotation * Vector3.forward;

    // Update camera position based on player position and distance
    transform.position = playerTransform.position + camDirection * camDistance;
    transform.LookAt(playerTransform.position);
}
    private void CheckForPlayer()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;

        if (playerTransform == null)
        {
            Debug.LogError("Player reference not set in RacingCameraController!");
            return;
        }

        playerVehiceController = playerTransform.GetComponent<PlayerVehicleController>();
    }

    private void OnDrawGizmos()
    {
        if (playerTransform)
        {
            var directionEndPoint = playerTransform.position + camDirection;
            Gizmos.DrawLine(playerTransform.position, directionEndPoint);
        }
    }

    void OnValidate()
    {
        CheckForPlayer();
    }
}
