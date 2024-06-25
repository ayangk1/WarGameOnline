using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Graduation_Design_Turn_Based_Game
{
    public class Battle2GameManager : MonoBehaviourPunCallbacks
    {
        public static Battle2GameManager Instance;
        public enum GameState
        {       //��Ϸ״̬ö��
            CheckPlayer,          //������
            PreStart,               //��Ϸ��ʼǰ
            Playing,                //��Ϸ������
            GameWin,                //��Ϸʤ��
            GameLose,               //��Ϸʧ��
            Tie,              //��Ϸƽ��
        };
        public GameState state = GameState.PreStart;    //��ʼ����Ϸ״̬
        public GameObject statePanel;				//״̬���
        public GameObject gameLoadingPanel;   //��Ϸ�������
        public GameObject gameOverPanel;   //��Ϸ�������
        [SerializeField] int loadedPlayerNum = 0;        //�Ѽ��س�������Ҹ���
        ExitGames.Client.Photon.Hashtable playerCustomProperties;
        public float _GameTime = 0;//��Ϸʱ��
        //����
        bool canStart;
        bool isInit;
        [Tooltip("��Ϸ�Ƿ����")]
        public bool playing; //��Ϸ�Ƿ����
        public bool Playing
        {
            get { return playing; }
            set
            {
                playing = value;
            }
        }
        public override void OnEnable()
        {
            if (!PhotonNetwork.InRoom)
                PhotonNetwork.LoadLevel(0);
            PhotonNetwork.AddCallbackTarget(this);

            //��Ϊ����master���ƿ�ʼ ֻ�ø��Լ�������� ���жϾͿ����� ���õ���RPC
            if (PhotonNetwork.IsMasterClient)
                loadedPlayerNum++;
            else
                photonView.RPC("ConfirmLoad", RpcTarget.All);
        }
        public override void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
        private void Start()
        {
            Instance = GetComponent<Battle2GameManager>();
            statePanel.SetActive(false);
            gameOverPanel.SetActive(false);
            ItemManager.Instance.SpawnBox = true;
        }
        void Update()
        {

            switch (state)
            {
                case GameState.CheckPlayer:
                    if (PhotonNetwork.IsMasterClient) //MasterClient��鵹��ʱ�ͳ�������������������Ϸ��ʼ
                        CheckPlayerConnected();
                    break;
                case GameState.PreStart:
                    //��Ϸ��ʼǰѡ���츳 ѡ�����������
                    if (!isInit)
                    {
                        InstantiatePlayer();            //������Ҷ���
                        isInit = true;
                    }
                    break;
                case GameState.Playing:
                    //��Ϸ��ʼbool
                    Playing = true;
                    //��Ϸʱ��
                    _GameTime += Time.deltaTime;
                    //ȷ����Ϸ����
                    ConfirmGameOver();
                    break;
                case GameState.GameWin:
                    break;
                case GameState.GameLose:
                    break;
                case GameState.Tie:
                    break;
                default:
                    break;
            }
        }
        //�����������Ƿ��Ѿ����س���
        void CheckPlayerConnected()
        {
            if (loadedPlayerNum == 2 && !canStart)
            {
                photonView.RPC("StartGame", RpcTarget.All);     //ʹ��RPC��������ҿ�ʼ��Ϸ
                canStart = true;
            }
        }
        //RPC��������ʼ��Ϸ
        [PunRPC]
        void StartGame()
        {
            Debug.Log("PUN��ʼ��Ϸ");
            state = GameState.PreStart;  //��Ϸ״̬�л�����Ϸ����״̬
        }
        void ConfirmGameOver()
        {
            if (PlayerManager.Instance.player == null || PlayerManager.Instance.enemy == null) return;

            if ((PlayerManager.Instance.player.GetComponent<GamePlayer>().IsDead() ||
                        PlayerManager.Instance.enemy.GetComponent<GamePlayer>().IsDead()) && PhotonNetwork.IsMasterClient
                        && PlayerManager.Instance.player != null && PlayerManager.Instance.enemy != null)
            {
                if (!PlayerManager.Instance.player.GetComponent<GamePlayer>().IsDead())
                    photonView.RPC("GameOver", RpcTarget.All, GameState.GameWin);
                if (PlayerManager.Instance.player.GetComponent<GamePlayer>().IsDead())
                    photonView.RPC("GameOver", RpcTarget.All, GameState.GameLose);
            }
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1 || PlayerManager.Instance.enemy == null)
                state = GameState.Tie;
        }
        //RPC���������ӳɹ����س�������Ҹ���
        [PunRPC]
        void ConfirmLoad()
        {
            loadedPlayerNum++;
        }

        [PunRPC]
        public void GameOver(GameState gameState)
        {
            if (gameState == GameState.GameWin)
            {
                gameOverPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "VICTORY";
                state = GameState.GameWin;
            }
            else if (gameState == GameState.GameLose)
            {
                gameOverPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "DEFEAT";
                state = GameState.GameLose;
            }
            else if (gameState == GameState.Tie)
            {
                gameOverPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "VICTORY";
                state = GameState.Tie;
            }
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            gameOverPanel.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate ()
            {
                PhotonNetwork.LoadLevel(0);
                Game.Instance.gameOver = true;
            });
            Playing = false;
        }
        // �������
        void InstantiatePlayer()
        {

        }
        //�ص����� ��������뿪����ʱ
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            photonView.RPC("GameOver", RpcTarget.All, GameState.GameWin);
        }
        //�뿪���亯��
        public void LeaveRoom()
        {
            photonView.RPC("GameOver", RpcTarget.Others, GameState.Tie);

            gameOverPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "DEFEAT";
            gameOverPanel.SetActive(true);
            //���ش���
            gameOverPanel.transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            gameOverPanel.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate ()
            {
                PhotonNetwork.LoadLevel(0);
                Game.Instance.gameOver = true;
            });
            state = GameState.GameLose;
            Playing = false;
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            PhotonNetwork.LoadLevel(0);
        }
    }
}
