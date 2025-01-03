using UnityEngine;
using Photon.Pun;

public class PowerUpPickup : MonoBehaviourPun
{
    public GameObject destroyVFXPrefab;
    public float respawnTime = 3f;

    private Transform spawnPoint;

    public void Initialize(Transform spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && photonView.IsMine)
        {
            photonView.RPC(nameof(HandlePickup), RpcTarget.All);
        }
    }

    [PunRPC]
    private void HandlePickup()
    {
        if (destroyVFXPrefab != null)
        {
            Instantiate(destroyVFXPrefab, transform.position, Quaternion.identity);
        }
        PhotonNetwork.Destroy(gameObject);
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(RespawnPowerUp());
        }
    }

    private System.Collections.IEnumerator RespawnPowerUp()
    {
        yield return new WaitForSeconds(respawnTime);

        GameObject spawner = FindObjectOfType<PowerUpSpawner>().gameObject;
        spawner.GetComponent<PowerUpSpawner>().SpawnPowerUpAtPoint(spawnPoint);
    }
}