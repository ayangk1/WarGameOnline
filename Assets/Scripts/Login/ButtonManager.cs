using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Graduation_Design_Turn_Based_Game
{
    public class ButtonManager : MonoBehaviourPunCallbacks
    {
        public static ButtonManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        public GameObject LobbyPanel;
        public GameObject LoginPanel;
        public GameObject GameSelectPanel;
        public GameObject SetPanel;
        public GameObject MatchRoomPanel;
        public GameObject CreateRoomPanel;
        public GameObject FriendPanel;
        public GameObject PreGamePanel;
        public GameObject GameOverPanel;
        public GameObject EntertainPanel;
        void Start()
        {
            MatchRoomPanel.SetActive(false);
            CreateRoomPanel.SetActive(false);
            SetPanel.SetActive(false);
            GameSelectPanel.SetActive(false);
            PreGamePanel.SetActive(false);
            GameOverPanel.SetActive(false);
            EntertainPanel.SetActive(false);
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                GameOverPanel.SetActive(true);
        }
        private void Update()
        {
            if (PhotonNetwork.InRoom)
                MatchRoomPanel.transform.Find("roomName").GetComponent<Text>().text = "房间号:" + PhotonNetwork.CurrentRoom.Name;
            if (!MatchRoomPanel.activeSelf)
            {
                RectTransform rect = FriendPanel.GetComponent<RectTransform>();
                rect.localPosition = new Vector3(590, 0, 0);
                Button button = FriendPanel.transform.Find("push").GetComponent<Button>();
                button.transform.localRotation = Quaternion.Euler(0, 0, 0);
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate () {
                    StartCoroutine(PanelMove());
                });
            }
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.PlayerListOthers.Length == 0)
            {
                PhotonNetwork.LeaveRoom();
                PreGamePanel.SetActive(false);
            }
        }
        //在游戏房间返回时 加入游戏大厅按键
        public void JoinLobbyButton()
        {
            LobbyPanel.SetActive(true);
            GameOverPanel.SetActive(false);
            Game.Instance.isGame = false;
            PhotonNetwork.LeaveRoom();
        }
        //在游戏房间返回时 再来一局按键
        public void GameAgainButton()
        {
            MatchRoomPanel.SetActive(true);
            GameOverPanel.SetActive(false);
            PhotonNetwork.LeaveRoom();
            Game.Instance.isGame = false;
        }
        //匹配模式
        public void MatchingButton()
        {
            MatchRoomPanel.SetActive(true);
            EntertainPanel.SetActive(false);
            GameOverPanel.SetActive(false);
        }
        //匹配返回按钮模式
        public void MatchingBackButton()
        {
            MatchRoomPanel.SetActive(false);
        }
        //娱乐模式
        public void EntertainButton()
        {
            EntertainPanel.SetActive(true);
            MatchRoomPanel.SetActive(false);
            GameOverPanel.SetActive(false);
        }
        //娱乐返回按钮模式
        public void EntertainBackButton()
        {
            EntertainPanel.SetActive(false);
        }
        //创建房间
        public void CreateRoomButton()
        {
            CreateRoomPanel.SetActive(true);
        }
        //创建房间返回按钮模式
        public void CreateRoomBackButton()
        {
            CreateRoomPanel.SetActive(false);
        }
        //游戏开始按钮
        public void GameStartButton()
        {
            GameSelectPanel.SetActive(true);
        }
        //选择面板返回
        public void GameSelectBackButton()
        {
            GameSelectPanel.SetActive(false);
        }
        //游戏开始按钮
        public void SetPanelButton()
        {
            SetPanel.SetActive(true);
        }
        //选择面板返回
        public void SetPanelBackButton()
        {
            SetPanel.SetActive(false);
        }
        //好友面板弹出
        public void FriendPanelOpenButton()
        {
            StartCoroutine(PanelMove());
        }
        IEnumerator PanelMove()
        {
            RectTransform rect = FriendPanel.GetComponent<RectTransform>();
            while (rect.localPosition.x > 450)
            {
                FriendPanel.transform.Translate(new Vector3(-1, 0, 0) * 10);
                yield return new WaitForSeconds(0.0001f);
            }
            rect.localPosition = new Vector3(450, 0, 0);
            Button button = FriendPanel.transform.Find("push").GetComponent<Button>();
            button.transform.localRotation = Quaternion.Euler(0, 0, -180);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate () {
                StartCoroutine(PanelBack());
            });
            yield return null;
        }
        IEnumerator PanelBack()
        {
            RectTransform rect = FriendPanel.GetComponent<RectTransform>();
            while (rect.localPosition.x < 590)
            {
                FriendPanel.transform.Translate(new Vector3(1, 0, 0) * 10);
                yield return new WaitForSeconds(0.0001f);
            }
            rect.localPosition = new Vector3(590, 0, 0);
            Button button = FriendPanel.transform.Find("push").GetComponent<Button>();
            button.transform.localRotation = Quaternion.Euler(0,0,0);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate () {
                StartCoroutine(PanelMove());
            });
            yield return null;
        }
    }
}
