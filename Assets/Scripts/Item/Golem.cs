using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace Graduation_Design_Turn_Based_Game
{
    public class Golem : MonoBehaviourPunCallbacks
    {
       // private const byte ITEM_EVENT = 1;
        public Vector3 worldPos;
        public float health;
        public float attack;
        public float step;
        public GameObject master;
        public int attackRange;

        private new  void OnEnable()
        {
            ////初始化
            health = 100;
            attack = 10;
            attackRange = 1;
            step = 1;
            transform.tag = "golem";
            transform.name = "golem";
            if (master == null)
            {
                PlayerManager.Instance.enemy.GetComponent<GamePlayer>().golem = gameObject;
                master = PlayerManager.Instance.enemy;
            }
        }
        //受伤
        public void IsHurt(float mAttackValue)
        {
            health -= mAttackValue;
            photonView.RPC("UpdateHP", RpcTarget.Others, health);
            if (IsDead())
            {
                AnimationPlay("Die");
            }
        }
        [PunRPC]
        public void UpdateHP(float newHP)
        {
            health = newHP;
        }
        public bool IsDead()
        {
            if (health <= 0)
            {
                return true;
            }
            else
            {
                return true;
            }
        }
        
        public void Destroy()
        {
            Destroy(transform.gameObject);
        }
        public void AnimationPlay(string name)
        {
            Animator animator = GetComponent<Animator>();
            animator.Play(name);
        }
    }
}
