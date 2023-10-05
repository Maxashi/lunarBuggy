using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerVehicleController PlayerVehiceController;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    private Vector3 offset;
    private float currentRotationX = 0f;

    void Start()
    {
        CheckForPlayer();

      

        offset = transform.position - playerTransform.position;
    }

    private void CheckForPlayer()
    {

        playerTransform = GameObject.FindWithTag("Player").transform;

        if (playerTransform == null)
        {
            Debug.LogError("Player reference not set in RacingCameraController!");
            return;
        }
    }

    void Update()
    {
        if (playerTransform == null)
            return;

        // Adjust camera distance based on player speed
        float targetDistance = Mathf.Lerp(minDistance, maxDistance, 0.5f);
        offset.z = Mathf.Lerp(offset.z, -targetDistance, Time.deltaTime * zoomSpeed);

        // Rotate the camera based on mouse input
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        currentRotationX -= Input.GetAxis("Mouse Y") * rotationSpeed;
        //currentRotationX = Mathf.Clamp(currentRotationX, -20f, 20f);

        Quaternion rotation = Quaternion.Euler(currentRotationX, mouseX, 0);
        offset = rotation * offset;

        // Update camera position based on player position
        transform.position = playerTransform.position + offset;
        transform.rotation = rotation;
    }
}
