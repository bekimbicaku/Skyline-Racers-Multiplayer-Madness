using UnityEngine;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPun
{
    [Header("Power-Up Spawn Points")]
    public Transform frontSpawnPoint;
    public Transform backSpawnPoint;

    [Header("UI Elements")]
    private RaceManager raceManager;
   
    [Header("Player Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Shield Settings")]
    public GameObject shieldEffectPrefab;
    private bool isShieldActive = false;
    private float shieldEndTime;

    [Header("Nitro Settings")]
    public float nitroBoostAmount = 10f;
    private float currentSpeed;

    private PowerUp currentPowerUp1;
    private PowerUp currentPowerUp2;

    private void Start()
    {
        raceManager = FindObjectOfType<RaceManager>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (!photonView.IsMine) return;

        if (isShieldActive)
        {
            Debug.Log("Shield absorbed the damage!");
            return;
        }

        currentHealth -= damage;
        Debug.Log($"{photonView.Owner.NickName} took {damage} damage!");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{photonView.Owner.NickName} has been eliminated!");
        // Additional death logic (respawn, remove player, etc.)
    }

    public void AddPowerUp(PowerUp powerUp)
    {
        if (currentPowerUp1 == null)
        {
            currentPowerUp1 = powerUp;
            UpdateUI(1);
        }
        else if (currentPowerUp2 == null)
        {
            currentPowerUp2 = powerUp;
            UpdateUI(2);
        }
        else
        {
            Debug.Log("Both power-up slots are full!");
        }
    }

    private void UpdateUI(int slot)
    {
        if (slot == 1)
        {
            raceManager.powerUpButtonUI1.GetComponent<UnityEngine.UI.Image>().sprite = currentPowerUp1.icon;
            raceManager.powerUpButtonUI1.SetActive(true);
        }
        else if (slot == 2)
        {
            raceManager.powerUpButtonUI2.GetComponent<UnityEngine.UI.Image>().sprite = currentPowerUp2.icon;
            raceManager.powerUpButtonUI2.SetActive(true);
        }
    }

    public void ActivatePowerUp(int slot)
    {
        if (slot == 1 && currentPowerUp1 != null)
        {
            photonView.RPC("UsePowerUp", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, 1);
            currentPowerUp1 = null;
            raceManager.powerUpButtonUI1.SetActive(false);
        }
        else if (slot == 2 && currentPowerUp2 != null)
        {
            photonView.RPC("UsePowerUp", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, 2);
            currentPowerUp2 = null;
            raceManager.powerUpButtonUI2.SetActive(false);
        }
    }

    [PunRPC]
    public void UsePowerUp(int playerID, int slot)
    {
        GameObject user = FindPlayerByID(playerID);
        if (user == null) return;

        if (slot == 1 && currentPowerUp1 != null)
        {
            currentPowerUp1.ApplyEffect(frontSpawnPoint, user);
        }
        else if (slot == 2 && currentPowerUp2 != null)
        {
            currentPowerUp2.ApplyEffect(backSpawnPoint, user);
        }
    }

    private GameObject FindPlayerByID(int playerID)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == playerID)
            {
                return player.TagObject as GameObject;
            }
        }
        return null;
    }

    public void ActivateShield(float duration)
    {
        if (isShieldActive) return;

        isShieldActive = true;
        shieldEndTime = Time.time + duration;
        if (shieldEffectPrefab != null)
        {
            Instantiate(shieldEffectPrefab, transform.position, Quaternion.identity, transform);
        }
        StartCoroutine(DeactivateShieldAfterDuration(duration));
    }

    private IEnumerator DeactivateShieldAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        isShieldActive = false;
        Debug.Log("Shield deactivated!");
    }

    public void ActivateNitro(float duration)
    {
        // Implement nitro activation logic
    }

    public void RepairDamage(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Repaired {amount} health. Current health: {currentHealth}");
    }
}