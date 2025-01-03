using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NameInputManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField nameInputField;
    public Button confirmButton;
    public TextMeshProUGUI warningText;
    public GameObject profilePanel;
    public GameObject menuPanel;

    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        warningText.text = "";
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("PlayerName");
        }
    }

    private void OnConfirmButtonClicked()
    {
        string playerName = nameInputField.text.Trim();

        // Check if name is not empty, unique and less than or equal to 10 characters
        if (string.IsNullOrEmpty(playerName))
        {
            warningText.text = "Name cannot be empty!";
        }
        else if (playerName.Length > 10)
        {
            warningText.text = "Name cannot exceed 10 characters!";
        }
        else if (!IsNameUnique(playerName))
        {
            warningText.text = "Name is already taken!";
        }
        else
        {
            PhotonNetwork.NickName = playerName;
            PlayerPrefs.SetString("PlayerName", playerName);
            warningText.color = Color.green;
            warningText.text = "Name changed successfully!";
        }
    }
    public void OpenProfile()
    {
        profilePanel.SetActive(true);
        menuPanel.SetActive(false);
    }
    public void CloseProfile()
    {
        profilePanel.SetActive(false);
        menuPanel.SetActive(true);
    }
    private bool IsNameUnique(string playerName)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == playerName)
            {
                return false;
            }
        }
        return true;
    }
}