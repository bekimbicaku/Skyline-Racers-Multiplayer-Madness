using UnityEngine;

public class OffensiveEffect : MonoBehaviour
{
    private GameObject user;

    public void Initialize(GameObject initiator)
    {
        user = initiator;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject != user)
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.TakeDamage(10f); // Replace 10f with a configurable damage value
            }

            Destroy(gameObject); // Destroy the effect after hitting a target
        }
    }
}
