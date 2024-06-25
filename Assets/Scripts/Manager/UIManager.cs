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
        public GameObject skillPanel;//�ж�ѡ�����
        public GameObject weaponPanel;//����ѡ�����
        public GameObject buffStateImg;//buff״̬ͼ��
        //player��UI����
        public GameObject player;
        public GameObject playerLifebar;
        public GameObject playerStateImg;
        //enemy��UI����
        public GameObject enemy;
        public GameObject enemyLifebar;
        public GameObject enemyStateImg;
        //���״̬ ����Ƿ�������Ҿ������½ǣ��ͻ��������Ͻ�
        public GameObject PlayerState;
        public GameObject EnemyState;
        //����״̬��Ϣ
        public Text GameTime;
        private Text prompt;//��ʾ��Ϣ
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
        
        public Text roundText;//�غ���Ϣ

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
                        roundText.text = "��Ļغ�";
                    }
                    else
                    {
                        roundText.text = "�Է��Ļغ�";
                    }
                }
                else
                {
                    if (PlayerManager.Instance.IsRound)
                    {
                        roundText.text = "��Ļغ�";
                    }
                    else
                    {
                        roundText.text = "�Է��Ļغ�";
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
        //�������
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
        //���ÿ��ư�ť
        public void ClickSetupButton()
        {
            isOpenSetupPenel = !isOpenSetupPenel;
        }
        //����UI���ļ���
        public void Panel(GameObject obj, Vector3 pos )
        {
            Vector3 cilcked = Camera.main.WorldToScreenPoint(pos);
            obj.SetActive(true);
            obj.transform.position = new Vector3(cilcked.x, cilcked.y - 80, cilcked.z);
        }
        //�������Ƿ񴩹�����
        public bool IsPointerOverObject()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }
        //������
        public void FindComponent()
        {
            PlayerState = GameObject.Find("PlayerState");
            EnemyState = GameObject.Find("EnemyState");
            playerStateImg = PlayerState.transform.GetChild(PlayerState.transform.childCount - 1).gameObject;
            enemyStateImg = EnemyState.transform.GetChild(EnemyState.transform.childCount - 1).gameObject;          
        }
        //��ʼ��
        public void Init()
        {
            PlayerState.transform.Find("talent").gameObject.SetActive(false);
            EnemyState.transform.Find("talent").gameObject.SetActive(false);
            isInit = true;
        }
        //����UI״̬λ��
        public void ChangeUIStatePos()
        {
            ExitGames.Client.Photon.Hashtable playerCustomProperties;
            //���ݶ������
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
        //����UI����
        [PunRPC]
        public void UpdateUI()
        {
            if (player == null || enemy == null) return;

            playerLifebar.transform.GetChild(0).GetComponent<Image>().fillAmount = player.GetComponent<GamePlayer>().Health / 100;
            playerLifebar.transform.GetChild(1).GetComponent<Text>().text = player.GetComponent<GamePlayer>().Health.ToString();
            enemyLifebar.transform.GetChild(0).GetComponent<Image>().fillAmount = enemy.GetComponent<GamePlayer>().Health / 100;
            enemyLifebar.transform.GetChild(1).GetComponent<Text>().text = enemy.GetComponent<GamePlayer>().Health.ToString();
            //���һ����״̬��
            PlayerState.transform.GetChild(0).GetComponent<Text>().text = "Ѫ��:" + player.GetComponent<GamePlayer>().Health.ToString();
            PlayerState.transform.GetChild(1).GetComponent<Text>().text = "������:" + player.GetComponent<GamePlayer>().AttackValue.ToString();
            PlayerState.transform.GetChild(2).GetComponent<Text>().text = "����ֵ:" + player.GetComponent<GamePlayer>().ShieldValue.ToString();
            if (player.GetComponent<GamePlayer>().IsAddCrit)
                PlayerState.transform.GetChild(3).GetComponent<Text>().text = "����:" + "��";
            if (!player.GetComponent<GamePlayer>().IsAddCrit)
                PlayerState.transform.GetChild(3).GetComponent<Text>().text = "����:" + "��";
            PlayerState.transform.GetChild(4).GetComponent<Text>().text = "��������:" + player.GetComponent<GamePlayer>().AttackFrequency.ToString();
            PlayerState.transform.GetChild(5).GetComponent<Text>().text = "��������:" + player.GetComponent<GamePlayer>().AttackRange.ToString();


            EnemyState.transform.GetChild(0).GetComponent<Text>().text = "Ѫ��:" + enemy.GetComponent<GamePlayer>().Health.ToString();
            EnemyState.transform.GetChild(1).GetComponent<Text>().text = "������:" + enemy.GetComponent<GamePlayer>().AttackValue.ToString();
            EnemyState.transform.GetChild(2).GetComponent<Text>().text = "����ֵ:" + enemy.GetComponent<GamePlayer>().ShieldValue.ToString();
            if (enemy.GetComponent<GamePlayer>().IsAddCrit)
                EnemyState.transform.GetChild(3).GetComponent<Text>().text = "����:" + "��";
            if (!enemy.GetComponent<GamePlayer>().IsAddCrit)
                EnemyState.transform.GetChild(3).GetComponent<Text>().text = "����:" + "��";
            EnemyState.transform.GetChild(4).GetComponent<Text>().text = "��������:" + enemy.GetComponent<GamePlayer>().AttackFrequency.ToString();
            EnemyState.transform.GetChild(5).GetComponent<Text>().text = "��������:" + enemy.GetComponent<GamePlayer>().AttackRange.ToString();
        }
        //���BuffͼƬ
        public void CheckBuffImg()
        {
            photonView.RPC("UpdateBuffImgUI", RpcTarget.All);
        }
        [PunRPC]
        public void UpdateBuffImgUI()
        {
            if (playerStateImg == null || enemyStateImg == null) return;

            #region player״̬ͼ��
            if (player.GetComponent<GamePlayer>().IsAddCrit)
            {
                int Count = 0;
                //����Ѿ����ͼ�� �Ͳ��ظ������
                for (int i = 0; i < playerStateImg.transform.childCount; i++)
                {
                    if (playerStateImg.transform.GetChild(i).name == "AddCrit")
                        Count++;
                }
                //���ͼ��
                if (Count == 0)
                {
                    GameObject go = Instantiate(buffStateImg, Vector3.zero, Quaternion.identity);
                    go.name = "AddCrit";
                    go.transform.SetParent(playerStateImg.transform);
                }
            }
            else
            {
                //���ͼ��
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
                //���ͼ��
                for (int i = 0; i < playerStateImg.transform.childCount; i++)
                {
                    if (playerStateImg.transform.GetChild(i).name == "AddAttackFrequency")
                    {
                        Destroy(playerStateImg.transform.GetChild(i).gameObject);
                    }
                }
            }
            #endregion

            #region enemy״̬ͼ��
            if (enemy.GetComponent<GamePlayer>().IsAddCrit)
            {
                int Count = 0;
                //����Ѿ����ͼ�� �Ͳ��ظ������
                for (int i = 0; i < enemyStateImg.transform.childCount; i++)
                {
                    if (enemyStateImg.transform.GetChild(i).name == "AddCrit")
                        Count++;
                }
                //���ͼ��
                if (Count == 0)
                {
                    GameObject go = Instantiate(buffStateImg, Vector3.zero, Quaternion.identity);
                    go.name = "AddCrit";
                    go.transform.SetParent(enemyStateImg.transform);
                }
            }
            else
            {
                //���ͼ��
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
                //���ͼ��
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
        //���ƶ������� �޸�text
        public void PromptClose()
        {
            Prompt.text = "";
        }
    }
}