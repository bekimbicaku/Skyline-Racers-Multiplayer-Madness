using Photon.Pun;
using UnityEngine;

public class StartPositionManager : MonoBehaviour
{
    public Transform[] startPositions; // Player start positions
    private bool[] positionOccupied;

    void Start()
    {
        positionOccupied = new bool[startPositions.Length];
    }

    public Transform AssignStartPosition()
    {
        int actorIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // Unique index per player
        if (actorIndex < startPositions.Length)
        {
            positionOccupied[actorIndex] = true;
            return startPositions[actorIndex];
        }
        else
        {
            Debug.LogWarning("Not enough spawn positions for all players.");
            return null;
        }
    }

    public void ReleasePosition(Transform position)
    {
        int index = System.Array.IndexOf(startPositions, position);
        if (index >= 0)
            positionOccupied[index] = false;
    }
}
