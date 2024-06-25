using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graduation_Design_Turn_Based_Game
{
    public class GameCamera : MonoBehaviour
    {
        private Vector2 startPos;
        public float camSpeed = 2.0F;//相机移动速度
        protected readonly Transform m_zoom = null;
        public float max = 3.7f;
        public float min = 0;
        protected static float current = 0;
        private float last = -1;
        GameObject cam;
        private void Start()
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera");
        }
        void Update()
        {
            if (!GameManager.Instance.Playing) return;

            if (BuffManager.Instance.cards.activeSelf) return;

            if (Input.touchCount == 2)
            {
                float dis = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);//两指之间的距离
                if (-1 == last) last = dis;
                float result = dis - last;//与上一帧比较变化

                if (result + current < min)//区间限制：最小
                    result = min - current;
                else if (result + current > max)//区间限制：最大
                    result = max - current;
                result *= 0.1f;//系数
                cam.transform.position += cam.transform.forward.normalized * result;
                current += result;//累计当前
                last = dis;//记录为上一帧的值
                PlayerManager.Instance.isTouchScreen = true;
            }
            else
            {
                last = -1;//不触发逻辑时
                PlayerManager.Instance.isTouchScreen = false;
            }

            if (Input.touchCount == 1)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    startPos = Input.touches[0].position;
                }
                if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    Vector2 dic = (Input.touches[0].position - startPos).normalized;
                    cam.transform.position += new Vector3(dic.x, 0, dic.y).normalized * -camSpeed;
                    PlayerManager.Instance.isTouchScreen = true;
                }
                else
                {
                    PlayerManager.Instance.isTouchScreen = false;
                }
            }
        }
        //切换到自身视角
        public void SelfButton()
        {
            SelfView();
        }
        //切换到对方视角
        public void EnemyView()
        {
            cam.transform.position = new Vector3(PlayerManager.Instance.enemy.transform.position.x, 
                cam.transform.position.y, PlayerManager.Instance.enemy.transform.position.z);
        }
        //切换到对方视角
        public void SelfView()
        {
            cam.transform.position = new Vector3(PlayerManager.Instance.player.transform.position.x,
                cam.transform.position.y, PlayerManager.Instance.player.transform.position.z);
        }
        //切换到己方golem
        public void SelfGolemView()
        {
            foreach (GameObject item in GameObject.FindGameObjectsWithTag("golem"))
            {
                if (item.GetComponent<Golem>().master == PlayerManager.Instance.player.GetComponent<GamePlayer>())
                {
                    cam.transform.position = new Vector3(item.transform.position.x,cam.transform.position.y, item.transform.position.z);
                    break;
                }
            }
            
        }
        //切换到对方golem
        public void EnemyGolemView()
        {
            foreach (GameObject item in GameObject.FindGameObjectsWithTag("golem"))
            {
                if (item.GetComponent<Golem>().master == PlayerManager.Instance.enemy.GetComponent<GamePlayer>())
                {
                    cam.transform.position = new Vector3(item.transform.position.x, cam.transform.position.y, item.transform.position.z);
                    break;
                }
            }
        }
    }
}