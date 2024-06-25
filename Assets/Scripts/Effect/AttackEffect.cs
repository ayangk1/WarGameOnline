using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graduation_Design_Turn_Based_Game
{
    public class AttackEffect : MonoBehaviourPunCallbacks
    {
        public Transform enemy;
        public Transform pos;
        public float speed = 5f;
        public float mtime = 0.002f;
        RaycastHit hit;
        // Start is called before the first frame update
        void Start()
        {
        }
        private new void OnEnable()
        {
            pos = GameObject.FindGameObjectWithTag("player").transform.GetChild(0).GetChild(0).GetChild(3).GetChild(0).transform;
        }
        public void SetTarget(Transform pos)
        {
            enemy = pos;
        }
        // Update is called once per frame
        void Update()
        {
            if (mtime > 0)
                transform.position = pos.position;

            mtime -= Time.deltaTime;
          //  enemy = GameObject.FindGameObjectWithTag("enemy").transform.GetChild(2).transform;
            if (mtime > 0) return;

            transform.LookAt(enemy);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            CheckHint();
        }
        //ÅÐ¶ÏÊÇ·ñ»÷ÖÐ
        void CheckHint()
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                switch (enemy.transform.tag)
                {
                    case "enemy":
                        if (Vector3.Distance(transform.position, enemy.position) < 1)
                        {
                            if (hit.transform.tag == "enemy" && hit.transform.GetComponentInParent<GamePlayer>().IsDead())
                            {
                                PhotonNetwork.Destroy(gameObject);
                                PlayerManager.Instance.isHurtOver = false;
                                return;
                            }

                            hit.transform.GetComponentInParent<GamePlayer>().IsHurt(GameObject.FindGameObjectWithTag("player").GetComponent<GamePlayer>().AttackValue);
                            PhotonNetwork.Destroy(gameObject);
                            PlayerManager.Instance.isHurtOver = false;
                        }
                        break;
                    case "golem":
                        if (Vector3.Distance(transform.position, enemy.position) < 1)
                        {
                            if (hit.transform.tag == "golem" && hit.transform.GetComponentInParent<Golem>().IsDead())
                            {
                                PhotonNetwork.Destroy(gameObject);
                                PlayerManager.Instance.isHurtOver = false;
                                return;
                            }

                            hit.transform.GetComponentInParent<Golem>().IsHurt(GameObject.FindGameObjectWithTag("player").GetComponent<GamePlayer>().AttackValue);
                            PhotonNetwork.Destroy(gameObject);
                            PlayerManager.Instance.isHurtOver = false;
                        }
                        break;
                }
                
            }
        }
    }
}