using Photon.Pun;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Bolt")]
public class BoltPowerUp : PowerUp
{
    public GameObject boltPrefab;

    public override void ApplyEffect(Transform spawnPoint, GameObject user)
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 offset = Vector3.up * i * 0.2f;
            PhotonNetwork.Instantiate(boltPrefab.name, spawnPoint.position + offset, spawnPoint.rotation);
        }
    }
}