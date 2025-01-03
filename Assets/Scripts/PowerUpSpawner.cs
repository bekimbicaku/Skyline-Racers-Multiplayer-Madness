using UnityEngine;
using Photon.Pun;

public class PowerUpSpawner : MonoBehaviourPun
{
    public GameObject questionPrefab;
    public GameObject exclamationPrefab;
    public Transform[] spawnPoints;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnAllPowerUps();
        }
    }

    private void SpawnAllPowerUps()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            SpawnPowerUpAtPoint(spawnPoint);
        }
    }

    public void SpawnPowerUpAtPoint(Transform spawnPoint)
    {
        GameObject prefabToSpawn = (Random.value > 0.5f) ? questionPrefab : exclamationPrefab;
        PhotonNetwork.Instantiate(prefabToSpawn.name, spawnPoint.position, spawnPoint.rotation);
    }
}