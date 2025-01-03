using UnityEngine;
using Photon.Pun;

[CreateAssetMenu(menuName = "PowerUps/Shock")]
public class ShockPowerUp : PowerUp
{
    public GameObject empDomePrefab;
    public int numberOfDomes = 3;
    public float spacing = 5f;

    public override void ApplyEffect(Transform spawnPoint, GameObject user)
    {
        GameObject targetPlayer = FindFirstPlacePlayer(user);
        if (targetPlayer == null) return;

        Vector3 startPosition = targetPlayer.transform.position + targetPlayer.transform.forward * spacing;

        for (int i = 0; i < numberOfDomes; i++)
        {
            Vector3 offset = i % 2 == 0 ? Vector3.left : Vector3.right;
            Vector3 domePosition = startPosition + offset * 2f + targetPlayer.transform.forward * i * spacing;
            PhotonNetwork.Instantiate(empDomePrefab.name, domePosition, Quaternion.identity);
        }
    }

    private GameObject FindFirstPlacePlayer(GameObject user)
    {
        GameObject firstPlace = null;
        float maxProgress = -1;

        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player == user) continue;

            RaceProgress raceProgress = player.GetComponent<RaceProgress>();
            if (raceProgress != null && raceProgress.GetProgress() > maxProgress)
            {
                maxProgress = raceProgress.GetProgress();
                firstPlace = player;
            }
        }

        return firstPlace;
    }
}