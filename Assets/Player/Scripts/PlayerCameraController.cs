using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    #region PlayerTransform

    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerVehicleController playerVehiceController;
    [SerializeField] private Vector3 lastPlayerPosition;
    #endregion

    #region  Camera Controls
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField, Range(0f, 2f)] private float rotationSpeedY = 1f;
    [SerializeField, Range(0f, 2f)] public float rotationSpeedX = 1f;

    /// <summary>
    /// Direction from target to camera.
    /// </summary>
    private Vector3 desiredCamDirection;
    private Vector3 desiredCamPosition;

    private float camDistance;

    [SerializeField, Range(-88f, 0f)] private float minYRotation = -80f;
    [SerializeField, Range(45f, 80f)] private float maxYRotation = 88f;

    private float currentRotationX  ;
    private float currentRotationY ; //Start behind the player
    #endregion

    void Start()
    {
        CheckForPlayer();

        var delta = transform.position - playerTransform.position;
        camDistance = delta.magnitude;
        desiredCamDirection = delta.normalized;
    }

    void Update()
    {
        if (playerTransform == null)
            return;

        transform.position = GetDesiredCamPosition();
        transform.LookAt(playerTransform.position);
    }

    private Vector3 GetDesiredCamPosition()
    {

        float mouseX = Input.GetAxis("Mouse X") * rotationSpeedX;
        float mouseY = -Input.GetAxis("Mouse Y") * rotationSpeedY;

        // Incrementally update the rotation angles
        currentRotationY += mouseX;
        currentRotationX = Mathf.Clamp(currentRotationX + mouseY, -maxYRotation, -minYRotation);

        // Create a quaternion based on the rotation angles
        Quaternion rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);

        // Use the rotation to set the camera direction
        desiredCamDirection = rotation * Vector3.forward;

        // Update camera position based on player position and distance
        return playerTransform.position + desiredCamDirection * camDistance;
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
            var directionEndPoint = playerTransform.position + desiredCamDirection;
            Gizmos.DrawLine(playerTransform.position, directionEndPoint);
        }
    }

    void OnValidate()
    {
        CheckForPlayer();
    }
}
