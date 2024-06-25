using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Graduation_Design_Turn_Based_Game
{
    public enum PlayerAttribute
    {
        Health,//血量
        Attack,//攻击力
        Shield,//护盾
        AttackFrequency,//攻击次数
        Crit,//暴击
    }
    public class GamePlayer : MonoBehaviourPun
    {
        public int Team;

        [SerializeField] private float health;        //生命值
        [SerializeField] public  float maxHealth;//最大生命值
        [SerializeField] private float attackValue;//可改变攻击力
        [SerializeField] public  float initAttackValue;//基础攻击力
        [SerializeField] private int attackFrequency;//攻击次数
        [SerializeField] private float shieldValue;//护盾
        [SerializeField] private bool isAddCrit;//下次是否暴击
        [SerializeField] private bool isAddRound;//是否增加回合
        [SerializeField] private int step;//步数
        [SerializeField] private int attackRange;//步数
        [SerializeField] public TalentState Talent;//天赋
        [SerializeField] public bool isMove;//玩家是否在移动

        //Golem
        public GameObject golem;

        //布尔
        public bool isInit;//是否初始化
        public float Health
        {
            get
            {
                return health;
            }
            set
            {
                //加血
                if (value > health && TalentManager.Instance._Talent == TalentState.Health)
                {
                    AttackValue -= (value - health) / maxHealth / 0.05f * 2;
                }
                else if (value < health && TalentManager.Instance._Talent == TalentState.Health)
                {
                    AttackValue += (value - health) / maxHealth / 0.05f * 2;
                }
                health = value;
            }
        }
        public int AttackRange
        {
            get
            {
                return attackRange;
            }
            set
            {
                attackRange = value;
            }
        }
        public float AttackValue
        {
            get
            {
                return attackValue + initAttackValue;
            }
            set
            {
                attackValue = value;
            }
        }
        public int Step
        {
            get
            {
                return step;
            }
            set
            {
                step = value;
            }
        }
        public int AttackFrequency
        {
            get
            {
                return attackFrequency;
            }
            set
            {
                if (value < attackFrequency)
                {
                    attackFrequency = value;
                    photonView.RPC("UpdateAttackFrequency", RpcTarget.All, attackFrequency);
                        UIManager.Instance.CheckBuffImg();
                }
                if (value > attackFrequency)
                {
                    attackFrequency = value;
                    photonView.RPC("UpdateAttackFrequency", RpcTarget.All, attackFrequency);
                    photonView.RPC("UpdatePromptUIPun", RpcTarget.All, "对方选择了增加攻击次数");
                    UIManager.Instance.CheckBuffImg();
                }
               
            }
        }
        public float ShieldValue
        {
            get
            {
                return shieldValue;
            }
            set
            {
                shieldValue = value;
            }
        }
        public bool IsAddCrit 
        {
            get { return isAddCrit; } 
            set 
            {
                if (isAddCrit == value) return;

                if (value)
                {
                    photonView.RPC("UpdateCrit", RpcTarget.All, isAddCrit);
                    UIManager.Instance.CheckBuffImg();
                    photonView.RPC("UpdatePromptUIPun", RpcTarget.All, "对方选择了暴击加成");
                }
                else
                {
                    photonView.RPC("UpdateCrit", RpcTarget.All, isAddCrit);
                    UIManager.Instance.CheckBuffImg();
                }
                isAddCrit = value;
            }
        }
        public bool IsAddRound
        {
            get { return isAddRound; }
            set { isAddRound = value; }
        }

        private void OnEnable()
        {
            ////初始化玩家数值
            maxHealth = 100;
            Health = maxHealth;
            initAttackValue = 5;
            attackValue = 0;
            attackFrequency = 1;
            isAddCrit = false;
            attackRange = 1;
            if (photonView.IsMine)
                IsAddTalent(TalentManager.Instance._Talent);

            if (Talent == TalentState.Run)
                step = 2;
            else
                step = 1;


            //初始化tag
            if (photonView.IsMine)
            {
                transform.tag = "player";
                transform.name = "player";
            }
            else
            {
                transform.tag = "enemy";
                transform.name = "enemy";
            }
        }
        //受伤
        public void IsHurt(float mAttackValue)
        {
            //是否暴击
            if (PlayerManager.Instance.player.GetComponent<GamePlayer>().IsAddCrit)
            {
                mAttackValue *= 2;
                PlayerManager.Instance.player.GetComponent<GamePlayer>().IsAddCrit = false;
            }
            mAttackValue -= ShieldValue;

            ShieldValue = 0;
            photonView.RPC("UpdateShield", RpcTarget.Others, shieldValue);

            if (mAttackValue <= 0)
                mAttackValue = 0;
            Health -= mAttackValue;

            photonView.RPC("UpdateHP", RpcTarget.Others, Health);

            PlayerManager.Instance.player.GetComponent<GamePlayer>().AttackFrequency--;
            if (PlayerManager.Instance.player.GetComponent<GamePlayer>().AttackFrequency > 0)
            {
                PlayerManager.Instance.isReAttack = true;
            }
            else
            {
                PlayerManager.Instance.ResetData();
            }
        }
        public bool IsDead()
        {
            if (Health <= 0)
            {
                photonView.RPC("UpdateHP", RpcTarget.All, Health);
                if (TalentManager.Instance._Talent == TalentState.Life)
                {
                    Health = maxHealth;
                    ExitGames.Client.Photon.Hashtable playerCustomProperties = PhotonNetwork.LocalPlayer.CustomProperties;
                    Debug.Log(playerCustomProperties["Team"]);
                    if (playerCustomProperties["Team"].Equals("Team1"))
                    {
                        transform.position = new Vector3(GameManager.Instance.bornPos[0].transform.position.x, 
                            0.1f, GameManager.Instance.bornPos[0].transform.position.z);
                    }
                    if (playerCustomProperties["Team"].Equals("Team2"))
                    {
                        transform.position = new Vector3(GameManager.Instance.bornPos[1].transform.position.x,
                            0.1f, GameManager.Instance.bornPos[1].transform.position.z);
                    }
                    
                }
                else
                    return true;
            }
            return false;
        }
        public void IsAddBuff(string buff)
        {
            if (buff == BuffState.AddHealth.ToString())
            {
                if (Health + 10 <= 100)
                    Health += 10;
                photonView.RPC("UpdateHP", RpcTarget.All, Health);
            }
            if (buff == BuffState.AddAttack.ToString())
            {
                AttackValue += 10;
                photonView.RPC("UpdateAttack", RpcTarget.All, AttackValue);
            }
            if (buff == BuffState.AddAttackFrequency.ToString())
            {
                AttackFrequency += 1;
            }
            if (buff == BuffState.AddShieldValue.ToString())
            {
                if (TalentManager.Instance._Talent == TalentState.Shield)
                    ShieldValue += 20;
                else
                    ShieldValue += 10;
                photonView.RPC("UpdateShield", RpcTarget.All, ShieldValue);
            }
            if (buff == BuffState.AddCrit.ToString())
            {
                IsAddCrit = true;
            }
            if (buff == BuffState.AddRound.ToString())
            {
                IsAddRound = true;
                photonView.RPC("UpdateAddRound", RpcTarget.All, IsAddRound);
            }
        }
        //添加天赋 
        public void IsAddTalent(TalentState talent)
        {
            Talent = talent;
            photonView.RPC("UpdateTalent", RpcTarget.All, Talent);
        }
        #region 同步数据
        //同步更新数据
        public void SynUpdateData(bool newBool)
        {
            photonView.RPC("UpdateBool", RpcTarget.All, "isMove", newBool);
        }
        [PunRPC]
        public void UpdateHP(float newHP)
        {
            Health = newHP;
            if (!photonView.IsMine)
            {
                UIManager.Instance.Prompt.text = "对方选择了恢复生命";
                UIManager.Instance.Prompt = UIManager.Instance.Prompt;
            }
        }
        [PunRPC]
        public void UpdateBool(string boolName, bool newBool)
        {
            switch (boolName)
            {
                case "isMove":
                    isMove = newBool;
                    break;
            }
        }
        [PunRPC]
        public void UpdateAttack(float newAttack)
        {
            AttackValue = newAttack;
            if (!photonView.IsMine)
            {
                UIManager.Instance.Prompt.text = "对方选择了增加攻击";
                UIManager.Instance.Prompt = UIManager.Instance.Prompt;
            }
        }
        [PunRPC]
        public void UpdateAttackFrequency(int newAttackFrequency)
        {
            AttackFrequency = newAttackFrequency;
        }
        [PunRPC]
        public void UpdateShield(float newShield)
        {
            ShieldValue = newShield;
            if (!photonView.IsMine)
            {
                UIManager.Instance.Prompt.text = "对方选择了增加护盾";
                UIManager.Instance.Prompt = UIManager.Instance.Prompt;
            }
        }
        [PunRPC]
        public void UpdateCrit(bool newSCrit)
        {
            IsAddCrit = newSCrit;
        }
        [PunRPC]
        public void UpdateAddRound(bool AddRound)
        {
            IsAddRound = AddRound;
            if (!photonView.IsMine)
            {
                UIManager.Instance.Prompt.text = "对方选择了增加一个回合";
                UIManager.Instance.Prompt = UIManager.Instance.Prompt;
            }
        }
        [PunRPC]
        public void UpdateTalent(TalentState talent)
        {
            Talent = talent;
            if (!photonView.IsMine)
            {
                UIManager.Instance.Prompt.text = "对方选择了" + TalentManager.Instance.TalentName + "天赋";
                UIManager.Instance.Prompt = UIManager.Instance.Prompt;
            }
        }
        [PunRPC]
        public void UpdatePromptUIPun(string text)
        {
            if (!photonView.IsMine)
            {
                UIManager.Instance.Prompt.text = text;
                UIManager.Instance.Prompt = UIManager.Instance.Prompt;
            }
        }
        public void UpdatePromptUI(string text)
        {
            if (!photonView.IsMine)
            {
                UIManager.Instance.Prompt.text = text;
                UIManager.Instance.Prompt = UIManager.Instance.Prompt;
            }
        }
        #endregion
        //重置玩家数据
        public void ResetPlayerData(params PlayerAttribute[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                switch (data[i])
                {
                    case PlayerAttribute.Health:
                        Health = maxHealth;
                        break;
                    case PlayerAttribute.Attack:
                        AttackValue = initAttackValue;
                        break;
                    case PlayerAttribute.Shield:
                        ShieldValue = 0;
                        break;
                    case PlayerAttribute.AttackFrequency:
                        AttackFrequency = 1;
                        break;
                    case PlayerAttribute.Crit:
                        IsAddCrit = false;
                        break;
                    default:
                        break;
                }
            }  
        }
    }
}
