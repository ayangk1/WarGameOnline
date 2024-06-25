using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

namespace Graduation_Design_Turn_Based_Game
{
    public class RoomPanelManager : MonoBehaviourPunCallbacks
    {
        public static RoomPanelManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        public float _MatchTime;
        public Text _MatchTimeText;
        public GameObject entertainment;
        public GameObject matching;
        public Button _BackButton;
        //����
        public bool isMatch = false;
        public bool isJoinRoom;
        ExitGames.Client.Photon.Hashtable customProperties;
        void Start()
        {
            _MatchTime = 0;
        }
        void Update()
        {
            if (ButtonManager.Instance.MatchRoomPanel.activeSelf)
                Matching();
            if (ButtonManager.Instance.EntertainPanel.activeSelf)
                Entertain();
        }
        //����ģʽ
        public void Entertain()
        {
            _MatchTimeText = entertainment.transform.GetChild(0).GetComponent<Text>();
            _BackButton = entertainment.transform.GetChild(1).GetComponent<Button>();

            if (!ButtonManager.Instance.EntertainPanel.activeSelf)
                _MatchTimeText.text = "����ģʽ";

            if (PhotonNetwork.PlayerList.Length == 2 && PhotonNetwork.InRoom && isMatch)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                if (ButtonManager.Instance.EntertainPanel.activeSelf)
                    photonView.RPC("MatchOver", RpcTarget.All);
                isMatch = false;
            }

            if (isMatch)
            {
                //ʱ��
                _MatchTime += Time.deltaTime;
                _MatchTimeText.text = (int)(_MatchTime / 60) + ":" + (int)(_MatchTime % 60);
                //ƥ�����
                if (RoomListManager.Instance.entertainSum == 0 && !isJoinRoom)
                {
                    string[] roomPropsInLobby = { "Mode" };
                    customProperties = new ExitGames.Client.Photon.Hashtable { { "Mode", "Entertainment" } };
                    RoomOptions roomOptions = new RoomOptions()
                    {
                        CustomRoomProperties = customProperties,
                        CustomRoomPropertiesForLobby = roomPropsInLobby
                    };
                    PhotonNetwork.CreateRoom(Random.Range(0, 999).ToString(), roomOptions,default);
                    _BackButton.onClick.RemoveAllListeners();
                    _BackButton.onClick.AddListener(delegate () {
                        Debug.Log("�˳��˷��䣬��������" + PhotonNetwork.CurrentRoom.Name + "����ģʽ");
                        _MatchTimeText.text = "����ģʽ";
                        PhotonNetwork.LeaveRoom();
                        isMatch = false;
                        isJoinRoom = false;
                    });
                    isJoinRoom = true;
                }
                else if (RoomListManager.Instance.entertainSum > 0 && !isJoinRoom)
                {
                    PhotonNetwork.JoinRoom(RoomListManager.Instance.roomName);
                    isJoinRoom = true;
                }
            }
            else if (!isMatch && !PhotonNetwork.InRoom)
            {
                _MatchTimeText.text = "����ģʽ";
                _MatchTime = 0;
                _BackButton.onClick.RemoveAllListeners();
                _BackButton.onClick.AddListener(delegate () {
                    ButtonManager.Instance.EntertainBackButton();
                });
            }
        }
        

