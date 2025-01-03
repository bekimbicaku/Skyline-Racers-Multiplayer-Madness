using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/PowerUp")]
public class PowerUp : ScriptableObject
{
    public string powerUpName;
    public Sprite icon;
    public float duration; // For timed effects (e.g., Shield, Nitro)
    public float damage;   // Damage dealt by offensive power-ups
    public GameObject effectPrefab; // Visual effect for the power-up

    public enum PowerUpType { Offensive, Defensive, Utility }
    public PowerUpType type;

    public virtual void ApplyEffect(Transform target, GameObject user)
    {
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, target.position, target.rotation);
        }

        if (type == PowerUpType.Offensive)
        {
            // Offensive effect logic
        }
        else if (type == PowerUpType.Defensive)
        {
            // Defensive effect logic
            PlayerManager playerManager = user.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                if (powerUpName == "Shield")
                {
                    playerManager.ActivateShield(duration);
                }
                else if (powerUpName == "Repair")
                {
                    playerManager.RepairDamage(damage); // Use the damage field as the healing value
                }
            }
        }
        else if (type == PowerUpType.Utility)
        {
            // Utility effect logic
            PlayerManager playerManager = user.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                if (powerUpName == "Nitro")
                {
                    playerManager.ActivateNitro(duration);
                }
            }
        }
    }
}