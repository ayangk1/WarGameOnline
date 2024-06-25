using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomPrefab : MonoBehaviourPunCallbacks
{
    public Button room;
    private new void OnEnable()
    {
        room = GetComponent<Button>();
        room.onClick.RemoveAllListeners();                       
        room.onClick.AddListener(delegate () {          
            PhotonNetwork.JoinRoom(GetComponentInChildren<Text>().text);
        });
    }
    private void Update()
    {
        if (transform.localScale != Vector3.one)
            transform.localScale = Vector3.one;
        
    }
}
