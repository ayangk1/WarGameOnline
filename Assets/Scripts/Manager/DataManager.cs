using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graduation_Design_Turn_Based_Game
{
    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance;
        private void Awake()
        {
            Instance = this;
            GameConfi = Resources.Load<GameConfi>("GameConfi");
        }
        public GameConfi GameConfi { get; private set; }
        [Tooltip("box���ɼ��ʱ��")]
        public float BOX_SPAWN_TIME = 10;//box���ɼ��ʱ��
    }
}
