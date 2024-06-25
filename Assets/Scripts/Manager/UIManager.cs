using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

namespace Graduation_Design_Turn_Based_Game
{
    public class UIManager : MonoBehaviourPunCallbacks
    {
        public static UIManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }
        public GameObject skillPanel;//行动选择面板
        public GameObject weaponPanel;//武器选择面板
        public GameObject buffStateImg;//buff状态图标
        //player的UI数据
        public GameObject player;
        public GameObject playerLifebar;
        public GameObject playerStateImg;
        //enemy的UI数据
        public GameObject enemy;
        public GameObject enemyLifebar;
        public GameObject enemyStateImg;
        //玩家状态 如果是服务器玩家就在左下角，客户端在右上角
        public GameObject PlayerState;
        public GameObject EnemyState;
        //其他状态信息
        public Text GameTime;
        private Text prompt;//提示信息
        public GameObject PromptObj;
        public Text Prompt
        {
            get 
            { 
                return prompt; 
            }
            set 
            { 
                if (value.text == "")
                {
                    prompt.text = value.text;
                    prompt.gameObject.SetActive(false);
                }
                else
                {
                    prompt.text = value.text;
                    prompt.gameObject.SetActive(true);
                    prompt.gameObject.GetComponent<Animator>().Play("PromptUI");
                }
            }
        }
        
        public Text roundText;//回合信息

        bool isOpenSetupPenel;
        bool isChange;
        bool isInit;

