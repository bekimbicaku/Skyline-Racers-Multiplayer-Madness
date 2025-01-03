using UnityEngine;

[System.Serializable]
public class CarStats
{
    public float baseSpeed = 100f;
    public float baseAcceleration = 10f;
    public float baseHandling = 5f;
    public float baseNitroDuration = 5f; // Nitro active time in seconds
    public float baseToughness = 100f; // Car durability

    [HideInInspector] public float speedMultiplier = 1f;
    [HideInInspector] public float accelerationMultiplier = 1f;
    [HideInInspector] public float handlingMultiplier = 1f;
    [HideInInspector] public float toughnessMultiplier = 1f;

    public float Speed => baseSpeed * speedMultiplier;
    public float Acceleration => baseAcceleration * accelerationMultiplier;
    public float Handling => baseHandling * handlingMultiplier;
    public float NitroDuration => baseNitroDuration;
    public float Toughness => baseToughness * toughnessMultiplier;

    public void ApplyUpgrade(string stat, float value)
    {
        switch (stat.ToLower())
        {
            case "Speed":
                speedMultiplier += value;
                break;
            case "Acceleration":
                accelerationMultiplier += value;
                break;
            case "Handling":
                handlingMultiplier += value;
                break;
            case "Toughness":
                toughnessMultiplier += value;
                break;
            default:
                Debug.LogWarning($"Unknown stat: {stat}");
                break;
        }
    }
}
