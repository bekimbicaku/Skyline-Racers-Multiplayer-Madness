using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class RaceManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI countdownText;
    private int countdownTime = 3;

    public GameObject powerUpButtonUI1;
    public GameObject powerUpButtonUI2;

    private void Awake()
    {

    }
    private void Start()
    {
        
       StartCoroutine(StartCountdown());
        
    }

    private IEnumerator StartCountdown()
    {
        for (int i = countdownTime; i > 0; i--)
        {
            photonView.RPC("UpdateCountdownText", RpcTarget.All, i);
            yield return new WaitForSeconds(1f);
        }

        photonView.RPC("StartRace", RpcTarget.All);
    }

    [PunRPC]
    private void UpdateCountdownText(int seconds)
    {
        countdownText.text = seconds.ToString();
    }

    [PunRPC]
    private void StartRace()
    {
        CarController[] carControllers = FindObjectsOfType<CarController>();
        foreach (CarController carController in carControllers)
        {
            carController.StartRace();
        }
        countdownText.text = "GO!";
    }
}