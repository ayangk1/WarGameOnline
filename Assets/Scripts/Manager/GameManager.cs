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
        {       //游戏状态枚举
            CheckPlayer,          //检查玩家
            PreStart,               //游戏开始前
            Playing,                //游戏进行中
            GameWin,                //游戏胜利
            GameLose,               //游戏失败
            Tie,              //游戏平局
        };
        public GameState state = GameState.PreStart;    //初始化游戏状态
        public GameObject statePanel;				//状态面板
        public GameObject gameLoadingPanel;   //游戏加载面板
        public GameObject gameOverPanel;   //游戏结束面板

        [SerializeField] int mapWidth;     //地图宽
        [SerializeField] int mapHeight;//地图高
        [SerializeField] int obstacleNum;//障碍数

        public Text playernum;
        public Text player1name;
        public Text player2name;

        [SerializeField] int loadedPlayerNum = 0;        //已加载场景的玩家个数

        ExitGames.Client.Photon.Hashtable playerCustomProperties;

        public float _GameTime = 0;//游戏时间
        public static float itemtime;
        public float _ItemTime = 5;//道具循环生成时间
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
        public GameObject[] bornPos;  //出生点方块
        public GameObject[] playerObj;                //玩家预制体
        //布尔
        bool canStart;
        bool isInit;
        [Tooltip("游戏是否进行")]
        public bool playing; //游戏是否进行
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

            //因为是有master控制开始 只用给自己添加人数 能判断就可以啦 不用调用RPC
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
                    if (PhotonNetwork.IsMasterClient) //MasterClient检查倒计时和场景加载人数，控制游戏开始
                        CheckPlayerConnected();
                    break;
                case GameState.PreStart:
                    //游戏开始前选择天赋 选择完生成玩家
                    if (!isInit)
                    {
                        InstantiatePlayer();            //创建玩家对象
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
                    //游戏开始bool
                    Playing = true;
                    //游戏时间
                    _GameTime += Time.deltaTime;
                    //生成物品 
                    //ItemManager.Instance.SpawnItem(Item.Box);
                    //确认游戏结束
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
        //检查所有玩家是否已经加载场景
        void CheckPlayerConnected()
        {
            if (loadedPlayerNum == 2 && !canStart)
            {
                InstantiateMap();
                photonView.RPC("StartGame", RpcTarget.All);     //使用RPC，所有玩家开始游戏
                canStart = true;
            }
        }
        //RPC函数，开始游戏
        [PunRPC]
        void StartGame()
        {
            Debug.Log("PUN开始游戏");
            state = GameState.PreStart;  //游戏状态切换到游戏进行状态
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
        //RPC函数，增加成功加载场景的玩家个数
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
        //创建地图
        void InstantiateMap()
        {
            if (!PhotonNetwork.IsMasterClient)      //只能由MasterClient调用
                return;

            Debug.Log("生成地图");

            BattleData bd = new BattleData();
            //高 宽 障碍数
            bd.Generate(mapWidth, mapHeight, obstacleNum);
            Battle.Instance.LoadBattle(bd);
        }
        // 生成玩家
        void InstantiatePlayer()
        {
            //出生点位置赋值
            bornPos[0].transform.position = GameObject.FindGameObjectsWithTag("born")[0].transform.position;
            bornPos[1].transform.position = GameObject.FindGameObjectsWithTag("born")[1].transform.position;
            //根据队伍分配
            playerCustomProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            Debug.Log(playerCustomProperties["Team"]);
            if (playerCustomProperties["Team"].Equals("Team1"))
            {
                var localPlayer = PhotonNetwork.Instantiate(playerObj[0].name, Vector3.zero, Quaternion.identity).GetComponent<GamePlayer>();
                localPlayer.transform.position = new Vector3(bornPos[0].transform.position.x, 0.2f, bornPos[0].transform.position.z);
            }
            if (playerCustomProperties["Team"].Equals("Team2"))
            {
                Debug.Log("客户端人物已生成");
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
        //回调函数 当有玩家离开房间时
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            photonView.RPC("GameOver", RpcTarget.All, GameState.GameWin);
        }
        //离开房间函数
        public void LeaveRoom()
        {
            photonView.RPC("GameOver", RpcTarget.Others, GameState.Tie);

            gameOverPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "DEFEAT";
            gameOverPanel.SetActive(true);
            //返回大厅
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
