using UnityEngine;

public class CanvasAndCameraPositionSwitcher : MonoBehaviour
{
    public Canvas[] canvases;          // Array to hold all canvases
    public Transform[] cameraPositions; // Array to store predefined camera positions and rotations
    public Camera mainCamera;          // Reference to the main camera
    public float transitionSpeed = 5f; // Speed of the camera transition (optional for smooth movement)

    void Start()
    {
        // Start by activating the first canvas and positioning the main camera
        SwitchToView(0);
    }

    public void SwitchToView(int index)
    {
        // Ensure only the selected canvas is active
        for (int i = 0; i < canvases.Length; i++)
        {
            canvases[i].gameObject.SetActive(i == index);
        }

        // Move the main camera to the specified position and rotation
        MoveCameraToPosition(index);
    }

    private void MoveCameraToPosition(int index)
    {
        if (index >= 0 && index < cameraPositions.Length)
        {
            // Move camera instantly (optional for instant snapping)
            mainCamera.transform.position = cameraPositions[index].position;
            mainCamera.transform.rotation = cameraPositions[index].rotation;

            // Uncomment for smooth camera transition
            // StartCoroutine(SmoothMoveCamera(index));
        }
        else
        {
            Debug.LogError("Invalid index for camera position!");
        }
    }

    // Optional: Smooth camera movement using a coroutine
    private System.Collections.IEnumerator SmoothMoveCamera(int index)
    {
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;

        Vector3 targetPosition = cameraPositions[index].position;
        Quaternion targetRotation = cameraPositions[index].rotation;

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            mainCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * transitionSpeed;
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;
    }
}
