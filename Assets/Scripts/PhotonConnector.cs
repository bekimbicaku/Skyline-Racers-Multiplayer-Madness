using UnityEngine;
using Photon.Pun; // Photon PUN namespace
using Photon.Realtime;

public class PhotonConnector : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        // Connect to Photon
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // Connect using default settings
        }
    }

    // Callback method when connected to Photon
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server.");
        // Optionally, join a room or create one here
    }

    // Callback method when connection fails
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from Photon. Cause: " + cause.ToString());
    }
}
