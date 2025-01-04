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
    private int currentBotCount = 0;

    private List<string> botNames = new List<string>
    {
        "BotAlpha", "BotBeta", "BotGamma", "BotDelta", "BotEpsilon", "BotZeta", "BotEta", "BotTheta"
    };

    public void StartAddingBots()
    {
        // Start coroutine to add bots sequentially
        StartCoroutine(AddBotsSequentially());
    }

    private void CreateBot()
    {
        if (currentBotCount >= maxBotsPerRoom) return;

        // Instantiate bot at a random spawn point
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        GameObject bot = PhotonNetwork.Instantiate(botPrefab.name, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);

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
            botPhotonView.Owner.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "PlayerName", botName } });
        }

        currentBotCount++;
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
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player left room: {otherPlayer.NickName}");
        if (otherPlayer.NickName.StartsWith("Bot"))
        {
            currentBotCount--;
        }
    }
}