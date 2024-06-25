using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyControl : MonoBehaviourPunCallbacks
{
    public GameObject LoginPanel;
    public GameObject RoomPanel;
    public GameObject LobbyPanel;
    public GameObject FadePanel;

    public GameObject[] roomPrefab;

    public Text playerName;
    public Text roomName;
    private new void OnEnable()
    {
        playerName.text = PhotonNetwork.NickName;
    }
    public void JoinOrCreateRoomButton()
    {
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };
        if (roomName.text.Length == 0)
        {
            LobbyPanel.SetActive(false);
            FadePanel.SetActive(true);
            PhotonNetwork.JoinOrCreateRoom("Ä¬ÈÏ·¿¼ä", roomOptions, default);
        }
        else
        {
            LobbyPanel.SetActive(false);
            FadePanel.SetActive(false);
            PhotonNetwork.JoinOrCreateRoom(roomName.text, roomOptions, default);
        }
    }
}
