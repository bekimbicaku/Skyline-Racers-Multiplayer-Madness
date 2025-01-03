using UnityEngine;
using Photon.Pun;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;
    public GameObject carPrefab;

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            Transform spawnPoint = spawnPoints[spawnIndex % spawnPoints.Length];
            PhotonNetwork.Instantiate(carPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
