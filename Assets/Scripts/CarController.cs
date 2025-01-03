using UnityEngine;
using Photon.Pun;

public class CarController : MonoBehaviourPun
{
    public CarStats carStats;
    public float nitroMultiplier = 1.5f;

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private bool isNitroActive = false;
    private bool raceStarted = false; // Indicates whether the race has started

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = 0f; // Ensure the car starts at 0 speed
    }

    void Update()
    {
        if (photonView.IsMine && raceStarted) // Only process controls if race has started
        {
            HandleControls();
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine && raceStarted) // Only move the car if race has started
        {
            MoveCar();
        }
    }

    private void HandleControls()
    {
        if (Input.GetKeyDown(KeyCode.N)) // Nitro button
        {
            ActivateNitro();
        }

        if (Input.GetKey(KeyCode.Space)) // Drift button
        {
            // Implement drifting logic here
        }

        if (Input.GetKey(KeyCode.B)) // Brake button
        {
            BrakeCar();
        }
    }

    private void MoveCar()
    {
        // Automatic acceleration
        currentSpeed += carStats.Acceleration * Time.fixedDeltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, 0, carStats.Speed);

        // Apply forward movement
        Vector3 forwardMovement = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMovement);

        // Turning
        float turnInput = Input.GetAxis("Horizontal");
        float turnStrength = carStats.Handling;
        Vector3 rotation = Vector3.up * turnInput * turnStrength * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
    }

    private void ActivateNitro()
    {
        if (!isNitroActive)
        {
            StartCoroutine(NitroBoost(carStats.NitroDuration));
        }
    }

    private System.Collections.IEnumerator NitroBoost(float duration)
    {
        isNitroActive = true;
        float originalSpeed = carStats.Speed;
        carStats.speedMultiplier *= nitroMultiplier;

        yield return new WaitForSeconds(duration);

        carStats.speedMultiplier /= nitroMultiplier;
        isNitroActive = false;
    }

    private void BrakeCar()
    {
        currentSpeed -= carStats.Acceleration * 2 * Time.fixedDeltaTime;
        currentSpeed = Mathf.Max(0, currentSpeed);
    }

    // Public method to start the race
    public void StartRace()
    {
        raceStarted = true;
    }
}
