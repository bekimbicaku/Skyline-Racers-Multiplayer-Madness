using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CarCustomizationUI : MonoBehaviour
{
    [Header("Car Selection")]
    public GameObject carParent; // Reference to CarParent in the scene
    public GameObject[] carModels;
    public Button nextCarButton;
    public Button previousCarButton;
    public TextMeshProUGUI carNameText;

    private int currentCarIndex = 0;

    [Header("Color Selection")]
    public Button[] colorButtons; // Buttons for selecting car colors
    private Material selectedMaterial; // Material selected via color picker

    [Header("Upgrades")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI handlingText;
    public TextMeshProUGUI accelerationText;
    public TextMeshProUGUI toughnessText;

    [Header("UI Elements")]
    public Button upgradeButton;
    public Button speedButton;
    public Button accelerationButton;
    public Button handlingButton;
    public Button toughnessButton;
    public Button selectButton;
    public Button unlockButton;

    public Image statIcon; // Image to display the icon
    public TextMeshProUGUI statNameText; // Text to display the name of the selected stat

    [Header("Icons")]
    public Sprite speedIcon;
    public Sprite accelerationIcon;
    public Sprite handlingIcon;
    public Sprite toughnessIcon;

    [Header("Upgrade Settings")]
    public float upgradeAmount = 1f; // Amount to upgrade per click

    private string selectedStat = "Speed"; // Default selected stat

    [Header("Car Unlocking")]
    public int[] unlockLevels; // Levels required to unlock each car
    public int[] carPrices; // Prices for unlocking cars
    public bool[] isCarUnlocked; // Unlock status for each car
    public TextMeshProUGUI unlockRequirementText;
    public int playerLevel = 1;

    public int upgradeCost = 100; // Cost of each upgrade
    public TextMeshProUGUI playerCoinsText;
    private int playerCoins = 1000;

    private CarCustomization carCustomization; // Reference to CarCustomization script on the current car

    private void Start()
    {
        // Initialize UI
        carModels = new GameObject[carParent.transform.childCount];
        for (int i = 0; i < carParent.transform.childCount; i++)
        {
            carModels[i] = carParent.transform.GetChild(i).gameObject;
        }

        LoadCarData();
        UpdateCarSelection();
        UpdateUpgradeUI();
        UpdatePlayerCoinsUI();

        // Attach button events
        nextCarButton.onClick.AddListener(SelectNextCar);
        previousCarButton.onClick.AddListener(SelectPreviousCar);
        selectButton.onClick.AddListener(SelectCar);
        unlockButton.onClick.AddListener(UnlockCar);
        upgradeButton.onClick.AddListener(UpgradeSelectedStat);

        speedButton.onClick.AddListener(() => SetSelectedStat("Speed"));
        accelerationButton.onClick.AddListener(() => SetSelectedStat("Acceleration"));
        handlingButton.onClick.AddListener(() => SetSelectedStat("Handling"));
        toughnessButton.onClick.AddListener(() => SetSelectedStat("Toughness"));

        foreach (var button in colorButtons)
        {
            button.onClick.AddListener(() => SelectCarColor(button));
        }

        SetSelectedStat("Speed");
    }

    #region Car Selection
    private void UpdateCarSelection()
    {
        for (int i = 0; i < carModels.Length; i++)
        {
            carModels[i].SetActive(i == currentCarIndex);
        }

        GameObject currentCar = carModels[currentCarIndex];
        carCustomization = currentCar.GetComponent<CarCustomization>();

        if (isCarUnlocked[currentCarIndex])
        {
            carNameText.text = currentCar.name;
            unlockButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
            unlockRequirementText.gameObject.SetActive(false);
        }
        else
        {
            carNameText.text = $"{currentCar.name} (Locked)";
            unlockButton.gameObject.SetActive(true);
            selectButton.gameObject.SetActive(false);
            unlockRequirementText.gameObject.SetActive(true);

            if (playerLevel >= unlockLevels[currentCarIndex])
            {
                unlockRequirementText.text = $"Unlock for {carPrices[currentCarIndex]} Coins";
                unlockButton.interactable = true;
            }
            else
            {
                unlockRequirementText.text = $"Reach Level {unlockLevels[currentCarIndex]} to Unlock";
                unlockButton.interactable = false;
            }
        }
    }

    public void SelectNextCar()
    {
        currentCarIndex = (currentCarIndex + 1) % carModels.Length;
        UpdateCarSelection();
    }

    public void SelectPreviousCar()
    {
        currentCarIndex = (currentCarIndex - 1 + carModels.Length) % carModels.Length;
        UpdateCarSelection();
    }
    #endregion

    #region Car Customization
    private void SelectCar()
    {
        PlayerPrefs.SetInt("SelectedCarIndex", currentCarIndex);
        PlayerPrefs.Save();

        string selectedCarPrefabName = carModels[currentCarIndex].name;
        PlayerPrefs.SetString("SelectedCarPrefab", selectedCarPrefabName);
        PlayerPrefs.Save();

        SaveCustomizationData();
        Debug.Log($"Car {selectedCarPrefabName} selected!");
    }

    private void SaveCustomizationData()
    {
        string selectedCarName = carModels[currentCarIndex].name;
        int materialIndex = Array.IndexOf(carCustomization.availableMaterials, carCustomization.carRenderer.material);

        Hashtable customizationData = new Hashtable
    {
        { "CarName", selectedCarName },
        { "CarColor", materialIndex }
    };

        PhotonNetwork.LocalPlayer.SetCustomProperties(customizationData);
        Debug.Log("Customization data saved.");
    }

    private void SelectCarColor(Button button)
    {
        if (carCustomization == null)
        {
            Debug.LogError("CarCustomization script is missing on the current car.");
            return;
        }

        int colorIndex = Array.IndexOf(colorButtons, button);
        if (colorIndex >= 0 && colorIndex < carCustomization.availableMaterials.Length)
        {
            carCustomization.ChangeCarColor(colorIndex);
            selectedMaterial = carCustomization.availableMaterials[colorIndex];

            // Save the selected color to PlayerPrefs
            PlayerPrefs.SetInt("CarColor", colorIndex);
            PlayerPrefs.Save();

            // Sync the selected car color across all players in the room
            SyncCarColorAcrossPlayers(colorIndex);
            Debug.Log($"Car color changed to material index: {colorIndex}");
        }
        else
        {
            Debug.LogError("Invalid color index selected.");
        }
    }

    private void SyncCarColorAcrossPlayers(int colorIndex)
    {
        Hashtable customizationData = new Hashtable
    {
        { "CarColor", colorIndex }
    };

        PhotonNetwork.LocalPlayer.SetCustomProperties(customizationData);
        Debug.Log("Car color synced with Photon Player Properties.");
    }

    #endregion

    #region Upgrades
    private void SetSelectedStat(string stat)
    {
        selectedStat = stat;
        switch (stat)
        {
            case "Speed":
                statIcon.sprite = speedIcon;
                statNameText.text = "Speed";
                break;
            case "Acceleration":
                statIcon.sprite = accelerationIcon;
                statNameText.text = "Acceleration";
                break;
            case "Handling":
                statIcon.sprite = handlingIcon;
                statNameText.text = "Handling";
                break;
            case "Toughness":
                statIcon.sprite = toughnessIcon;
                statNameText.text = "Toughness";
                break;
        }
    }

    private void UpgradeSelectedStat()
    {
        if (playerCoins < upgradeCost)
        {
            Debug.Log("Not enough coins to upgrade!");
            return;
        }

        playerCoins -= upgradeCost;

        switch (selectedStat)
        {
            case "Speed":
                carCustomization?.UpgradeSpeed(upgradeAmount);
                break;
            case "Acceleration":
                carCustomization?.UpgradeAcceleration(upgradeAmount);
                break;
            case "Handling":
                carCustomization?.UpgradeHandling(upgradeAmount);
                break;
            case "Toughness":
                carCustomization?.UpgradeToughness(upgradeAmount);
                break;
        }

        UpdateUpgradeUI();
        UpdatePlayerCoinsUI();
    }

    private void UpdateUpgradeUI()
    {
        if (carCustomization != null)
        {
            speedText.text = $"Speed: {carCustomization.CurrentSpeed}";
            handlingText.text = $"Handling: {carCustomization.CurrentHandling}";
            accelerationText.text = $"Acceleration: {carCustomization.CurrentAcceleration}";
            toughnessText.text = $"Toughness: {carCustomization.CurrentToughness}";
        }
    }

    private void UpdatePlayerCoinsUI()
    {
        playerCoinsText.text = $"Coins: {playerCoins}";
    }
    #endregion

    #region Car Unlocking
    private void UnlockCar()
    {
        if (playerLevel >= unlockLevels[currentCarIndex] && playerCoins >= carPrices[currentCarIndex])
        {
            playerCoins -= carPrices[currentCarIndex];
            isCarUnlocked[currentCarIndex] = true;

            SaveCarData();
            UpdateCarSelection();
            UpdatePlayerCoinsUI();

            Debug.Log($"Car {carModels[currentCarIndex].name} unlocked!");
        }
        else
        {
            Debug.Log("Cannot unlock car. Check level or coins.");
        }
    }

    private void LoadCarData()
    {
        for (int i = 0; i < carModels.Length; i++)
        {
            isCarUnlocked[i] = PlayerPrefs.GetInt($"CarUnlocked_{i}", i == 0 ? 1 : 0) == 1; // First car unlocked by default
        }
        currentCarIndex = PlayerPrefs.GetInt("SelectedCarIndex", 0);
    }

    private void SaveCarData()
    {
        for (int i = 0; i < carModels.Length; i++)
        {
            PlayerPrefs.SetInt($"CarUnlocked_{i}", isCarUnlocked[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }
    #endregion
}
