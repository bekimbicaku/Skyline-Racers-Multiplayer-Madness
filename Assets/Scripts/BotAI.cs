using UnityEngine;

public class BotAI : MonoBehaviour
{
    public float speed = 10f;
    private Transform[] waypoints;
    private int currentWaypoint = 0;
    private Transform botTransform;

    private void Start()
    {
        botTransform = transform;
        WaypointManager waypointManager = FindObjectOfType<WaypointManager>();
        if (waypointManager != null)
        {
            waypoints = waypointManager.waypoints;
        }
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0 || waypoints[currentWaypoint] == null || !waypoints[currentWaypoint].gameObject.activeSelf) return;

        // Move towards the next waypoint
        Transform target = waypoints[currentWaypoint];
        Vector3 direction = target.position - botTransform.position;
        botTransform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        // Check if the bot has reached the waypoint
        if (Vector3.Distance(botTransform.position, target.position) < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }
}