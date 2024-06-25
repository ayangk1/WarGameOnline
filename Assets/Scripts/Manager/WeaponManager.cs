using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Graduation_Design_Turn_Based_Game
{
    public enum WeaponState
    {
        Boxing, //空
        Wand,  //法杖
        Bow,//弓
        Sword,//剑盾
    }
    public class WeaponManager : MonoBehaviourPunCallbacks
    {
        public static WeaponManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        public GameObject[] WeaponBase;
        public GameObject weaponUI;
        private WeaponState weapon;
        public WeaponState Weapon
        {
            get { return weapon; }
            set
            {
                switch (value)
                {
                    case WeaponState.Boxing:
                        if (PlayerManager.Instance.player == null) return;
                        for (int i = 0; i < WeaponBase.Length; i++)
                        {
                            WeaponBase[i].SetActive(false);
                        }
                        PlayerManager.Instance.player.GetComponent<GamePlayerAnimator>().SynchronizeAnimationState("BoxIdle");
                        weapon = value;
                        break;
                    case WeaponState.Wand:
                        for (int i = 0; i < WeaponBase.Length; i++)
                        {
                            if (i == 0)
                                WeaponBase[i].SetActive(true);
                            else
                                WeaponBase[i].SetActive(false);
                        }
                        PlayerManager.Instance.player.GetComponent<GamePlayerAnimator>().SynchronizeAnimationState("WandIdle");
                        weapon = value;
                        break; 
                    case WeaponState.Bow:
                        for (int i = 0; i < WeaponBase.Length; i++)
                        {
                            if (i == 1 || i == 2)
                                WeaponBase[i].SetActive(true);
                            else
                                WeaponBase[i].SetActive(false);
                        }
                        PlayerManager.Instance.player.GetComponent<GamePlayerAnimator>().SynchronizeAnimationState("BowIdle");
                        weapon = value;
                        break;
                    case WeaponState.Sword:
                        for (int i = 0; i < WeaponBase.Length; i++)
                        {
                            if (i == 3 || i == 4)
                                WeaponBase[i].SetActive(true);
                            else
                                WeaponBase[i].SetActive(false);
                        }
                        PlayerManager.Instance.player.GetComponent<GamePlayerAnimator>().SynchronizeAnimationState("SwordIdle");
                        weapon = value;
                        break;
                    default:
                        break;
                }
                
            }
        }

        public GameObject wand;
        public GameObject bow;
        public GameObject quiver;
        public GameObject sword;
        public GameObject shield;

        public List<GameObject> WeaponList;
        public List<GameObject> PlayerList;

        //布尔
        public bool isInit;
        public bool isOpen;
        private bool selectOver;
        public bool SelectOver
        {
            get { return selectOver; }
            set
            {
                selectOver = value;
                if (selectOver)
                {
                    weaponUI.SetActive(false);
                }
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            WeaponList = new List<GameObject>();
            WeaponBase = new GameObject[5];
            Weapon = WeaponState.Boxing;
        }
        // Update is called once per frame
        void Update()
        {
            if (!GameManager.Instance.Playing) return;

            FindWeapon();

            if (isOpen)
            {
                weaponUI.SetActive(true);
                for (int i = 0; i < weaponUI.transform.childCount - 1; i++)
                {
                    weaponUI.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();       //移除ReadyButton所有监听事件
                    switch (i)
                    {
                        // 0-Wand 1-Bow 2-Sword 3-box
                        case 0:
                            weaponUI.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                WeaponWand();
                            });
                            break;
                        case 1:
                            weaponUI.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                WeaponBow();
                            });
                            break;
                        case 2:
                            weaponUI.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                WeaponSword();
                            });
                            break;
                        case 4:
                            weaponUI.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                            {
                                WeaponBoxing();
                            });
                            break;
                        default:
                            break;
                    }
                }
                isOpen = false;
            }
        }
        public void WeaponBoxing()
        {
            Weapon = WeaponState.Boxing;
            PlayerManager.Instance.player.GetComponent<GamePlayer>().AttackRange = 1;
            PlayerManager.Instance.player.GetComponent<GamePlayer>().initAttackValue = 5;

            photonView.RPC("GetWeapon", RpcTarget.All);

            SelectOver = true;
        }
        public void WeaponWand()
        {
            Weapon = WeaponState.Wand;
            PlayerManager.Instance.player.GetComponent<GamePlayer>().AttackRange = 3;
            PlayerManager.Instance.player.GetComponent<GamePlayer>().initAttackValue = 10;


            photonView.RPC("GetWeapon", RpcTarget.All);

            SelectOver = true;
        }
        public void WeaponBow()
        {
            Weapon = WeaponState.Bow;
            PlayerManager.Instance.player.GetComponent<GamePlayer>().AttackRange = 5;
            PlayerManager.Instance.player.GetComponent<GamePlayer>().initAttackValue = 10;

            photonView.RPC("GetWeapon", RpcTarget.All);

            SelectOver = true;
        }
        public void WeaponSword()
        {
            Weapon = WeaponState.Sword;
            PlayerManager.Instance.player.GetComponent<GamePlayer>().AttackRange = 1;
            PlayerManager.Instance.player.GetComponent<GamePlayer>().initAttackValue = 20;


            photonView.RPC("GetWeapon", RpcTarget.All);

            SelectOver = true;
        }
        [PunRPC]//获得武器后销毁箱子
        public void GetWeapon()
        {
            GameManager.Instance.ItemTime = DataManager.Instance.BOX_SPAWN_TIME;
            GameObject.FindGameObjectWithTag("canTouch").GetComponent<GridUnit>().data.gridType = GridType.normal;
            GameObject.FindGameObjectWithTag("canTouch").GetComponent<GridUnit>().RenderType = GridRenderType.none;
            ItemManager.Instance.SpawnBox = true;

            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.Destroy(GameObject.FindGameObjectWithTag("box").gameObject);
        }
        public void FindWeapon()
        {
            if (WeaponList.Count == 0)
            {
                FindChildByName(PlayerManager.Instance.player.transform, "wand");
                FindChildByName(PlayerManager.Instance.player.transform, "bow");
                FindChildByName(PlayerManager.Instance.player.transform, "quiver");
                FindChildByName(PlayerManager.Instance.player.transform, "sword");
                FindChildByName(PlayerManager.Instance.player.transform, "shield");
            }
            if (!isInit)
            {
                for (int i = 0; i < WeaponList.Count; i++)
                {
                    switch (WeaponList[i].name)
                    {
                        case "wand":
                            WeaponBase[0] = WeaponList[i];
                            break;
                        case "bow":
                            WeaponBase[1] = WeaponList[i];
                            break;
                        case "quiver":
                            WeaponBase[2] = WeaponList[i];
                            break;
                        case "sword":
                            WeaponBase[3] = WeaponList[i];
                            break;
                        case "shield":
                            WeaponBase[4] = WeaponList[i];
                            break;
                    }
                }
                isInit = true;
            }
        }
        public void FindChildByName(Transform root,string mName)
        {
            PlayerList.Add(root.gameObject);
            if (root.name == mName)
            {
                WeaponList.Add(root.gameObject);
                return;
            }
            if (root.childCount == 0) return;

            for (int i = 0; i < root.childCount; i++)
            {
                FindChildByName(root.GetChild(i), mName);
            }
        }
    }
}
