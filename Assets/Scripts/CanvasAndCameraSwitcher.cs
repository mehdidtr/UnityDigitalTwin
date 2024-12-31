using System.Collections.Generic;
using UnityEngine;

public class CanvasAndCameraPositionSwitcher : MonoBehaviour
{
    public Canvas[] canvases;             // Array to hold all canvases
    public Transform[] cameraPositions;  // Array to store predefined camera positions and rotations
    public Camera mainCamera;         // Reference to the main camera
    public float transitionSpeed = 5f;   // Speed of the camera transition (optional for smooth movement)

    private Dictionary<string, Transform> turbineCameraPositions; // Maps turbineID to camera position

    void Start()
    {
        // Initialize the dictionary mapping turbine IDs to camera positions
        turbineCameraPositions = new Dictionary<string, Transform>
        {
            { "T98", cameraPositions[2] },
            { "T99", cameraPositions[3] },
            { "T100", cameraPositions[4] },
            { "T101", cameraPositions[5] },
            { "T102", cameraPositions[6] },
            { "T103", cameraPositions[7] },
            { "T104", cameraPositions[8] },
            { "T105", cameraPositions[9] },
            { "T106", cameraPositions[10] },
            { "T107", cameraPositions[11] },
            // Add more mappings as needed
        };

        // Start by activating the first canvas and positioning the main camera
        SwitchToView(0);
    }

    public void SwitchToView(int index)
    {
        MoveCameraToPosition(index);

        if (index == 0 ){
            canvases[0].gameObject.SetActive(true);
            canvases[1].gameObject.SetActive(false);
        }
        else{
            canvases[0].gameObject.SetActive(false);
            canvases[1].gameObject.SetActive(true);
        }


        // Move the main camera to the specified position and rotation
    }

    public void MoveToTurbine(string turbineID)
    {
        if (turbineCameraPositions.TryGetValue(turbineID, out Transform targetPosition))
        {
            MoveCameraToPosition(targetPosition);
        }
        else
        {
            Debug.LogError($"No camera position found for turbine ID: {turbineID}");
        }
    }

    private void MoveCameraToPosition(Transform targetPosition)
    {
        if (targetPosition != null)
        {
            // Move camera instantly (optional for instant snapping)
            mainCamera.transform.position = targetPosition.position;
            mainCamera.transform.rotation = targetPosition.rotation;

            // Uncomment for smooth camera transition
            // StartCoroutine(SmoothMoveCamera(targetPosition));
        }
        else
        {
            Debug.LogError("Invalid camera position!");
        }
    }

    private void MoveCameraToPosition(int index)
    {
        if (index >= 0 && index < cameraPositions.Length)
        {
            MoveCameraToPosition(cameraPositions[index]);
        }
        else
        {
            Debug.LogError("Invalid index for camera position!");
        }
    }

    // Optional: Smooth camera movement using a coroutine
    private System.Collections.IEnumerator SmoothMoveCamera(Transform targetPosition)
    {
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;

        Vector3 targetPositionVector = targetPosition.position;
        Quaternion targetRotation = targetPosition.rotation;

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPositionVector, elapsedTime);
            mainCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * transitionSpeed;
            yield return null;
        }

        mainCamera.transform.position = targetPositionVector;
        mainCamera.transform.rotation = targetRotation;
    }
}
