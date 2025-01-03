using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject garagePanel;
    public CameraPositionManager cameraPositionManager;

    public void ShowMainPanel()
    {
        mainPanel.SetActive(true);
        garagePanel.SetActive(false);
        cameraPositionManager.MoveToMainPanel();
    }

    public void ShowGaragePanel()
    {
        mainPanel.SetActive(false);
        garagePanel.SetActive(true);
        cameraPositionManager.MoveToGaragePanel();
    }
}
