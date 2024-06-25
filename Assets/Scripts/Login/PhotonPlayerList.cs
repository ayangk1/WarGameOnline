using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Graduation_Design_Turn_Based_Game
{
    public class PhotonPlayerList : MonoBehaviourPunCallbacks
    {
        public int playerNum;
        public int PlayerNum
        {
            get 
            { 
                return playerNum; 
            }
            set 
            { 
                if(value < playerNum)
                {

                }
                playerNum = value; 
            }
        }
        public List<string> PlayerList;
        private const byte PLAYER_Add_EVENT = 30;
        private const byte PLAYER_Drop_EVENT = 29;

        private void NetworkingClient_EventReceived(EventData obj)
        {
            if (obj.Code == PLAYER_Add_EVENT)
            {
                object[] datas = (object[])obj.CustomData;
                string otherPlayerName = (string)datas[0];
                PlayerList.Add(otherPlayerName);
            }
            if (obj.Code == PLAYER_Drop_EVENT)
            {
                object[] datas = (object[])obj.CustomData;
                string otherPlayerName = (string)datas[0];
                foreach(var item in PlayerList)
                {
                    if (item == otherPlayerName)
                    {
                        PlayerList.Remove(item);
                    }
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
            PlayerList = new List<string>();
            PlayerNum = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                foreach (var item in PlayerList)
                {
                    Debug.Log(item);
                }
            }
        }
        public void MasterPlayerList()
        {
            if (PlayerNum == 0)
            {

            }
        }
        [PunRPC]
        public void AddMasterPlayer()
        {
            PlayerNum += 1;
        }
        [PunRPC]
        public void DropMasterPlayer()
        {
            PlayerNum -= 1;
        }
        //当加入大厅时
        public override void OnJoinedLobby()
        {
            if (!PhotonNetwork.IsConnected) return;
            Debug.Log(666);
            
            string name = PhotonNetwork.NickName;
            object[] datas = new object[] { name };
            PhotonNetwork.RaiseEvent(PLAYER_Add_EVENT, datas, RaiseEventOptions.Default, SendOptions.SendUnreliable);
            photonView.RPC("AddMasterPlayer", RpcTarget.Others);
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            if (!PhotonNetwork.IsConnected) return;
            string name = PhotonNetwork.NickName;
            object[] datas = new object[] { name };
            PhotonNetwork.RaiseEvent(PLAYER_Drop_EVENT, datas, RaiseEventOptions.Default, SendOptions.SendUnreliable);
            photonView.RPC("DropMasterPlayer", RpcTarget.Others);
        }
    }
}
