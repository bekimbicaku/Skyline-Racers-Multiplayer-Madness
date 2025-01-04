using UnityEngine;
using Photon.Pun;

public class WaypointManager : MonoBehaviourPunCallbacks
{
    public Transform[] waypoints;

    private void Start()
    {
        // Initially deactivate waypoints
        SetWaypointsActive(false);
    }

    // Method to activate or deactivate waypoints
    public void SetWaypointsActive(bool isActive)
    {
        foreach (Transform waypoint in waypoints)
        {
            waypoint.gameObject.SetActive(isActive);
        }
    }

    // Photon callback when the local player joins a room
    public override void OnJoinedRoom()
    {
        // Check if the room is the race room
        if (PhotonNetwork.CurrentRoom.Name.Contains("RaceRoom"))
        {
            SetWaypointsActive(true);
        }
        else
        {
            SetWaypointsActive(false);
        }
    }

    // Photon callback when the local player leaves a room
    public override void OnLeftRoom()
    {
        SetWaypointsActive(false);
    }
}