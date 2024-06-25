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
        [SerializeField] public bool isRound;//false是master true是client
        public bool IsRound
        {
            get 
            { 
                return isRound; 
            }
            set 
            {
                isRound = value;
                if (!isRound && PhotonNetwork.IsMasterClient) //主机回合开始
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

                if (isRound && !PhotonNetwork.IsMasterClient) //客户端回合开始
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
        //玩家
        public GameObject player;
        public GameObject enemy;
        //判断交互
        public bool isMove;
        public bool isAttack;
        public bool isSkill;
        //寻路的起始点和终点
        public GridUnit start;
        public GridUnit end;
        //布尔
        public bool isUpdateCards;
        public bool isTouchScreen;
        public bool isHurtOver;
        public bool isReAttack;//是否重复攻击
        public bool isBowAttack;//是否开启弓箭攻击
        public bool isInit;//数据的初始化
        public bool isBeGolem;//是否存在己方Golem
        //玩家属性
        [SerializeField] private int roundNum = 1;
        public int RoundNum
        {
            get
            {
                return roundNum;
            }
            set
            {
                //如果回合数在减少
                if (value < roundNum && value > 0)
                {
                    BuffManager.Instance.isOpen = true;
                }
                //回合在减少 等于0时 并且存在golem
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
        //查找玩家
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
        //选择卡片
        public void SelectCards()
        {
                if (isUpdateCards)
                {
                    BuffManager.Instance.isOpen = true;
                    isUpdateCards = false;
                }
            if (!BuffManager.Instance.SelectOver) return;
        }
        //判断回合
        public void RoundJudge()
        {
            //判断是谁的回合
            if (PhotonNetwork.IsMasterClient && IsRound) return;//false是master true是client
            if (!PhotonNetwork.IsMasterClient && !IsRound) return;
            //回合开始选择卡片
            SelectCards();
            //游戏交互
            Touch();
        }
        //游戏交互
        public void Touch()
        {
            //判断是否穿过UI面板
            if (UIManager.Instance.IsPointerOverObject()) return;
            if (isSkill || isMove || isAttack || GolemManager.Instance.GolemRound) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //点击玩家
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

        #region 移动
        public void Move()
        {
            //生成可移动范围
            TileManager.Instance.Range(player.transform.position, player.GetComponent<GamePlayer>().Step,GridRenderType.range,true);

            start = Battle.Instance.GetGridUnit(new Vector3(GameObject.FindGameObjectWithTag("player").transform.position.x,
            0, GameObject.FindGameObjectWithTag("player").transform.position.z));
            isMove = true;

            //重置UI数据
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
                    //判断对方是否在移动
                    player.GetComponent<GamePlayer>().SynUpdateData(true);
                    TileManager.Instance.isFindPlayerTile = false;
                    end = Battle.Instance.GetGridUnit(hit.transform.position);
                    Nav(pos, end);

                    //播放动画
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
        #region 技能
        public void Skill()
        {
            //生成可放置范围
            TileManager.Instance.Range(player.transform.position, 3,GridRenderType.skill,true);

            isSkill = true;

            //重置UI数据
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
        #region 攻击
        public void Attack()
        {
            //生成可攻击范围
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
        //延迟生成
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
        //弓箭的攻击
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

        /*播放人物动画========================================================*/
        public void PlayerAnimation(string animName)
        {
            if (GameObject.FindGameObjectWithTag("player") != null)
            {
                GameObject.FindGameObjectWithTag("player").GetComponent<GamePlayerAnimator>().SynchronizeAnimationState(animName);
            }
        }
        /*导航===================================================*/
        private void Nav(GridUnit startpos, GridUnit endpos)
        {
            Battle.Instance.ClearRenderType();
            start.RenderType = GridRenderType.start;
            end.RenderType = GridRenderType.end;

            if (startpos != null && endpos != null)
            {
                //加入导航数据
                AStar aStar = new AStar();
                aStar.Navigation(start, end, Battle.Instance.mapData);

                //用于排除终点的上色
                Battle.Instance.PaintColor(aStar.Path(), GridRenderType.path, 1);

                InputManager.Instance.AddPath(aStar.Path());
                InputManager.Instance.MoveStart();
            }
        }
        [PunRPC]
        public void Round()
        {
            //回合更改时重置数据
            ResetUIData();
            IsRound = !IsRound;
        }
        /*重置===================================================*/
        //重置数据
        public void ResetData()
        {
            //清空导航的起始和结束
            start = null;
            end = null;
            //改变回合
            RoundNum--;
            if (RoundNum <= 0 && !isBeGolem)
                ResetRound();
            //更新buff
            if (!GolemManager.Instance.GolemRound)
                isUpdateCards = !isUpdateCards;
            RoundNum = 1;

            //重置互动bool
            ResetBoolData();
            isHurtOver = true;

            BuffManager.Instance.SelectOver = false;
            TileManager.Instance.isFindPlayerTile = true;
            
            //重置人物动画
            ResetAnimation();
            //清除渲染类型
            Battle.Instance.ClearRenderType();
            //photonView.RPC("ResetRender", RpcTarget.All);
            //重置UI数据
            ResetUIData();
        }
        //重置UI数据
        public void ResetUIData()
        {
            UIManager.Instance.skillPanel.SetActive(false);
            //检查Buff贴图
            UIManager.Instance.CheckBuffImg();
        }
        public void ResetRound()
        {
            photonView.RPC("Round", RpcTarget.All);
        }
        [PunRPC]
        public void ResetRender()
        {
            //清除渲染类型
            Battle.Instance.ClearRenderType();
        }
        //重置bool数据
        public void ResetBoolData()
        {
            isMove = false;
            isAttack = false;
            isSkill = false;
        }
        //重置UI数据
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
