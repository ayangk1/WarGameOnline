using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Graduation_Design_Turn_Based_Game
{
    public enum BuffState
    {
        AddHealth,
        AddAttack,
        AddAttackFrequency,
        AddShieldValue,
        AddRound,
        AddCrit,
    }
    public class BuffManager : MonoBehaviourPunCallbacks, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public static BuffManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        public GameObject cards;
        public BuffState BuffState;
        public bool isOpen;
        public int randomBuff;
        public string buff;
        private Sprite buffSprite;

        private GameObject player;
        private GameObject enemy;

        private bool selectOver;
        public bool SelectOver
        {
            get { return selectOver; }
            set 
            { 
                selectOver = value;
                if (selectOver)
                {
                    cards.SetActive(false);
                    UIManager.Instance.CheckBuffImg();
                }
            }
        }
        public int RandomBuff
        {
            get { return randomBuff; }
            set
            {
                randomBuff = value;
                switch (randomBuff)
                {
                    case 0:
                        buff = "恢复10生命值";
                        buffSprite = DataManager.Instance.GameConfi.AddHealth;
                        break;
                    case 1:
                        buff = "增加10攻击力";
                        buffSprite = DataManager.Instance.GameConfi.AddAttack;
                        break;
                    case 2:
                        buff = "增加一次攻击";
                        buffSprite = DataManager.Instance.GameConfi.AddAttackFrequency;
                        break;
                    case 3:
                        buff = "增加10护盾";
                        buffSprite = DataManager.Instance.GameConfi.AddShieldValue;
                        break;
                    case 4:
                        buff = "增加一个回合";
                        buffSprite = DataManager.Instance.GameConfi.AddRound;
                        break;
                    case 5:
                        buff = "增加暴击";
                        buffSprite = DataManager.Instance.GameConfi.AddCrit;
                        break;
                    default:
                        break;
                }
            }
        }
        private void Start()
        {
            if (cards.activeSelf)
                cards.SetActive(false);
        }
        void Update()
        {
            FindPlayer();
            if (isOpen)
            {
                cards.SetActive(true);
                for (int i = 0; i < cards.transform.childCount; i++)
                {
                    RandomBuff = Random.Range(0, 6);
                    cards.transform.GetChild(i).GetChild(2).GetComponent<Text>().text = buff;
                    cards.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = buffSprite;
                    cards.transform.GetChild(i).transform.GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();       //移除ReadyButton所有监听事件
                    switch (RandomBuff)
                    {
                        case 0:
                            cards.transform.GetChild(i).transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                AddHealth();
                            });
                            break;
                        case 1:
                            cards.transform.GetChild(i).transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                AddAttack();
                            });
                            break;
                        case 2:
                            cards.transform.GetChild(i).transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                AddAttackFrequency();
                            });
                            break;
                        case 3:
                            cards.transform.GetChild(i).transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                AddShieldValue();
                            });
                            break;
                        case 4:
                            cards.transform.GetChild(i).transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                AddRound();
                            });
                            break;
                        case 5:
                            cards.transform.GetChild(i).transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                AddCrit();
                            });
                            break;
                        default:
                            break;
                    }
                }
                isOpen = false;
            }
        }
        //添加生命
        public void AddHealth()
        {
            player.GetComponent<GamePlayer>().IsAddBuff("AddHealth");
            SelectOver = true;
        }
        //添加攻击
        public void AddAttack()
        {
            player.GetComponent<GamePlayer>().IsAddBuff("AddAttack");
            SelectOver = true;
        }
        public void AddAttackFrequency()
        {
            player.GetComponent<GamePlayer>().IsAddBuff("AddAttackFrequency");
            SelectOver = true;
        }
        public void AddShieldValue()
        {
            player.GetComponent<GamePlayer>().IsAddBuff("AddShieldValue");
            SelectOver = true;
        }
        public void AddRound()
        {
            player.GetComponent<GamePlayer>().IsAddBuff("AddRound");
            PlayerManager.Instance.RoundNum += 1;
            SelectOver = true;
        }
        //添加暴击
        public void AddCrit()
        {
            player.GetComponent<GamePlayer>().IsAddBuff("AddCrit");
            SelectOver = true;
        }
        //查找玩家
        public void FindPlayer()
        {
            if (player == null && GameManager.Instance.state == GameManager.GameState.Playing)
            {
                player = GameObject.FindGameObjectWithTag("player");
                return;
            }
            if (enemy == null && GameManager.Instance.state == GameManager.GameState.Playing)
            {
                enemy = GameObject.FindGameObjectWithTag("enemy");
                return;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}