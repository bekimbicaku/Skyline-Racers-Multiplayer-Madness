using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class PlayerNameDisplay : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerNameText;
    private PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            playerNameText.text = PhotonNetwork.NickName;
        }
        else
        {
            playerNameText.text = pv.Owner.NickName;
        }
    }
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (photonView.Owner == targetPlayer && changedProps.ContainsKey("PlayerName"))
        {
            playerNameText.text = (string)changedProps["PlayerName"];
        }
    }
}