using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graduation_Design_Turn_Based_Game
{
    public enum Item
    {
        Box,//盒子物品
        Golem,//石头人
    }
    public class ItemManager : MonoBehaviourPunCallbacks
    {
        public static ItemManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        public bool SpawnBox;
        public bool SpawnGolem;

        private bool isSyn;//是否同步物品数据
        public bool IsSyn
        {
            get { return isSyn; }
            set
            {
                isSyn = value;
                if (isSyn)
                {

                }
            }
        }
        private const byte ITEM_EVENT = 1;
        public Vector3 itemPos = Vector3.zero;  //物品点方块
        public GameObject[] itemObj;
        private void Start()
        {
            SpawnBox = true;
            SpawnGolem = true;
        }
        //创建item
        private void CreatePhotonItem(Item item, Vector3 pos)
        {
            switch (item)
            {
                case Item.Box:
                    //实例化物体
                    int index = Random.Range(0, 2);
                    var box = PhotonNetwork.Instantiate(itemObj[index].name, Vector3.zero, Quaternion.identity).GetComponent<Box>();
                    itemPos = GameObject.FindGameObjectWithTag("boxTile").transform.position;
                    box.transform.position = new Vector3(itemPos.x, 0.1f, itemPos.z);
                    box.transform.tag = "box";

                    //传输的数据
                    Vector3 boxWorldPos = box.worldPos;
                    //网格类型 世界坐标 网格坐标 行 列 地图宽 地图高
                    object[] boxDatas = new object[] { boxWorldPos };

                    PhotonNetwork.RaiseEvent(ITEM_EVENT, boxDatas, RaiseEventOptions.Default, SendOptions.SendUnreliable);
                    break;
                case Item.Golem:
                    //实例化物体
                    var golem = PhotonNetwork.Instantiate(itemObj[2].name, Vector3.zero, Quaternion.identity).GetComponent<Golem>();
                    golem.transform.position = new Vector3(pos.x, 0.1f, pos.z);
                    golem.health = 100;
                    golem.attack = 10;
                    golem.step = 1;
                    golem.attackRange = 1;
                    golem.master = PlayerManager.Instance.player.gameObject;
                    golem.transform.tag = "golem";
                    GameObject.FindGameObjectWithTag("player").GetComponent<GamePlayer>().golem = golem.gameObject;

                    //传输的数据
                    Vector3 golemWorldPos = new Vector3(pos.x, 0.1f, pos.z);
                    float golemHealth = golem.health;
                    float golemAttack = golem.attack;
                    float golemStep = golem.step;
                    int golemAttackRange = golem.attackRange;

                    //网格类型 世界坐标 网格坐标 行 列 地图宽 地图高
                    object[] golemDatas = new object[] { golemWorldPos, golemHealth, golemAttack, golemStep , golemAttackRange };

                    PhotonNetwork.RaiseEvent(ITEM_EVENT, golemDatas, RaiseEventOptions.Default, SendOptions.SendUnreliable);
                    break;
            }
        }
        //生成物品
        public void SpawnItem(Item item,params object[] data)
        {
            if (!PhotonNetwork.IsMasterClient)      //只能由MasterClient调用
                return;

            switch (item)
            {
                case Item.Box:
                    GameManager.Instance.ItemTime -= Time.deltaTime;
                    if (GameManager.Instance.ItemTime <= 0 && GameObject.FindGameObjectWithTag("box") == null)
                        TileManager.Instance.DisposeItemTile(Item.Box);

                    if (GameObject.FindGameObjectWithTag("boxTile") != null && SpawnBox)
                    {
                        CreatePhotonItem(item, itemPos);
                        Debug.Log("生成Box");
                        SpawnBox = false;
                    }
                    break;
                case Item.Golem:
                    if (GameObject.FindGameObjectWithTag("golemTile") != null && SpawnGolem)
                    {
                        Debug.Log("生成golem");
                        CreatePhotonItem(item, (Vector3)data[0]);
                        SpawnGolem = false;
                    }
                    break;
            }
        }
    }
}