using UnityEngine;

public class RaceProgress : MonoBehaviour
{
    public int currentCheckpointIndex = 0; // The last checkpoint passed
    public float distanceToNextCheckpoint = 0f; // Distance to the next checkpoint
    public float totalProgress = 0f; // Overall race progress (used for ranking)

    private Transform[] checkpoints;

    private void Start()
    {
        checkpoints = TrackManager.Instance.GetCheckpoints(); // Get checkpoints from TrackManager
    }

    private void Update()
    {
        if (checkpoints == null || checkpoints.Length == 0) return;

        // Calculate distance to the next checkpoint
        Transform nextCheckpoint = checkpoints[currentCheckpointIndex];
        distanceToNextCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);

        // Update total progress
        totalProgress = currentCheckpointIndex + (1f - distanceToNextCheckpoint / TrackManager.Instance.GetCheckpointDistance(currentCheckpointIndex));
    }

    public void PassCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex == (currentCheckpointIndex + 1) % checkpoints.Length)
        {
            currentCheckpointIndex = checkpointIndex;
        }
    }

    public float GetProgress()
    {
        return totalProgress;
    }
}
