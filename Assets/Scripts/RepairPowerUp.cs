using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Repair")]
public class RepairPowerUp : PowerUp
{
    public float repairAmount = 50f;

    public override void ApplyEffect(Transform spawnPoint, GameObject user)
    {
        PlayerManager playerManager = user.GetComponent<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.RepairDamage(repairAmount);
        }
    }
}