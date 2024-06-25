using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace Graduation_Design_Turn_Based_Game
{
    public class RoomListManager : MonoBehaviourPunCallbacks
    {
        public static RoomListManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        public GameObject roomPrefab;
        public Transform layout;
        public int entertainSum;
        public int matchSum;
        public string roomName;
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].PlayerCount == 0)
                    roomList.Remove(roomList[i]);
            }
            foreach (RoomInfo roomInfo in roomList)
            {
                Debug.Log("房间名：" + roomInfo.Name + "\n" + "模式：" + (string)roomInfo.CustomProperties["Mode"]);
                if (roomInfo.IsOpen && (string)roomInfo.CustomProperties["Mode"] == "Entertainment")
                {
                    entertainSum++;
                    roomName = roomInfo.Name;
                    break;
                }
                if (roomInfo.IsOpen && (string)roomInfo.CustomProperties["Mode"] == "Matching")
                {
                    matchSum++;
                    roomName = roomInfo.Name;
                    break;
                }
            }
            
            Debug.Log("房间列表更新啦" + "-共" + roomList.Count + "个房间");
            if (roomList.Count > 0)
            {
                foreach (RoomInfo roomInfo in roomList)
                {
                    if (roomInfo.IsOpen)
                        Debug.Log("房间" + roomInfo.Name + "开放");
                }
            }
        }
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            foreach (var item in propertiesThatChanged)
            {
                Debug.Log("自定义属性改变了：" + item.Key + "," + item.Value);
            }
        }
    }
}