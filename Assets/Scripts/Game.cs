using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Graduation_Design_Turn_Based_Game
{
    public class Game : MonoBehaviour
    {
        public static Game Instance;
        private void Awake()
        {
            Instance = this;
        }
        public Scene scene;
        public bool gameOver;        //判断是否在游戏中
        public bool isGame;
        private void OnEnable()
        {
            isGame = true;
        }
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        private void Update()
        {
            if (scene.buildIndex == 0 && gameOver)
            {
                ButtonManager.Instance.MatchRoomPanel.SetActive(true);
                gameOver = false;
            }
            if (isGame == false)
                Destroy(gameObject);
        }
    }
}
