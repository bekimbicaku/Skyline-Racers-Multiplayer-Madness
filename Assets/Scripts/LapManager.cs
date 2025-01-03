using UnityEngine;
using Photon.Pun;

public class LapManager : MonoBehaviourPun
{
    public int totalLaps = 3;
    private int currentLap = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && photonView.IsMine)
        {
            currentLap++;

            if (currentLap >= totalLaps)
            {
                Debug.Log("Race Finished!");
                photonView.RPC("AnnounceWinner", RpcTarget.All, PhotonNetwork.NickName);
            }
        }
    }

    [PunRPC]
    void AnnounceWinner(string winnerName)
    {
        Debug.Log($"{winnerName} is the winner!");
    }
}
