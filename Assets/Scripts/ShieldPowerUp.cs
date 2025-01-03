using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Shield")]
public class ShieldPowerUp : PowerUp
{
    public GameObject shieldEffectPrefab;

    public override void ApplyEffect(Transform spawnPoint, GameObject user)
    {
        PlayerManager playerManager = user.GetComponent<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.ActivateShield(duration);
        }
    }
}