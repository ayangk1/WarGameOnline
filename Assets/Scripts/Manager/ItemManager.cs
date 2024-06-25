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
        Box,//������Ʒ
        Golem,//ʯͷ��
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

        private bool isSyn;//�Ƿ�ͬ����Ʒ����
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
        public Vector3 itemPos = Vector3.zero;  //��Ʒ�㷽��
        public GameObject[] itemObj;
        private void Start()
        {
            SpawnBox = true;
            SpawnGolem = true;
        }
        //����item
        private void CreatePhotonItem(Item item, Vector3 pos)
        {
            switch (item)
            {
                case Item.Box:
                    //ʵ��������
                    int index = Random.Range(0, 2);
                    var box = PhotonNetwork.Instantiate(itemObj[index].name, Vector3.zero, Quaternion.identity).GetComponent<Box>();
                    itemPos = GameObject.FindGameObjectWithTag("boxTile").transform.position;
                    box.transform.position = new Vector3(itemPos.x, 0.1f, itemPos.z);
                    box.transform.tag = "box";

                    //���������
                    Vector3 boxWorldPos = box.worldPos;
                    //�������� �������� �������� �� �� ��ͼ�� ��ͼ��
                    object[] boxDatas = new object[] { boxWorldPos };

                    PhotonNetwork.RaiseEvent(ITEM_EVENT, boxDatas, RaiseEventOptions.Default, SendOptions.SendUnreliable);
                    break;
                case Item.Golem:
                    //ʵ��������
                    var golem = PhotonNetwork.Instantiate(itemObj[2].name, Vector3.zero, Quaternion.identity).GetComponent<Golem>();
                    golem.transform.position = new Vector3(pos.x, 0.1f, pos.z);
                    golem.health = 100;
                    golem.attack = 10;
                    golem.step = 1;
                    golem.attackRange = 1;
                    golem.master = PlayerManager.Instance.player.gameObject;
                    golem.transform.tag = "golem";
                    GameObject.FindGameObjectWithTag("player").GetComponent<GamePlayer>().golem = golem.gameObject;

                    //���������
                    Vector3 golemWorldPos = new Vector3(pos.x, 0.1f, pos.z);
                    float golemHealth = golem.health;
                    float golemAttack = golem.attack;
                    float golemStep = golem.step;
                    int golemAttackRange = golem.attackRange;

                    //�������� �������� �������� �� �� ��ͼ�� ��ͼ��
                    object[] golemDatas = new object[] { golemWorldPos, golemHealth, golemAttack, golemStep , golemAttackRange };

                    PhotonNetwork.RaiseEvent(ITEM_EVENT, golemDatas, RaiseEventOptions.Default, SendOptions.SendUnreliable);
                    break;
            }
        }
        //������Ʒ
        public void SpawnItem(Item item,params object[] data)
        {
            if (!PhotonNetwork.IsMasterClient)      //ֻ����MasterClient����
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
                        Debug.Log("����Box");
                        SpawnBox = false;
                    }
                    break;
                case Item.Golem:
                    if (GameObject.FindGameObjectWithTag("golemTile") != null && SpawnGolem)
                    {
                        Debug.Log("����golem");
                        CreatePhotonItem(item, (Vector3)data[0]);
                        SpawnGolem = false;
                    }
                    break;
            }
        }
    }
}