using UnityEngine;
using Photon.Pun;

public class PowerUpInstance : MonoBehaviour
{
    private Transform spawnPoint;

    public void Initialize(Transform point)
    {
        spawnPoint = point;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.AddPowerUp(GetComponent<PowerUp>());
                PhotonNetwork.Destroy(gameObject); // Destroy the power-up after collection

                // Respawn the power-up at this spawn point after a delay
                if (PhotonNetwork.IsMasterClient)
                {
                    Invoke(nameof(Respawn), 2f);
                }
            }
        }
    }

    private void Respawn()
    {
        PhotonNetwork.Instantiate(gameObject.name, spawnPoint.position, spawnPoint.rotation);
    }
}
