using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

public class PowerUpBehavior : MonoBehaviourPun
{
    public PowerUp powerUpData;

    [PunRPC]
    public void InitializePowerUp()
    {
        // Set appearance or behaviors based on powerUpData
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerView = other.GetComponent<PhotonView>();
            if (playerView.IsMine)
            {
                CollectPowerUp(playerView);
            }
        }
    }

    private void CollectPowerUp(PhotonView playerView)
    {
        photonView.RPC("HandleCollection", RpcTarget.AllBuffered);
        playerView.GetComponent<PlayerManager>().AddPowerUp(powerUpData);
    }

    [PunRPC]
    public void HandleCollection()
    {
        // Disable power-up visuals for the collector
        gameObject.SetActive(false);

        // Respawn after 2 seconds
        Invoke(nameof(Respawn), 2f);
    }

    private void Respawn()
    {
        gameObject.SetActive(true);
    }
}
