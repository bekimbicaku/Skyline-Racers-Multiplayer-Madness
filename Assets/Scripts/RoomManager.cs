using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public StartPositionManager spawnPositionManager; // Spawn position manager
    private bool raceStarted = false;

    private string currentRoomCode; // Code of the room currently created or joined
    private static Dictionary<string, string> roomCodes = new Dictionary<string, string>(); // Stores room codes and corresponding room names
    private PhotonView pv;
    public Camera mainCamera; // Reference to the main camera
    public TMP_InputField roomCodeInput; // Input field for room code
    public Button quickRaceButton, createRoomButton, joinRoomButton, leaveRoomButton; // Added leaveRoomButton
    public TextMeshProUGUI countdownText, roomStatusText, roomCodeDisplay;
    public GameObject menuPanel, gamePanel;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (pv == null)
        {
            Debug.LogError("PhotonView is not attached to RoomManager!");
        }
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        // Automatically spawn if already in a room
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
            SpawnPlayerAtStartPosition();

        menuPanel.SetActive(true);
        gamePanel.SetActive(false);

        // Assign button listeners
        quickRaceButton.onClick.AddListener(() => QuickJoin());
        createRoomButton.onClick.AddListener(() => CreatePrivateRoom());
        joinRoomButton.onClick.AddListener(() =>
        {
            string roomCode = roomCodeInput.text.Trim();
            if (!string.IsNullOrEmpty(roomCode))
            {
                JoinPrivateRoom(roomCode);
            }
            else
            {
                Debug.LogWarning("Room code cannot be empty.");
            }
        });

        leaveRoomButton.onClick.AddListener(() => LeaveRoom());  // Listener for leave room button

        // Update room status when already in a room (if applicable)
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            UpdateRoomStatus(PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);
        }
    }

    // Quick Join logic
    public void QuickJoin()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Debug.LogWarning("Not connected to Photon.");
        }
    }

    // Callback for when joining a random room fails
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No random room available. Creating a new room...");
        CreateQuickRoom();
    }

    // Method to create a room for Quick Join
    private void CreateQuickRoom()
    {
        string roomName = $"QuickRoom_{Random.Range(1000, 9999)}";
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 8,
            IsVisible = true, // The room is visible to others
            IsOpen = true     // The room is open for joining
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    /// <summary>
    /// Create a private room with a unique code.
    /// </summary>
    public void CreatePrivateRoom()
    {
        string roomCode = GenerateUniqueCode();
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 8,  // Limit players to 8
            IsVisible = false, // Invisible to random joins
            IsOpen = true     // Open for code-based joining
        };

        // Save room code for later use
        currentRoomCode = roomCode;
        roomCodes[roomCode] = roomCode;

        PhotonNetwork.CreateRoom(roomCode, roomOptions, TypedLobby.Default);
        Debug.Log($"Created Private Room with Code: {roomCode}");
    }

    /// <summary>
    /// Join a private room using a provided room code.
    /// </summary>
    /// <param name="roomCode">The code of the room to join.</param>
    public void JoinPrivateRoom(string roomCode)
    {
        PhotonNetwork.JoinRoom(roomCode);
        Debug.Log($"Attempting to join Room with Code: {roomCode}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
        UpdateRoomStatus(PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);
        SpawnPlayerAtStartPosition();
        SwitchToGamePanel();

        // Deactivate Main Camera for the local player
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
        }

        // Activate the camera for the player's car
        ActivatePlayerCarCamera();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created: " + PhotonNetwork.CurrentRoom.Name);
        SwitchToGamePanel();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player entered room: {newPlayer.NickName}");
        UpdateRoomStatus(PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);

        // Start the race countdown if the room is full and the local player is the Master Client
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Room is full. Starting race countdown...");
            StartCoroutine(StartRaceCountdown());
        }

        // Sync car customization for the new player
        SyncCarCustomizationForNewPlayer(newPlayer);
    }

    private void SyncCarCustomizationForNewPlayer(Player newPlayer)
    {
        if (newPlayer.CustomProperties.TryGetValue("CarColor", out object carColorIndex))
        {
            int materialIndex = (int)carColorIndex;
            GameObject playerCar = FindPlayerCar(newPlayer);
            if (playerCar != null)
            {
                CarCustomization customization = playerCar.GetComponent<CarCustomization>();
                if (customization != null)
                {
                    // Use RPC to sync car color for the new player
                    customization.photonView.RPC("SyncCarColor", RpcTarget.AllBuffered, materialIndex);
                }
            }
        }
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            SwitchToMenuPanel();
            Debug.Log("Leaving room...");
        }
        else
        {
            Debug.LogWarning("Not in a room to leave.");
        }
    }

    // Callback that is triggered after the player leaves the room
    public override void OnLeftRoom()
    {
        Debug.Log("Left room.");
        PhotonNetwork.ConnectUsingSettings();
        // Update room status when leaving the room
        UpdateRoomStatus(PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.MaxPlayers);

        // Switch UI panels when leaving the room
        SwitchToMenuPanel();
    }

    // Update the room status text
    private void UpdateRoomStatus(int playerCount, int maxPlayers)
    {
        if (PhotonNetwork.InRoom)
        {
            // If the player is still in a room, update the room number and status
            roomStatusText.text = $"Room: {playerCount}/{maxPlayers} players";
        }
        else
        {
            // If the player is not in a room, show the "Not in a room" status
            roomStatusText.text = "Not in a room";
        }
    }

    // Switch to the Menu Panel after leaving the room
    private void SwitchToMenuPanel()
    {
        if (menuPanel != null && gamePanel != null)
        {
            menuPanel.SetActive(true);  // Show the Menu Panel
            gamePanel.SetActive(false); // Hide the Game Panel
            mainCamera.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Menu Panel or Game Panel not assigned in the inspector.");
        }
    }

    private void ActivatePlayerCarCamera()
    {
        // Find the local player's car
        GameObject playerCar = FindPlayerCar(PhotonNetwork.LocalPlayer);

        if (playerCar != null)
        {
            // Deactivate the main camera
            if (mainCamera != null)
            {
                mainCamera.gameObject.SetActive(false);
            }

            // Activate the car camera for the local player
            Camera carCamera = playerCar.GetComponentInChildren<Camera>();
            if (carCamera != null)
            {
                carCamera.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Player's car does not have a camera attached.");
            }
        }
        else
        {
            Debug.LogWarning("Player's car not found.");
        }
    }

    private GameObject FindPlayerCar(Player player)
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView view = obj.GetComponent<PhotonView>();
            if (view != null && view.Owner == player)
            {
                return obj;
            }
        }
        return null;
    }

    private IEnumerator StartRaceCountdown()
    {
        if (raceStarted) yield break; // Prevent multiple starts

        raceStarted = true;

        for (int i = 5; i > 0; i--)
        {
            pv.RPC("UpdateCountdownUI", RpcTarget.All, i); // Sync countdown
            yield return new WaitForSeconds(1f);
        }

        // Start the race after the countdown
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("StartRace", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void StartRace()
    {
        if (!raceStarted)
        {
            raceStarted = true;

            // Master Client starts the scene load
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("RaceScene");
                PhotonNetwork.AutomaticallySyncScene = true;
            }
        }
    }

    /// <summary>
    /// Generate a unique room code consisting of 6 characters.
    /// </summary>
    private string GenerateUniqueCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        System.Random random = new System.Random();
        string code;
        do
        {
            code = new string(
                new char[6].Select(_ => chars[random.Next(chars.Length)]).ToArray());
        } while (roomCodes.ContainsKey(code)); // Ensure code uniqueness

        return code;
    }
    private void SpawnPlayerAtStartPosition()
    {
        Transform spawnPoint = spawnPositionManager.AssignStartPosition();

        string selectedCarPrefabName = PlayerPrefs.GetString("SelectedCarPrefab", "DefaultCarPrefab");
        Debug.Log($"Attempting to instantiate car prefab: {selectedCarPrefabName}");

        if (spawnPoint != null)
        {
            GameObject playerCar = PhotonNetwork.Instantiate(selectedCarPrefabName, spawnPoint.position, spawnPoint.rotation);
            PhotonView carPhotonView = playerCar.GetComponent<PhotonView>();

            if (carPhotonView != null && carPhotonView.IsMine)
            {
                CarCustomization customization = playerCar.GetComponent<CarCustomization>();
                if (customization != null)
                {
                    customization.ApplyCarCustomization();
                }
                else
                {
                    Debug.LogWarning("Spawned car does not have a CarCustomization component!");
                }
            }
            else
            {
                Debug.LogError("No PhotonView or not owned by the player!");
            }
            ActivatePlayerCarCamera();
        }
        else
        {
            Debug.LogError("No spawn point assigned!");
        }
    }

    [PunRPC]
    private void UpdateCountdownUI(int countdown)
    {
        UpdateCountdown(countdown);
    }

    public void UpdateCountdown(int seconds)
    {
        countdownText.text = $"Race starts in: {seconds}";
    }

    private void SwitchToGamePanel()
    {
        if (menuPanel != null && gamePanel != null)
        {
            menuPanel.SetActive(false); // Hide the Menu Panel
            gamePanel.SetActive(true);  // Show the Game Panel
        }
        else
        {
            Debug.LogWarning("Menu Panel or Game Panel not assigned in the inspector.");
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        Debug.Log($"Player properties updated for: {targetPlayer.NickName}");
        foreach (var key in changedProps.Keys)
        {
            Debug.Log($"{key}: {changedProps[key]}");
        }
    }
}