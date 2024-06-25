using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Chat;
using ExitGames.Client;


namespace Graduation_Design_Turn_Based_Game
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        public GameObject RoomPanel;
        public GameObject LobbyPanel;
        public GameObject LoginPanel;
        public GameObject FadePanel;
        public InputField playerName;
        public GameObject SetPanel;
        
        private void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                LoginPanel.SetActive(true);
                RoomPanel.SetActive(false);
                LobbyPanel.SetActive(false);
                FadePanel.SetActive(false);
            }
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log(PhotonNetwork.NetworkClientState);
            }
        }
        //���������ʱ
        public override void OnJoinedLobby()
        {
            Debug.Log("加入大厅成功");
            FadePanel.SetActive(false);
            LoginPanel.SetActive(false);
            LobbyPanel.SetActive(true);
        }
        //���뿪����ʱ
        public override void OnLeftRoom()
        {
            if (LobbyPanel != null)
                LobbyPanel.SetActive(true);
            if (RoomPanel != null)
                RoomPanel.SetActive(false);
            if (LoginPanel != null)
                LoginPanel.SetActive(false);
        }
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }
        
        //连接到服务器按钮
        public void ConnectedToServerButton()
        {           
            FadePanel.SetActive(true);
            
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            
            PhotonNetwork.NickName = playerName.text;
            if (PhotonNetwork.NickName == "")
                PhotonNetwork.NickName = "游客" + Random.Range(0, 999);
        }
        //�˳�������ť
        public void ExitLobbyButton()
        {
            PhotonNetwork.Disconnect();
            LoginPanel.SetActive(true);
            LobbyPanel.SetActive(false);
        }
        //�뿪���䰴ť
        public void LeaveRoomButton()
        {
            PhotonNetwork.LeaveRoom();
        }
        //�Ͽ�����
        public override void OnDisconnected(DisconnectCause cause)
        {
            if (LoginPanel != null)
            {
                LoginPanel.SetActive(true);
                LobbyPanel.SetActive(false);
                if (SetPanel != null)
                    SetPanel.SetActive(false);
            }
                
        }
        //�˳���Ϸ
        public void ExitGame()
        {
            PhotonNetwork.Disconnect();
            SetPanel.SetActive(false);
        }
    }
}