using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public static TrackManager Instance;

    [SerializeField]
    private Transform[] checkpoints;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Transform[] GetCheckpoints()
    {
        return checkpoints;
    }

    public float GetCheckpointDistance(int checkpointIndex)
    {
        if (checkpointIndex < 0 || checkpointIndex >= checkpoints.Length) return 0f;

        int nextIndex = (checkpointIndex + 1) % checkpoints.Length;
        return Vector3.Distance(checkpoints[checkpointIndex].position, checkpoints[nextIndex].position);
    }
}
