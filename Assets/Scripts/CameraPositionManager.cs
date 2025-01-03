using UnityEngine;

public class CameraPositionManager : MonoBehaviour
{
    [Header("Camera Targets")]
    public Transform mainPanelPosition;  // Position for Main Panel
    public Transform garagePanelPosition;  // Position for Garage Panel

    [Header("Transition Settings")]
    public float transitionSpeed = 5f;  // Speed of the camera transition

    private Transform targetPosition;  // Current target position for the camera
    private bool isTransitioning = false;

    private void Start()
    {
        // Initialize camera position to main panel by default
        if (mainPanelPosition != null)
        {
            transform.position = mainPanelPosition.position;
            transform.rotation = mainPanelPosition.rotation;
            targetPosition = mainPanelPosition;
        }
    }

    private void Update()
    {
        if (isTransitioning && targetPosition != null)
        {
            // Smoothly move the camera to the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition.position, transitionSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetPosition.rotation, transitionSpeed * Time.deltaTime);

            // Stop transitioning when close enough to the target
            if (Vector3.Distance(transform.position, targetPosition.position) < 0.1f &&
                Quaternion.Angle(transform.rotation, targetPosition.rotation) < 1f)
            {
                isTransitioning = false;
            }
        }
    }

    /// <summary>
    /// Switch the camera to the Main Panel position.
    /// </summary>
    public void MoveToMainPanel()
    {
        if (mainPanelPosition != null)
        {
            targetPosition = mainPanelPosition;
            isTransitioning = true;
        }
    }

    /// <summary>
    /// Switch the camera to the Garage Panel position.
    /// </summary>
    public void MoveToGaragePanel()
    {
        if (garagePanelPosition != null)
        {
            targetPosition = garagePanelPosition;
            isTransitioning = true;
        }
    }
}
