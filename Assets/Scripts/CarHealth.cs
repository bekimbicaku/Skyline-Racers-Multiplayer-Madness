using Photon.Pun;
using UnityEngine;
public class CarHealth : MonoBehaviourPunCallbacks
{
    public float carHealth;
    private CarCustomization carCustomization;

    void Start()
    {
        carCustomization = GetComponent<CarCustomization>();
    }

    public void TakeDamage(float damage)
    {
        carHealth -= damage;

        if (carHealth <= 0)
        {
            // Handle car destruction (e.g., respawn or game over)
            DestroyCar();
        }
    }

    private void DestroyCar()
    {
        // Logic for destroying the car (e.g., respawn, reset, etc.)
        Debug.Log("Car destroyed!");
    }
}
