using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the switching of canvases and camera positions for different views or turbines.
/// </summary>
public class CanvasAndCameraPositionSwitcher : MonoBehaviour
{
    /// <summary>
    /// Array to hold all canvases used in the scene.
    /// </summary>
    public Canvas[] canvases;

    /// <summary>
    /// Array to store predefined camera positions and rotations.
    /// </summary>
    public Transform[] cameraPositions;

    /// <summary>
    /// Reference to the main camera in the scene.
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// Speed of the camera transition for smooth movement.
    /// </summary>
    public float transitionSpeed = 5f;

    /// <summary>
    /// Dictionary mapping turbine IDs to specific camera positions.
    /// </summary>
    private Dictionary<string, Transform> turbineCameraPositions;

    /// <summary>
    /// Initializes the turbine-to-camera mapping and sets the initial canvas and camera position.
    /// </summary>
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
            { "T107", cameraPositions[11] }
        };

        // Start by activating the first canvas and positioning the main camera
        SwitchToView(0);
    }

    /// <summary>
    /// Switches to a specific view by activating the corresponding canvas and moving the camera.
    /// </summary>
    /// <param name="index">The index of the view to switch to.</param>
    public void SwitchToView(int index)
    {
        MoveCameraToPosition(index);

        if (index == 0)
        {
            canvases[0].gameObject.SetActive(true);
            canvases[1].gameObject.SetActive(false);
        }
        else
        {
            canvases[0].gameObject.SetActive(false);
            canvases[1].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Moves the camera to a position associated with a specific turbine ID.
    /// </summary>
    /// <param name="turbineID">The ID of the turbine to focus on.</param>
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

    /// <summary>
    /// Instantly moves the camera to a specified target position and rotation.
    /// </summary>
    /// <param name="targetPosition">The target position and rotation for the camera.</param>
    private void MoveCameraToPosition(Transform targetPosition)
    {
        if (targetPosition != null)
        {
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

    /// <summary>
    /// Moves the camera to a position based on the index in the camera positions array.
    /// </summary>
    /// <param name="index">The index of the target camera position.</param>
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

    /// <summary>
    /// Smoothly moves the camera to a specified position and rotation over time.
    /// </summary>
    /// <param name="targetPosition">The target position and rotation for the camera.</param>
    /// <returns>An enumerator for coroutine execution.</returns>
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
