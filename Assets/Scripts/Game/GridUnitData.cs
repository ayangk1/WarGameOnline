using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

namespace Graduation_Design_Turn_Based_Game
{
    public enum GridType
    {
        none,
        normal,
        obstacle,
        born,
        item,
    }
    public class GridUnitData
    {
        public Vector2 gridPos;
        public Vector3 worldPos;

        public int row;
        public int col;

        public GridType gridType;
    }
}
