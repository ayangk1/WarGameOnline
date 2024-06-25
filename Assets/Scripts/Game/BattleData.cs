using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Graduation_Design_Turn_Based_Game
{
    public struct Point
    {
        public int row;
        public int col;
    }
    public class BattleData
    {
        public int mapWidth;
        public int mapHeight;

        //已创建障碍网格列表
        public List<GridUnitData> obstacleGridUnits;
        //已创建正常网格列表
        public List<GridUnitData> normalGridUnits;
        //已创建出生点网格列表
        public List<GridUnitData> bornGridUnits;
        //用来存放网格的坐标
        public int[,] obstacleMapPos;
        //用来存放网格的坐标
        public int[,] normalMapPos;
        //用来存放网格的坐标
        public int[,] bornMapPos;
        //用来存放网格的坐标
        public GridUnitData[,] mapPos;
        //节点数据
        public Dictionary<Point, AStarData> nodeDic = new Dictionary<Point, AStarData>();
        public int[] mapPosint;
        public void Generate(int row, int col, int obstacleNum)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            obstacleGridUnits = new List<GridUnitData>();
            normalGridUnits = new List<GridUnitData>();
            bornGridUnits = new List<GridUnitData>();

            mapWidth = col;
            mapHeight = row;
            mapPos = new GridUnitData[mapWidth, mapHeight];

            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    GridUnitData gud = new GridUnitData();
                    gud.row = i;
                    gud.col = j;
                    gud.gridPos = new Vector2Int(i, j);
                    gud.worldPos = new Vector3(j * 3, 0.1f,i * 3);
                    mapPos[i,j] = gud;
                    SetGridType(gud, GridType.normal);
                    DisposeBornGrid(gud, mapHeight, mapWidth);
                    //将导航数据存入字典方便通过位置查找
                    Point point = new Point
                    {
                        row = i,
                        col = j
                    };
                    AStarData node = new AStarData(i, j);
                    nodeDic.Add(point, node);
                }
            }
            DisposeObstacleGrid(obstacleNum);
        }
        //设置网格类型
        private void SetGridType(GridUnitData gridUnit, GridType gridType)
        {
            switch (gridType)
            {
                case GridType.normal:
                    normalGridUnits.Add(gridUnit);
                    break;
                case GridType.obstacle:
                    obstacleGridUnits.Add(gridUnit);
                    break;
                case GridType.born:
                    bornGridUnits.Add(gridUnit);
                    break;
            }
            gridUnit.gridType = gridType;
        }

        // 放置出生点格子
        private void DisposeBornGrid(GridUnitData gridUnit, int row, int col)
        {
            if (gridUnit.gridPos == new Vector2Int(0, 0)
                || gridUnit.gridPos == new Vector2Int(col- 1, row -1))
            {
                SetGridType(gridUnit, GridType.born);
                normalGridUnits.Remove(gridUnit);
            }
        }
        //放置障碍格子
        private void DisposeObstacleGrid(int obstacleNum)
        {
            for (int i = 0; i < obstacleNum; i++)
            {
                int tryTime = 999;
                int tryindex = 0;
                GridUnitData target = null;
                while (tryTime > 0 && target == null)
                {
                    tryindex = Random.Range(0, normalGridUnits.Count);
                    target = normalGridUnits[tryindex];
                    foreach (GridUnitData item in bornGridUnits)
                    {
                        //控制不在出生地旁边
                        if (Mathf.Abs(normalGridUnits[tryindex].gridPos.x - item.gridPos.x) < 2 &&
                           Mathf.Abs(normalGridUnits[tryindex].gridPos.y - item.gridPos.y) < 2)
                        {
                            target = null;
                            break;
                        }
                    }
                    tryTime--;
                }
                if (target != null)
                {
                    SetGridType(normalGridUnits[tryindex], GridType.obstacle);
                    normalGridUnits.RemoveAt(tryindex);
                }
            }
        }

        


    }
}

