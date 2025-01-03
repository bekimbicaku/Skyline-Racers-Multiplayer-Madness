using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Barge")]
public class BargePowerUp : PowerUp
{
    public float radius = 10f;
    public float force = 500f;

    public override void ApplyEffect(Transform spawnPoint, GameObject user)
    {
        Collider[] hitColliders = Physics.OverlapSphere(user.transform.position, radius);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Player") && hit.gameObject != user)
            {
                Vector3 pushDirection = hit.transform.position - user.transform.position;
                hit.GetComponent<Rigidbody>().AddForce(pushDirection.normalized * force);
            }
        }
    }
}