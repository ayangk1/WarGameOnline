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
            Debug.Log("��ʼ�����Գɹ�");
            PhotonNetwork.AddCallbackTarget(this);
            //����Ƿ���������team1
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("IsMasterClient");
                costomProperties = new ExitGames.Client.Photon.Hashtable() {
                    { "Team","Team1" },		//��Ҷ���
                    { "IsReady",false},		//�Ƿ�׼����
                    {"Talent","None" }    //�츳
				};
                PhotonNetwork.LocalPlayer.CustomProperties = costomProperties; // ���Լ���ֵ�����أ�
                PhotonNetwork.SetPlayerCustomProperties(costomProperties);// ���Լ���ֵ���ƶˣ�
            }
            else
            {
                costomProperties = new ExitGames.Client.Photon.Hashtable() {
                    { "Team","Team2" },		//��Ҷ���
                    { "IsReady",false},		//�Ƿ�׼����
                    {"Talent","None" }
                };
                PhotonNetwork.LocalPlayer.CustomProperties = costomProperties;
                PhotonNetwork.SetPlayerCustomProperties(costomProperties);
            }
        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (Game.Instance != null && Game.Instance.isGame) return;
            Debug.Log("׼��״̬:" + (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsReady"]);
            if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["IsReady"] == true)
                selfText.text = "ѡ�����";
            foreach (var item in PhotonNetwork.PlayerListOthers)
            {
                if (item.CustomProperties["IsReady"] != null)
                    if ((bool)item.CustomProperties["IsReady"] == true)
                        enemyText.text = "ѡ�����";
            }
        }
    }
}