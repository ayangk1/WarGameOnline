using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graduation_Design_Turn_Based_Game
{
    public class MapCreater : MonoBehaviour
    {
        private static MapCreater instance;
        public static MapCreater Instance
        {
            get
            {
                return instance;
            }
        }

        void Update()
        {

        }

        public BattleData MapCreate()
        {
            BattleData bd = new BattleData();
            bd.Generate(10,10,10);
            return bd;
        }
    }

}
