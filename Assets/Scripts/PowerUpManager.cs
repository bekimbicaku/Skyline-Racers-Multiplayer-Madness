using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviourPun
{
    public GameObject[] powerUpPrefabs;
    public Transform[] spawnPoints;
    public List<string> offensivePowers = new List<string> { "Barge", "Bolt", "Mine", "Shock" };
    public List<string> defensivePowers = new List<string> { "Repair", "Shield", "Nitro", "Shunt" };

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnPowerUps();
        }
    }

    [PunRPC]
    public void SpawnPowerUps()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            int index = Random.Range(0, powerUpPrefabs.Length);
            GameObject powerUp = PhotonNetwork.Instantiate(powerUpPrefabs[index].name, spawnPoint.position, Quaternion.identity);
            PhotonView photonView = powerUp.GetComponent<PhotonView>();
            photonView.RPC("InitializePowerUp", RpcTarget.AllBuffered);
        }
    }

    public string GetRandomOffensivePower()
    {
        return offensivePowers[Random.Range(0, offensivePowers.Count)];
    }

    public string GetRandomDefensivePower()
    {
        return defensivePowers[Random.Range(0, defensivePowers.Count)];
    }
}