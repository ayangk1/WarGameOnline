using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

namespace Graduation_Design_Turn_Based_Game
{
    public class RoomControl : MonoBehaviourPunCallbacks
    {
        public Button startButton;
        public Text selfText;
        public Text enemyText;
        ExitGames.Client.Photon.Hashtable costomProperties;
        public override void OnEnable()
        {
            if (Game.Instance != null&&Game.Instance.isGame) return;
            if (!PhotonNetwork.InRoom && !PhotonNetwork.IsConnected) return;
            Debug.Log("初始化属性成功");
            PhotonNetwork.AddCallbackTarget(this);
            //如果是服务器则是team1
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("IsMasterClient");
                costomProperties = new ExitGames.Client.Photon.Hashtable() {
                    { "Team","Team1" },		//玩家队伍
                    { "IsReady",false},		//是否准备好
                    {"Talent","None" }    //天赋
				};
                PhotonNetwork.LocalPlayer.CustomProperties = costomProperties; // 给自己赋值（本地）
                PhotonNetwork.SetPlayerCustomProperties(costomProperties);// 给自己赋值（云端）
            }
            else
            {
                costomProperties = new ExitGames.Client.Photon.Hashtable() {
                    { "Team","Team2" },		//玩家队伍
                    { "IsReady",false},		//是否准备好
                    {"Talent","None" }
                };
                PhotonNetwork.LocalPlayer.CustomProperties = costomProperties;
                PhotonNetwork.SetPlayerCustomProperties(costomProperties);
            }
        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (Game.Instance != null && Game.Instance.isGame) return;
            Debug.Log("准备状态:" + (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsReady"]);
            if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["IsReady"] == true)
                selfText.text = "选择完成";
            foreach (var item in PhotonNetwork.PlayerListOthers)
            {
                if (item.CustomProperties["IsReady"] != null)
                    if ((bool)item.CustomProperties["IsReady"] == true)
                        enemyText.text = "选择完成";
            }
        }
    }
}