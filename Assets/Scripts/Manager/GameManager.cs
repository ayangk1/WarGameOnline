using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Graduation_Design_Turn_Based_Game
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance;
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

        [SerializeField] int mapWidth;     //��ͼ��
        [SerializeField] int mapHeight;//��ͼ��
        [SerializeField] int obstacleNum;//�ϰ���

        public Text playernum;
        public Text player1name;
        public Text player2name;

        [SerializeField] int loadedPlayerNum = 0;        //�Ѽ��س�������Ҹ���

        ExitGames.Client.Photon.Hashtable playerCustomProperties;

        public float _GameTime = 0;//��Ϸʱ��
        public static float itemtime;
        public float _ItemTime = 5;//����ѭ������ʱ��
        public float ItemTime
        {
            get
            {
                if (_ItemTime <= -1)
                    return itemtime;
                else
                    return _ItemTime;
            }
            set 
            {
                _ItemTime = value;
                if (_ItemTime == DataManager.Instance.BOX_SPAWN_TIME)
                {
                 //   ItemManager.Instance.SpawnBox = true;
                }
                
            }
        }
        public GameObject[] bornPos;  //�����㷽��
        public GameObject[] playerObj;                //���Ԥ����
        //����
        bool canStart;
        bool isInit;
        [Tooltip("��Ϸ�Ƿ����")]
        public bool playing; //��Ϸ�Ƿ����
        public bool Playing
        {
            get{ return playing; }
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
            gameLoadingPanel.SetActive(true);

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
            Instance = GetComponent<GameManager>();
            statePanel.SetActive(false);
            gameOverPanel.SetActive(false);
            itemtime = DataManager.Instance.BOX_SPAWN_TIME;
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
                    if (PlayerManager.Instance.player != null && PlayerManager.Instance.enemy != null)
                    {
                         GameObject.FindGameObjectWithTag("MainCamera").transform.position = new Vector3(PlayerManager.Instance.player.transform.position.x,
                             GameObject.FindGameObjectWithTag("MainCamera").transform.position.y, PlayerManager.Instance.player.transform.position.z);

                        state = GameState.Playing;
                        gameLoadingPanel.SetActive(false);
                    }
                    break;
                case GameState.Playing:
                    //��Ϸ��ʼbool
                    Playing = true;
                    //��Ϸʱ��
                    _GameTime += Time.deltaTime;
                    //������Ʒ 
                    //ItemManager.Instance.SpawnItem(Item.Box);
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
                InstantiateMap();
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
        //������ͼ
        void InstantiateMap()
        {
            if (!PhotonNetwork.IsMasterClient)      //ֻ����MasterClient����
                return;

            Debug.Log("���ɵ�ͼ");

            BattleData bd = new BattleData();
            //�� �� �ϰ���
            bd.Generate(mapWidth, mapHeight, obstacleNum);
            Battle.Instance.LoadBattle(bd);
        }
        // �������
        void InstantiatePlayer()
        {
            //������λ�ø�ֵ
            bornPos[0].transform.position = GameObject.FindGameObjectsWithTag("born")[0].transform.position;
            bornPos[1].transform.position = GameObject.FindGameObjectsWithTag("born")[1].transform.position;
            //���ݶ������
            playerCustomProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            Debug.Log(playerCustomProperties["Team"]);
            if (playerCustomProperties["Team"].Equals("Team1"))
            {
                var localPlayer = PhotonNetwork.Instantiate(playerObj[0].name, Vector3.zero, Quaternion.identity).GetComponent<GamePlayer>();
                localPlayer.transform.position = new Vector3(bornPos[0].transform.position.x, 0.2f, bornPos[0].transform.position.z);
            }
            if (playerCustomProperties["Team"].Equals("Team2"))
            {
                Debug.Log("�ͻ�������������");
                var localPlayer = PhotonNetwork.Instantiate(playerObj[1].name, Vector3.zero, Quaternion.identity).GetComponent<GamePlayer>();
                localPlayer.transform.position = new Vector3(bornPos[1].transform.position.x, 0.2f, bornPos[1].transform.position.z);
            }
        }
        [PunRPC]
        void UpdateData()
        {
            playernum.text = PhotonNetwork.CountOfPlayersInRooms.ToString();
            if ((string)PhotonNetwork.LocalPlayer.CustomProperties["team"] == "team1")
                player1name.text = PhotonNetwork.LocalPlayer.NickName;
            if ((string)PhotonNetwork.LocalPlayer.CustomProperties["team"] == "team2")
                player2name.text = PhotonNetwork.LocalPlayer.NickName;
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