        //ƥ��ģʽ
        public void Matching()
        {
            _MatchTimeText = matching.transform.GetChild(0).GetComponent<Text>();
            _BackButton = matching.transform.GetChild(1).GetComponent<Button>();

            if (!ButtonManager.Instance.MatchRoomPanel.activeSelf)
            {
                _MatchTimeText.text = "ƥ��ģʽ";
            }
            if (PhotonNetwork.PlayerList.Length == 2 && PhotonNetwork.InRoom && isMatch)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                if (ButtonManager.Instance.MatchRoomPanel.activeSelf)
                    photonView.RPC("MatchOver", RpcTarget.All);
                isMatch = false;
            }
            if (isMatch)
            {
                //ʱ��
                _MatchTime += Time.deltaTime;
                _MatchTimeText.text = (int)(_MatchTime / 60) + ":" + (int)(_MatchTime % 60);
                //ƥ�����
                if (RoomListManager.Instance.matchSum == 0 && !isJoinRoom)
                {
                    string[] roomPropsInLobby = { "Mode" };
                    customProperties = new ExitGames.Client.Photon.Hashtable { { "Mode", "Matching" } };
                    RoomOptions roomOptions = new RoomOptions()
                    {
                        CustomRoomProperties = customProperties,
                        CustomRoomPropertiesForLobby = roomPropsInLobby
                    };
                    PhotonNetwork.CreateRoom(Random.Range(0, 999).ToString(), roomOptions, default);

                    _BackButton.onClick.RemoveAllListeners();
                    _BackButton.onClick.AddListener(delegate () {
                        Debug.Log("�˳��˷��䣬��������" + PhotonNetwork.CurrentRoom.Name + "ƥ��ģʽ");
                        _MatchTimeText.text = "ƥ��ģʽ";
                        PhotonNetwork.LeaveRoom();
                        isMatch = false;
                        isJoinRoom = false;
                    });
                    isJoinRoom = true;
                }
                else if (RoomListManager.Instance.matchSum > 0 && !isJoinRoom)
                {
                    PhotonNetwork.JoinRoom(RoomListManager.Instance.roomName);
                    isJoinRoom = true;
                }
            }
            else if (!isMatch && !PhotonNetwork.InRoom)
            {
                _MatchTimeText.text = "ƥ��ģʽ";
                _MatchTime = 0;
                _BackButton.onClick.RemoveAllListeners();
                _BackButton.onClick.AddListener(delegate () {
                    ButtonManager.Instance.MatchingBackButton();
                });
            }
        }
        //��ʼƥ�䰴ť  
        public void StartMatchButton()
        {
            
            _BackButton.onClick.RemoveAllListeners();
            _BackButton.onClick.AddListener(delegate () {
                isMatch = false;
            });
            isMatch = true;
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PhotonNetwork.LeaveRoom();
            ButtonManager.Instance.GameSelectPanel.SetActive(false);
            ButtonManager.Instance.MatchRoomPanel.SetActive(false);
        }
        public override void OnLeftRoom()
        {
            
        }
        public override void OnJoinedRoom()
        {

            if ((string)PhotonNetwork.CurrentRoom.CustomProperties["Mode"] == "Entertainment")
            {
                Debug.Log("���뷿�䣺" + PhotonNetwork.CurrentRoom.Name + "����ģʽ");
            }
            if ((string)PhotonNetwork.CurrentRoom.CustomProperties["Mode"] == "Matching")
            {
                Debug.Log("���뷿�䣺" + PhotonNetwork.CurrentRoom.Name + "ƥ��ģʽ");
            }
        }
        public override void OnCreatedRoom()
        {
            if ((string)PhotonNetwork.CurrentRoom.CustomProperties["Mode"] == "Entertainment")
            {
                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
                hash.Add("Mode", "Entertainment");
                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
                Debug.Log("������һ�����䣬�ȴ���Ҽ���,��������" + PhotonNetwork.CurrentRoom.Name + "����ģʽ");
            }
            if ((string)PhotonNetwork.CurrentRoom.CustomProperties["Mode"] == "Matching")
            {
                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
                hash.Add("Mode", "Matching");
                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
                Debug.Log("������һ�����䣬�ȴ���Ҽ���,��������" + PhotonNetwork.CurrentRoom.Name + "ƥ��ģʽ");
            }
        }
        [PunRPC]
        public void MatchOver()
        {
            customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            if ((string)customProperties["Mode"] == "Entertainment")
            {
                PhotonNetwork.LoadLevel(2);
            }
            else if ((string)customProperties["Mode"] == "Matching")
            {
                ButtonManager.Instance.PreGamePanel.SetActive(true);
            }
            
        }

    }
}