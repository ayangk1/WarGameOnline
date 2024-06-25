using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Graduation_Design_Turn_Based_Game
{
    public class GolemManager : MonoBehaviourPunCallbacks
    {
        public static GolemManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        private bool golemRound;
        public bool GolemRound
        {
            get 
            {
                if (!PlayerManager.Instance.isBeGolem)
                {
                    golemRound = false;
                }
                return golemRound; 
            }
            set { golemRound = value; }
        }
        public bool isMove;
        public Golem golem;
        public Golem enemyGolem;
        private ExitGames.Client.Photon.Hashtable playerCustomProperties;

        private void Start()
        {
            playerCustomProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            isMove = false;
        }
        private void Update()
        {
            if (PhotonNetwork.IsMasterClient && PlayerManager.Instance.IsRound) return;
            if (!PhotonNetwork.IsMasterClient && !PlayerManager.Instance.IsRound) return;

            foreach (var item in GameObject.FindGameObjectsWithTag("golem"))
            {
                if (golem == null && item.GetComponent<Golem>().master == PlayerManager.Instance.player.gameObject)
                {
                    golem = item.GetComponent<Golem>();
                }
                if (enemyGolem == null && item.GetComponent<Golem>().master == PlayerManager.Instance.enemy.gameObject)
                {
                    enemyGolem = item.GetComponent<Golem>();
                }
            }

            if (isMove) MoveEnd();
            GolemToward();
            Touch();
        }
        //控制角色朝向
        private void GolemToward()
        {
            if (golem != null)
            {
                golem.transform.LookAt(PlayerManager.Instance.enemy.transform);
            }
        }
        // 生成玩家
        public void InstantiateGolem(Vector3 pos)
        {
            //根据队伍分配 
            playerCustomProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            if (playerCustomProperties["Team"].Equals("Team1"))
            {
                var localGolem = PhotonNetwork.Instantiate(ItemManager.Instance.itemObj[2].name, Vector3.zero, Quaternion.identity).GetComponent<Golem>();
                localGolem.transform.position = new Vector3(pos.x, 0.1f, pos.z);
                PlayerManager.Instance.player.GetComponent<GamePlayer>().golem = localGolem.gameObject;
                localGolem.master = PlayerManager.Instance.player;
            }
            if (playerCustomProperties["Team"].Equals("Team2"))
            {
                var localGolem = PhotonNetwork.Instantiate(ItemManager.Instance.itemObj[2].name, Vector3.zero, Quaternion.identity).GetComponent<Golem>();
                localGolem.transform.position = new Vector3(pos.x, 0.1f, pos.z);
                PlayerManager.Instance.player.GetComponent<GamePlayer>().golem = localGolem.gameObject;
                localGolem.master = PlayerManager.Instance.player;
            }
        }
        public void Touch()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if ((hit.transform.tag == "golem" || hit.transform.tag == "golemTile")
                    && Input.GetMouseButtonDown(0) && GolemRound)
                {
                    Battle.Instance.ClearRenderType();
                    Move();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    Battle.Instance.ClearRenderType();
                }
            }
        }
        #region 移动
        public void Move()
        {
            TileManager.Instance.Range(golem.transform.position, golem.step, GridRenderType.range,false);
            isMove = true;
        }
        public void MoveEnd()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "range" && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    golem.AnimationPlay("Walk");
                    Nav(hit.transform.position);
                }
            }
        }
        #endregion

        private void Nav(Vector3 pos)
        {
            Hashtable hash = new Hashtable();
            hash.Add("easetype", iTween.EaseType.linear);
            hash.Add("islocal", true);
            hash.Add("speed", 5);
            hash.Add("oncomplete", "ResetData");
            hash.Add("oncompletetarget", gameObject);
            hash.Add("lookahead", 1);
            hash.Add("position", pos);
            iTween.MoveTo(golem.gameObject, hash);
        }
        public void ResetData()
        {
            isMove = false;
            golem.AnimationPlay("Idle");
            PlayerManager.Instance.ResetRound();
            PlayerManager.Instance.ResetData();
            GolemRound = false;
        }
    }
}