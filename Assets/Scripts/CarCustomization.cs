using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class CarCustomization : MonoBehaviourPunCallbacks
{
    public Renderer carRenderer; // Renderer for the car body
    public Material[] availableMaterials; // Array of materials for car colors or designs

    // Player upgrade stats
    public float baseSpeed = 100f;
    public float baseHandling = 5f;
    public float baseAcceleration = 10f;
    public float baseToughness = 100f;

    private float speedUpgrade;
    private float handlingUpgrade;
    private float accelerationUpgrade;
    private float toughnessUpgrade;

    // Dynamic stats after upgrades
    public float CurrentSpeed => baseSpeed + speedUpgrade;
    public float CurrentHandling => baseHandling + handlingUpgrade;
    public float CurrentAcceleration => baseAcceleration + accelerationUpgrade;
    public float CurrentToughness => baseToughness + toughnessUpgrade;

    private void Start()
    {
        if (carRenderer == null)
        {
            carRenderer = GetComponentInChildren<Renderer>(); // Or your specific car model
        }
        // Apply initial customization settings (either from saved data or defaults)
        ApplyCarCustomization();
    }

    // Upgrade methods for each stat
    public void UpgradeSpeed(float amount)
    {
        speedUpgrade += amount;
        SyncCarStats();
    }

    public void UpgradeHandling(float amount)
    {
        handlingUpgrade += amount;
        SyncCarStats();
    }

    public void UpgradeAcceleration(float amount)
    {
        accelerationUpgrade += amount;
        SyncCarStats();
    }

    public void UpgradeToughness(float amount)
    {
        toughnessUpgrade += amount;
        SyncCarStats();
    }

    // Sync the car stats across the network
    private void SyncCarStats()
    {
        photonView.RPC("UpdateCarStats", RpcTarget.AllBuffered, speedUpgrade, handlingUpgrade, accelerationUpgrade, toughnessUpgrade);
    }

    [PunRPC]
    private void UpdateCarStats(float speed, float handling, float acceleration, float toughness)
    {
        speedUpgrade = speed;
        handlingUpgrade = handling;
        accelerationUpgrade = acceleration;
        toughnessUpgrade = toughness;
    }

    // Apply saved customization settings
    public void ApplyCarCustomization()
    {
        // Load stats
        speedUpgrade = PlayerPrefs.GetFloat($"{gameObject.name}_SpeedUpgrade", 0);
        handlingUpgrade = PlayerPrefs.GetFloat($"{gameObject.name}_HandlingUpgrade", 0);
        accelerationUpgrade = PlayerPrefs.GetFloat($"{gameObject.name}_AccelerationUpgrade", 0);
        toughnessUpgrade = PlayerPrefs.GetFloat($"{gameObject.name}_ToughnessUpgrade", 0);

        // Load saved color
        int materialIndex = PlayerPrefs.GetInt("CarColor", 0);
        if (materialIndex >= 0 && materialIndex < availableMaterials.Length)
        {
            carRenderer.material = availableMaterials[materialIndex];
            photonView.RPC("SyncCarColor", RpcTarget.AllBuffered, materialIndex);
        }

        // Sync the car stats across the network
        SyncCarStats();
    }

    [PunRPC]
    private void SyncCarColor(int materialIndex)
    {
        if (materialIndex >= 0 && materialIndex < availableMaterials.Length)
        {
            carRenderer.material = availableMaterials[materialIndex];
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // Get saved material index
        int materialIndex = PlayerPrefs.GetInt("CarColor", 0);

        // Synchronize color across the network
        photonView.RPC("SyncCarColor", RpcTarget.AllBuffered, materialIndex);

        // Apply customization settings
        ApplyCarCustomization();
    }

    // Save the car's customization data
    public void SaveCarCustomization()
    {
        PlayerPrefs.SetFloat($"{gameObject.name}_SpeedUpgrade", speedUpgrade);
        PlayerPrefs.SetFloat($"{gameObject.name}_HandlingUpgrade", handlingUpgrade);
        PlayerPrefs.SetFloat($"{gameObject.name}_AccelerationUpgrade", accelerationUpgrade);
        PlayerPrefs.SetFloat($"{gameObject.name}_ToughnessUpgrade", toughnessUpgrade);

        int materialIndex = Array.IndexOf(availableMaterials, carRenderer.material);
        PlayerPrefs.SetInt("CarColor", materialIndex >= 0 ? materialIndex : 0);

        PlayerPrefs.Save();
    }

    public void ChangeCarColor(int materialIndex)
    {
        if (materialIndex >= 0 && materialIndex < availableMaterials.Length)
        {
            carRenderer.material = availableMaterials[materialIndex];
            photonView.RPC("SyncCarColor", RpcTarget.AllBuffered, materialIndex);
        }
        else
        {
            Debug.LogError("Invalid material index.");
        }
    }
}