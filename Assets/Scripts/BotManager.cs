using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;

public class BotManager : MonoBehaviourPunCallbacks
{
    public GameObject botPrefab;
    public StartPositionManager startPositionManager;
    public int maxBotsPerRoom = 3;
    public int currentBotCount = 0;

    private List<string> botNames = new List<string>
    {
        "BotAlpha", "BotBeta", "BotGamma", "BotDelta", "BotEpsilon", "BotZeta", "BotEta", "BotTheta"
    };

    private HashSet<Transform> usedSpawnPoints = new HashSet<Transform>();

    public void StartAddingBots()
    {
        if (!PhotonNetwork.IsMasterClient) return; // Only the Master Client should add bots

        StartCoroutine(AddBotsSequentially());
    }

    private IEnumerator AddBotsSequentially()
    {
        while (currentBotCount < maxBotsPerRoom)
        {
            yield return new WaitForSeconds(2f); // Wait before adding the next bot
            CreateBot();
        }
    }

    private void CreateBot()
    {
        if (currentBotCount >= maxBotsPerRoom) return;

        Transform spawnPoint = startPositionManager.AssignBotStartPosition();
        if (spawnPoint == null)
        {
            Debug.LogWarning("No free spawn points available for bots");
            return;
        }

        GameObject bot = PhotonNetwork.Instantiate(botPrefab.name, spawnPoint.position, spawnPoint.rotation);
        bot.GetComponent<PhotonView>().Owner.NickName = botNames[currentBotCount % botNames.Count]; // Give the bot a name
        bot.GetComponent<PhotonView>().Owner.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsBot", true } });
        currentBotCount++;

        RoomManager roomManager = FindObjectOfType<RoomManager>();
        roomManager.UpdateRoomStatus(PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers); // Call UpdateRoomStatus whenever a bot is added
    }

   
}