using System.Collections;
using UnityEngine;
using Photon.Pun;

namespace Graduation_Design_Turn_Based_Game
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        public static PlayerManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        [SerializeField] public bool isRound;//false��master true��client
        public bool IsRound
        {
            get 
            { 
                return isRound; 
            }
            set 
            {
                isRound = value;
                if (!isRound && PhotonNetwork.IsMasterClient) //�����غϿ�ʼ
                {
                    TileManager.Instance.AlterTileTag();

                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameCamera>().SelfView();
                    if (player.GetComponent<GamePlayer>().AttackFrequency <= 0)
                        player.GetComponent<GamePlayer>().AttackFrequency = 1;
                }
                else if (isRound && PhotonNetwork.IsMasterClient)
                {
                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameCamera>().EnemyView();
                }

                if (isRound && !PhotonNetwork.IsMasterClient) //�ͻ��˻غϿ�ʼ
                {
                    TileManager.Instance.AlterTileTag();

                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameCamera>().SelfView();
                    if (player.GetComponent<GamePlayer>().AttackFrequency <= 0)
                        player.GetComponent<GamePlayer>().AttackFrequency = 1;
                    
                }
                else if (!isRound && !PhotonNetwork.IsMasterClient)
                {
                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameCamera>().EnemyView();
                }
            }
        }
        //���
        public GameObject player;
        public GameObject enemy;
        //�жϽ���
        public bool isMove;
        public bool isAttack;
        public bool isSkill;
        //Ѱ·����ʼ����յ�
        public GridUnit start;
        public GridUnit end;
        //����
        public bool isUpdateCards;
        public bool isTouchScreen;
        public bool isHurtOver;
        public bool isReAttack;//�Ƿ��ظ�����
        public bool isBowAttack;//�Ƿ�����������
        public bool isInit;//���ݵĳ�ʼ��
        public bool isBeGolem;//�Ƿ���ڼ���Golem
        //�������
        [SerializeField] private int roundNum = 1;
        public int RoundNum
        {
            get
            {
                return roundNum;
            }
            set
            {
                //����غ����ڼ���
                if (value < roundNum && value > 0)
                {
                    BuffManager.Instance.isOpen = true;
                }
                //�غ��ڼ��� ����0ʱ ���Ҵ���golem
                if (value < roundNum && value == 0 && isBeGolem)
                {
                    GolemManager.Instance.GolemRound = true;
                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameCamera>().SelfGolemView();
                } 
                roundNum = value;
            }
        }
        private void Start()
        {
            isMove = false;
            isAttack = false;
            isSkill = false;
    }
        private void Update()
        {
            FindPlayer();

            if (!GameManager.Instance.Playing) return;

            RoundJudge();

            if (isMove) MoveEnd(start);
            if (isSkill) SkillEnd();
            if (isAttack) AttackEnd();
            if (isBowAttack) BowAttack();
        }
        //�������
        public void FindPlayer()
        {
            if (player == null && GameManager.Instance.state == GameManager.GameState.PreStart)
            {
                player = GameObject.FindGameObjectWithTag("player");
                return;
            }
            if (enemy == null && GameManager.Instance.state == GameManager.GameState.PreStart)
            {
                enemy = GameObject.FindGameObjectWithTag("enemy");
                return;
            }
            if (enemy == null || player == null)
                return;
            if (!isInit)
            {
                isInit = true;
            }
        }
        //ѡ��Ƭ
        public void SelectCards()
        {
                if (isUpdateCards)
                {
                    BuffManager.Instance.isOpen = true;
                    isUpdateCards = false;
                }
            if (!BuffManager.Instance.SelectOver) return;
        }
        //�жϻغ�
        public void RoundJudge()
        {
            //�ж���˭�Ļغ�
            if (PhotonNetwork.IsMasterClient && IsRound) return;//false��master true��client
            if (!PhotonNetwork.IsMasterClient && !IsRound) return;
            //�غϿ�ʼѡ��Ƭ
            SelectCards();
            //��Ϸ����
            Touch();
        }
        //��Ϸ����
        public void Touch()
        {
            //�ж��Ƿ񴩹�UI���
            if (UIManager.Instance.IsPointerOverObject()) return;
            if (isSkill || isMove || isAttack || GolemManager.Instance.GolemRound) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //������
                if ((hit.transform.tag == "player" || hit.transform.tag == "playerTile")
                    && !GolemManager.Instance.GolemRound && Input.GetMouseButtonDown(0))
                {
                    Battle.Instance.ClearRenderType();
                    UIManager.Instance.Panel(UIManager.Instance.skillPanel, hit.transform.position);
                }
                else if (hit.transform.tag != "range" && hit.transform.tag != "canAttackEnemyTile" && hit.transform.tag != "skill" 
                    && Input.GetMouseButtonDown(0) && !GolemManager.Instance.GolemRound
                    && Input.touchCount == 1 && Input.touches[0].phase != TouchPhase.Moved)
                {
                    Battle.Instance.ClearRenderType();
                    ResetBoolData();
                    UIManager.Instance.skillPanel.SetActive(false);
                }
                if (hit.transform.tag == "box" && Battle.Instance.GetGridUnit(hit.transform.position).RenderType == GridRenderType.canTouch 
                    && Input.GetMouseButtonDown(0) && !GolemManager.Instance.GolemRound)
                {
                    WeaponManager.Instance.isOpen = true;
                }
                else if(Input.GetMouseButtonDown(1))
                {
                    WeaponManager.Instance.SelectOver = true;
                }
            }
        }

        public bool IsTouch()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    if (touch.tapCount == 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region �ƶ�
        public void Move()
        {
            //���ɿ��ƶ���Χ
            TileManager.Instance.Range(player.transform.position, player.GetComponent<GamePlayer>().Step,GridRenderType.range,true);

            start = Battle.Instance.GetGridUnit(new Vector3(GameObject.FindGameObjectWithTag("player").transform.position.x,
            0, GameObject.FindGameObjectWithTag("player").transform.position.z));
            isMove = true;

            //����UI����
            ResetUIData();
        }
        public void MoveEnd(GridUnit pos)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "range" && Input.GetKeyDown(KeyCode.Mouse0) && !isTouchScreen)
                {
                    //�ж϶Է��Ƿ����ƶ�
                    player.GetComponent<GamePlayer>().SynUpdateData(true);
                    TileManager.Instance.isFindPlayerTile = false;
                    end = Battle.Instance.GetGridUnit(hit.transform.position);
                    Nav(pos, end);

                    //���Ŷ���
                    switch (WeaponManager.Instance.Weapon)
                    {
                        case WeaponState.Boxing:
                            PlayerAnimation("BoxWalk");
                            break;
                        case WeaponState.Bow:
                            PlayerAnimation("BowWalk");
                            break;
                        case WeaponState.Sword:
                            PlayerAnimation("SwordWalk");
                            break;
                        case WeaponState.Wand:
                            PlayerAnimation("WandWalk");
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        #endregion
        #region ����
        public void Skill()
        {
            //���ɿɷ��÷�Χ
            TileManager.Instance.Range(player.transform.position, 3,GridRenderType.skill,true);

            isSkill = true;

            //����UI����
            ResetUIData();
        }
        public void SkillEnd()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "skill" && Input.GetKeyDown(KeyCode.Mouse0) && !isTouchScreen)
                {
                    isBeGolem = true;
                    GolemManager.Instance.InstantiateGolem(hit.transform.position);
                    ResetData();
                }
            }
        }
        #endregion
        #region ����
        public void Attack()
        {
            //���ɿɹ�����Χ
            TileManager.Instance.Range(player.transform.position, player.GetComponent<GamePlayer>().Step, GridRenderType.range,true);
            isAttack = true;
            ResetUIData();
        }
        public void AttackEnd()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if ((hit.transform.tag == "enemy" || hit.transform.tag == "canAttackEnemyTile") 
                    && Battle.Instance.GetGridUnit(enemy.transform.position).RenderType == GridRenderType.canAttackEnemyTile
                    && Input.GetKeyDown(KeyCode.Mouse0) 
                    && !isTouchScreen)
                {
                    switch (WeaponManager.Instance.Weapon)
                    {
                        case WeaponState.Boxing:
                            PlayerAnimation("BoxAttack");
                            enemy.GetComponentInParent<GamePlayer>().IsHurt(player.GetComponent<GamePlayer>().AttackValue);
                            break;
                        case WeaponState.Bow:
                            isBowAttack = true;
                            break;
                        case WeaponState.Sword:
                            PlayerAnimation("SwordAttack");
                            enemy.GetComponentInParent<GamePlayer>().IsHurt(player.GetComponent<GamePlayer>().AttackValue);
                            break;
                        case WeaponState.Wand:
                            PlayerAnimation("WandAttack");
                            PhotonNetwork.Instantiate("magic", GameObject.Find("playerWeapon").transform.GetChild(0).transform.position, Quaternion.identity);
                            break;
                        default:
                            break;
                    }
                }
            }
            if (isReAttack)
            {
                switch (WeaponManager.Instance.Weapon)
                {
                    case WeaponState.Boxing:
                        PlayerAnimation("BoxAttack");
                        enemy.GetComponentInParent<GamePlayer>().IsHurt(player.GetComponent<GamePlayer>().AttackValue);
                        break;
                    case WeaponState.Bow:
                        isBowAttack = true;
                        break;
                    case WeaponState.Sword:
                        PlayerAnimation("SwordAttack");
                        enemy.GetComponentInParent<GamePlayer>().IsHurt(GameObject.FindGameObjectWithTag("player").GetComponent<GamePlayer>().AttackValue);
                        break;
                    case WeaponState.Wand:
                        PlayerAnimation("WandAttack");
                        PhotonNetwork.Instantiate("magic", GameObject.Find("playerWeapon").transform.GetChild(0).transform.position, Quaternion.identity);                    
                        break;
                    default:
                        break;
                }
                isReAttack = false;
            }
        }
        //�ӳ�����
        public void DelaySpawn()
        {
            Transform pos = null;
            foreach (var item in WeaponManager.Instance.PlayerList)
            {
                if (item.name == "arrowPos")
                    pos = item.transform;
            }
            PhotonNetwork.Instantiate("arrow", pos.transform.position, Quaternion.identity);
        }
        //�����Ĺ���
        public void BowAttack()
        {
            float rotationY = Mathf.Lerp(0,40,0.5f);
            Debug.Log(rotationY);
            player.transform.localRotation = Quaternion.Euler(0, rotationY, 0);

            if (player.transform.localRotation.y == 40)
            { 
                PlayerAnimation("BowAttack");
                Invoke("DelaySpawn", 0.5f);
                isBowAttack = false;
            }
        }
        #endregion

        /*�������ﶯ��========================================================*/
        public void PlayerAnimation(string animName)
        {
            if (GameObject.FindGameObjectWithTag("player") != null)
            {
                GameObject.FindGameObjectWithTag("player").GetComponent<GamePlayerAnimator>().SynchronizeAnimationState(animName);
            }
        }
        /*����===================================================*/
        private void Nav(GridUnit startpos, GridUnit endpos)
        {
            Battle.Instance.ClearRenderType();
            start.RenderType = GridRenderType.start;
            end.RenderType = GridRenderType.end;

            if (startpos != null && endpos != null)
            {
                //���뵼������
                AStar aStar = new AStar();
                aStar.Navigation(start, end, Battle.Instance.mapData);

                //�����ų��յ����ɫ
                Battle.Instance.PaintColor(aStar.Path(), GridRenderType.path, 1);

                InputManager.Instance.AddPath(aStar.Path());
                InputManager.Instance.MoveStart();
            }
        }
        [PunRPC]
        public void Round()
        {
            //�غϸ���ʱ��������
            ResetUIData();
            IsRound = !IsRound;
        }
        /*����===================================================*/
        //��������
        public void ResetData()
        {
            //��յ�������ʼ�ͽ���
            start = null;
            end = null;
            //�ı�غ�
            RoundNum--;
            if (RoundNum <= 0 && !isBeGolem)
                ResetRound();
            //����buff
            if (!GolemManager.Instance.GolemRound)
                isUpdateCards = !isUpdateCards;
            RoundNum = 1;

            //���û���bool
            ResetBoolData();
            isHurtOver = true;

            BuffManager.Instance.SelectOver = false;
            TileManager.Instance.isFindPlayerTile = true;
            
            //�������ﶯ��
            ResetAnimation();
            //�����Ⱦ����
            Battle.Instance.ClearRenderType();
            //photonView.RPC("ResetRender", RpcTarget.All);
            //����UI����
            ResetUIData();
        }
        //����UI����
        public void ResetUIData()
        {
            UIManager.Instance.skillPanel.SetActive(false);
            //���Buff��ͼ
            UIManager.Instance.CheckBuffImg();
        }
        public void ResetRound()
        {
            photonView.RPC("Round", RpcTarget.All);
        }
        [PunRPC]
        public void ResetRender()
        {
            //�����Ⱦ����
            Battle.Instance.ClearRenderType();
        }
        //����bool����
        public void ResetBoolData()
        {
            isMove = false;
            isAttack = false;
            isSkill = false;
        }
        //����UI����
        public void ResetAnimation()
        {
            switch (WeaponManager.Instance.Weapon)
            {
                case WeaponState.Boxing:
                    PlayerAnimation("BoxIdle");
                    break;
                case WeaponState.Bow:
                    PlayerAnimation("BowIdle");
                    break;
                case WeaponState.Sword:
                    PlayerAnimation("SwordIdle");
                    break;
                case WeaponState.Wand:
                    PlayerAnimation("WandIdle");
                    break;
                default:
                    break;
            }
        }
    }
}
