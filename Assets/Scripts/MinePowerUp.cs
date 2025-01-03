using Photon.Pun;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Mine")]
public class MinePowerUp : PowerUp
{
    public override void ApplyEffect(Transform spawnPoint, GameObject user)
    {
        PhotonNetwork.Instantiate(effectPrefab.name, spawnPoint.position, Quaternion.identity);
    }
}