        private void Start()
        {
            PromptObj.SetActive(true);
            prompt = GameObject.Find("Prompt").GetComponent<Text>();
            prompt.text = "";
            isOpenSetupPenel = false;
            playerLifebar.SetActive(false);
            enemyLifebar.SetActive(false);
            weaponPanel.SetActive(false);
            skillPanel.SetActive(false);
        }
        void Update()
        {
            if (!GameManager.Instance.Playing) return;

            if (GameManager.Instance.Playing)
                GameTime.text = (int)(GameManager.Instance._GameTime / 60) + ":" + (int)(GameManager.Instance._GameTime % 60);

            FindPlayer();

            if (!PhotonNetwork.InRoom) return;
            
            if (!isChange)
            {
                ChangeUIStatePos();
                isChange = true;
            }

                if (PhotonNetwork.IsMasterClient)
                {
                    if (!PlayerManager.Instance.IsRound)
                    {
                        roundText.text = "你的回合";
                    }
                    else
                    {
                        roundText.text = "对方的回合";
                    }
                }
                else
                {
                    if (PlayerManager.Instance.IsRound)
                    {
                        roundText.text = "你的回合";
                    }
                    else
                    {
                        roundText.text = "对方的回合";
                    }
                }
            
            FindComponent();
            if (!isInit)
                Init();
            
            photonView.RPC("UpdateTalentUI", RpcTarget.All);

            if (PhotonNetwork.InRoom)
                photonView.RPC("UpdateUI", RpcTarget.All);
            
            if (player != null)
                Panel(playerLifebar, player.transform.position);
            if (enemy != null)
                Panel(enemyLifebar, enemy.transform.position);

        }
        //查找玩家
        public void FindPlayer()
        {
            if (player == null && GameManager.Instance.Playing)
            {
                player = GameObject.FindGameObjectWithTag("player");
                return;
            }
            if (enemy == null && GameManager.Instance.Playing)
            {
                enemy = GameObject.FindGameObjectWithTag("enemy");
                return;
            }
            if (player == null || enemy == null)
            {
                return;
            }
        }
        //设置控制按钮
        public void ClickSetupButton()
        {
            isOpenSetupPenel = !isOpenSetupPenel;
        }
        //控制UI面板的激活
        public void Panel(GameObject obj, Vector3 pos )
        {
            Vector3 cilcked = Camera.main.WorldToScreenPoint(pos);
            obj.SetActive(true);
            obj.transform.position = new Vector3(cilcked.x, cilcked.y - 80, cilcked.z);
        }
        //检测鼠标是否穿过物体
        public bool IsPointerOverObject()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }
        //获得组件
        public void FindComponent()
        {
            PlayerState = GameObject.Find("PlayerState");
            EnemyState = GameObject.Find("EnemyState");
            playerStateImg = PlayerState.transform.GetChild(PlayerState.transform.childCount - 1).gameObject;
            enemyStateImg = EnemyState.transform.GetChild(EnemyState.transform.childCount - 1).gameObject;          
        }
        //初始化
        public void Init()
        {
            PlayerState.transform.Find("talent").gameObject.SetActive(false);
            EnemyState.transform.Find("talent").gameObject.SetActive(false);
            isInit = true;
        }
        //更换UI状态位置
        public void ChangeUIStatePos()
        {
            ExitGames.Client.Photon.Hashtable playerCustomProperties;
            //根据队伍分配
            playerCustomProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            if (playerCustomProperties["Team"].Equals("Team2"))
            {
                PlayerState = GameObject.Find("PlayerState");
                EnemyState = GameObject.Find("EnemyState");
                EnemyState.name = "PlayerState";
                PlayerState.name = "EnemyState";
            }
        }
        [PunRPC]
        public void UpdateTalentUI()
        {
            if (player == null || enemy == null) return;
        }
        //更新UI数据
        [PunRPC]
        public void UpdateUI()
        {
            if (player == null || enemy == null) return;

            playerLifebar.transform.GetChild(0).GetComponent<Image>().fillAmount = player.GetComponent<GamePlayer>().Health / 100;
            playerLifebar.transform.GetChild(1).GetComponent<Text>().text = player.GetComponent<GamePlayer>().Health.ToString();
            enemyLifebar.transform.GetChild(0).GetComponent<Image>().fillAmount = enemy.GetComponent<GamePlayer>().Health / 100;
            enemyLifebar.transform.GetChild(1).GetComponent<Text>().text = enemy.GetComponent<GamePlayer>().Health.ToString();
            //玩家一二的状态栏
            PlayerState.transform.GetChild(0).GetComponent<Text>().text = "血量:" + player.GetComponent<GamePlayer>().Health.ToString();
            PlayerState.transform.GetChild(1).GetComponent<Text>().text = "攻击力:" + player.GetComponent<GamePlayer>().AttackValue.ToString();
            PlayerState.transform.GetChild(2).GetComponent<Text>().text = "护盾值:" + player.GetComponent<GamePlayer>().ShieldValue.ToString();
            if (player.GetComponent<GamePlayer>().IsAddCrit)
                PlayerState.transform.GetChild(3).GetComponent<Text>().text = "暴击:" + "是";
            if (!player.GetComponent<GamePlayer>().IsAddCrit)
                PlayerState.transform.GetChild(3).GetComponent<Text>().text = "暴击:" + "否";
            PlayerState.transform.GetChild(4).GetComponent<Text>().text = "攻击次数:" + player.GetComponent<GamePlayer>().AttackFrequency.ToString();
            PlayerState.transform.GetChild(5).GetComponent<Text>().text = "攻击距离:" + player.GetComponent<GamePlayer>().AttackRange.ToString();


            EnemyState.transform.GetChild(0).GetComponent<Text>().text = "血量:" + enemy.GetComponent<GamePlayer>().Health.ToString();
            EnemyState.transform.GetChild(1).GetComponent<Text>().text = "攻击力:" + enemy.GetComponent<GamePlayer>().AttackValue.ToString();
            EnemyState.transform.GetChild(2).GetComponent<Text>().text = "护盾值:" + enemy.GetComponent<GamePlayer>().ShieldValue.ToString();
            if (enemy.GetComponent<GamePlayer>().IsAddCrit)
                EnemyState.transform.GetChild(3).GetComponent<Text>().text = "暴击:" + "是";
            if (!enemy.GetComponent<GamePlayer>().IsAddCrit)
                EnemyState.transform.GetChild(3).GetComponent<Text>().text = "暴击:" + "否";
            EnemyState.transform.GetChild(4).GetComponent<Text>().text = "攻击次数:" + enemy.GetComponent<GamePlayer>().AttackFrequency.ToString();
            EnemyState.transform.GetChild(5).GetComponent<Text>().text = "攻击距离:" + enemy.GetComponent<GamePlayer>().AttackRange.ToString();
        }
        //检查Buff图片
        public void CheckBuffImg()
        {
            photonView.RPC("UpdateBuffImgUI", RpcTarget.All);
        }
        [PunRPC]
        public void UpdateBuffImgUI()
        {
            if (playerStateImg == null || enemyStateImg == null) return;

            #region player状态图标
            if (player.GetComponent<GamePlayer>().IsAddCrit)
            {
                int Count = 0;
                //如果已经添加图标 就不重复添加了
                for (int i = 0; i < playerStateImg.transform.childCount; i++)
                {
                    if (playerStateImg.transform.GetChild(i).name == "AddCrit")
                        Count++;
                }
                //添加图标
                if (Count == 0)
                {
                    GameObject go = Instantiate(buffStateImg, Vector3.zero, Quaternion.identity);
                    go.name = "AddCrit";
                    go.transform.SetParent(playerStateImg.transform);
                }
            }
            else
            {
                //清除图标
                for (int i = 0; i < playerStateImg.transform.childCount; i++)
                {
                    if (playerStateImg.transform.GetChild(i).name == "AddCrit")
                    {
                        Destroy(playerStateImg.transform.GetChild(i).gameObject);
                    }
                }
            }
            if (player.GetComponent<GamePlayer>().AttackFrequency > 1)
            {
                int Count = 0;
                for (int i = 0; i < playerStateImg.transform.childCount; i++)
                {
                    if (playerStateImg.transform.GetChild(i).name == "AddAttackFrequency")
                        Count++;
                }
                if (Count == 0)
                {
                    GameObject go = Instantiate(buffStateImg, Vector3.zero, Quaternion.identity);
                    go.name = "AddAttackFrequency";
                    go.transform.SetParent(playerStateImg.transform);
                }
            }
            else
            {
                //清除图标
                for (int i = 0; i < playerStateImg.transform.childCount; i++)
                {
                    if (playerStateImg.transform.GetChild(i).name == "AddAttackFrequency")
                    {
                        Destroy(playerStateImg.transform.GetChild(i).gameObject);
                    }
                }
            }
            #endregion

            #region enemy状态图标
            if (enemy.GetComponent<GamePlayer>().IsAddCrit)
            {
                int Count = 0;
                //如果已经添加图标 就不重复添加了
                for (int i = 0; i < enemyStateImg.transform.childCount; i++)
                {
                    if (enemyStateImg.transform.GetChild(i).name == "AddCrit")
                        Count++;
                }
                //添加图标
                if (Count == 0)
                {
                    GameObject go = Instantiate(buffStateImg, Vector3.zero, Quaternion.identity);
                    go.name = "AddCrit";
                    go.transform.SetParent(enemyStateImg.transform);
                }
            }
            else
            {
                //清除图标
                for (int i = 0; i < enemyStateImg.transform.childCount; i++)
                {
                    if (enemyStateImg.transform.GetChild(i).name == "AddCrit")
                    {
                        Destroy(enemyStateImg.transform.GetChild(i).gameObject);
                    }
                }
            }

            if (enemy.GetComponent<GamePlayer>().AttackFrequency > 1)
            {
                int Count = 0;
                for (int i = 0; i < enemyStateImg.transform.childCount; i++)
                {
                    if (enemyStateImg.transform.GetChild(i).name == "AddAttackFrequency")
                        Count++;
                }
                if (Count == 0)
                {
                    GameObject go = Instantiate(buffStateImg, Vector3.zero, Quaternion.identity);
                    go.name = "AddAttackFrequency";
                    go.transform.SetParent(enemyStateImg.transform);
                }
            }
            else
            {
                //清除图标
                for (int i = 0; i < enemyStateImg.transform.childCount; i++)
                {
                    if (enemyStateImg.transform.GetChild(i).name == "AddAttackFrequency")
                    {
                        Destroy(enemyStateImg.transform.GetChild(i).gameObject);
                    }
                }
            }
            #endregion
        }
        //控制动画结束 修改text
        public void PromptClose()
        {
            Prompt.text = "";
        }
    }
}