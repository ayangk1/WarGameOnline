using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graduation_Design_Turn_Based_Game
{
    public class GameCamera : MonoBehaviour
    {
        private Vector2 startPos;
        public float camSpeed = 2.0F;//����ƶ��ٶ�
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
                float dis = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);//��ָ֮��ľ���
                if (-1 == last) last = dis;
                float result = dis - last;//����һ֡�Ƚϱ仯

                if (result + current < min)//�������ƣ���С
                    result = min - current;
                else if (result + current > max)//�������ƣ����
                    result = max - current;
                result *= 0.1f;//ϵ��
                cam.transform.position += cam.transform.forward.normalized * result;
                current += result;//�ۼƵ�ǰ
                last = dis;//��¼Ϊ��һ֡��ֵ
                PlayerManager.Instance.isTouchScreen = true;
            }
            else
            {
                last = -1;//�������߼�ʱ
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
        //�л��������ӽ�
        public void SelfButton()
        {
            SelfView();
        }
        //�л����Է��ӽ�
        public void EnemyView()
        {
            cam.transform.position = new Vector3(PlayerManager.Instance.enemy.transform.position.x, 
                cam.transform.position.y, PlayerManager.Instance.enemy.transform.position.z);
        }
        //�л����Է��ӽ�
        public void SelfView()
        {
            cam.transform.position = new Vector3(PlayerManager.Instance.player.transform.position.x,
                cam.transform.position.y, PlayerManager.Instance.player.transform.position.z);
        }
        //�л�������golem
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
        //�л����Է�golem
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