using UnityEngine;

public class BotAI : MonoBehaviour
{
    public float speed = 10f;
    private Transform[] waypoints;
    private int currentWaypoint = 0;

    private void Start()
    {
        WaypointManager waypointManager = FindObjectOfType<WaypointManager>();
        if (waypointManager != null)
        {
            waypoints = waypointManager.waypoints;
        }
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0 || waypoints[currentWaypoint].gameObject.activeSelf == false) return;

        // Move towards the next waypoint
        Transform target = waypoints[currentWaypoint];
        Vector3 direction = target.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        // Check if the bot has reached the waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }
}