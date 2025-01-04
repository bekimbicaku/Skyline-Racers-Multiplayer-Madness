using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;

public class BotManager : MonoBehaviourPunCallbacks
{
    public GameObject botPrefab;
    public Transform[] spawnPoints;
    public int maxBotsPerRoom = 3;
    public int currentBotCount = 0;

    private List<string> botNames = new List<string>
    {
        "BotAlpha", "BotBeta", "BotGamma", "BotDelta", "BotEpsilon", "BotZeta", "BotEta", "BotTheta"
    };

    private List<Transform> usedSpawnPoints = new List<Transform>();

    public void StartAddingBots()
    {
        // Start coroutine to add bots sequentially
        StartCoroutine(AddBotsSequentially());
    }

    private void CreateBot()
    {
        if (currentBotCount >= maxBotsPerRoom) return;

        Transform spawnPoint = GetFreeSpawnPoint();
        if (spawnPoint == null)
        {
            Debug.LogWarning("No free spawn points available for bots");
            return;
        }

        // Instantiate bot at the free spawn point
        GameObject bot = PhotonNetwork.Instantiate(botPrefab.name, spawnPoint.position, spawnPoint.rotation);

        // Add BotAI component to the bot
        bot.AddComponent<BotAI>();

        // Assign a random name to the bot
        int nameIndex = Random.Range(0, botNames.Count);
        string botName = botNames[nameIndex];
        botNames.RemoveAt(nameIndex); // Remove the name from the list to avoid duplicates

        // Set the bot name in Photon custom properties
        PhotonView botPhotonView = bot.GetComponent<PhotonView>();
        if (botPhotonView != null)
        {
            botPhotonView.Owner.NickName = botName;
            botPhotonView.Owner.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "PlayerName", botName }, { "IsBot", true } });
        }

        // Mark the spawn point as used
        usedSpawnPoints.Add(spawnPoint);

        currentBotCount++;
    }

    private Transform GetFreeSpawnPoint()
    {
        // Find a spawn point that is not used
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (!usedSpawnPoints.Contains(spawnPoint))
            {
                return spawnPoint;
            }
        }
        return null;
    }

    private IEnumerator AddBotsSequentially()
    {
        while (currentBotCount < maxBotsPerRoom)
        {
            yield return new WaitForSeconds(2f); // Wait before adding the next bot
            CreateBot();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player entered room: {newPlayer.NickName}");
        UpdateRoomStatus();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player left room: {otherPlayer.NickName}");
        if (otherPlayer.CustomProperties.ContainsKey("IsBot"))
        {
            currentBotCount--;
        }
        UpdateRoomStatus();
    }

    private void UpdateRoomStatus()
    {
        int totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount + currentBotCount;
        Debug.Log($"Room status updated: {totalPlayers} players in room.");
    }
}