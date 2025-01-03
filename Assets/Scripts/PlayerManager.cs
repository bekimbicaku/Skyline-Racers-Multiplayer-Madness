using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerManager : MonoBehaviourPun
{
    [Header("Power-Up Spawn Points")]
    public Transform frontSpawnPoint;
    public Transform backSpawnPoint;

    [Header("UI Elements")]
    public GameObject powerUpButtonUI1;
    public GameObject powerUpButtonUI2;

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
            powerUpButtonUI1.GetComponent<UnityEngine.UI.Image>().sprite = currentPowerUp1.icon;
            powerUpButtonUI1.SetActive(true);
        }
        else if (slot == 2)
        {
            powerUpButtonUI2.GetComponent<UnityEngine.UI.Image>().sprite = currentPowerUp2.icon;
            powerUpButtonUI2.SetActive(true);
        }
    }

    public void ActivatePowerUp(int slot)
    {
        if (slot == 1 && currentPowerUp1 != null)
        {
            photonView.RPC("UsePowerUp", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, 1);
            currentPowerUp1 = null;
            powerUpButtonUI1.SetActive(false);
        }
        else if (slot == 2 && currentPowerUp2 != null)
        {
            photonView.RPC("UsePowerUp", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, 2);
            currentPowerUp2 = null;
            powerUpButtonUI2.SetActive(false);
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
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == playerID)
            {
                return player.TagObject as GameObject;
            }
        }
        return null;
    }

    public void ActivateShield(float shieldDuration)
    {
        if (isShieldActive) return;

        isShieldActive = true;
        shieldEndTime = Time.time + shieldDuration;

        if (shieldEffectPrefab != null)
        {
            GameObject shieldEffect = Instantiate(shieldEffectPrefab, transform.position, Quaternion.identity, transform);
            Destroy(shieldEffect, shieldDuration);
        }

        StartCoroutine(ShieldDurationCoroutine(shieldDuration));
    }

    private IEnumerator ShieldDurationCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        isShieldActive = false;
    }

    public void RepairDamage(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log($"{photonView.Owner.NickName} repaired {amount} health!");
    }

    public void ActivateNitro(float nitroDuration)
    {
        StartCoroutine(NitroBoostCoroutine(nitroDuration));
    }

    private IEnumerator NitroBoostCoroutine(float duration)
    {
        float originalSpeed = currentSpeed;
        currentSpeed += nitroBoostAmount;
        Debug.Log("Nitro activated!");
        yield return new WaitForSeconds(duration);
        currentSpeed = originalSpeed;
        Debug.Log("Nitro ended!");
    }
}