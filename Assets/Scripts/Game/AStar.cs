using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graduation_Design_Turn_Based_Game
{
    public class AStarData
    {
        public int G;
        public int H;
        public int F
        {
            get { return G + H; }
        }

        public AStarData preNode;

        public int row;
        public int col;

        public AStarData(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
    public class AStar
    {
        //导航
        List<AStarData> pathNode = new List<AStarData>();
        public void Navigation(GridUnit start, GridUnit end, BattleData battleData)
        {
            List<AStarData> openSet = new List<AStarData>();
            List<AStarData> closeSet = new List<AStarData>();

            AStarData startNode = FindInDic(start.data.row, start.data.col, battleData);
            AStarData endNode = FindInDic(end.data.row, end.data.col, battleData);
            
            int limit = 0;

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                AStarData currNode = openSet[0];

                for (int i = 0; i < openSet.Count; i++)
                {
                    if (currNode.F > openSet[i].F || (currNode.F == openSet[i].F && currNode.H > openSet[i].H))
                    {
                        currNode = openSet[i];
                    }
                }

                openSet.Remove(currNode);
                closeSet.Add(currNode);

                if (currNode == endNode)
                {
                    Path(startNode, currNode, battleData);
                    return;
                }

                foreach (AStarData node in GetNeighbour(currNode, battleData))
                {
                    if (closeSet.Contains(node)) continue;

                    //代表当前邻居应该有的代价 主要是为第一次判断做铺垫
                    int newMoveCost = currNode.G + GetG(currNode, node);
                    //如果没有搜查过 或者 G值大
                    if (newMoveCost < node.G || !openSet.Contains(node))
                    {

                        node.G = newMoveCost;
                        node.H = GetH(node, endNode);
                        node.preNode = currNode;

                        if (!openSet.Contains(node))
                        {
                            openSet.Add(node);
                        }
                    }
                }
                ++limit;
                if (limit == battleData.mapHeight * battleData.mapWidth)
                {
                    Debug.Log("达到最大次数");
                    Path(currNode, startNode, battleData);
                    return;
                }
            }
            Debug.Log("搜查结束");
        }
        
        //生成导航路径
        public void Path(AStarData start, AStarData end, BattleData battleData)
        {
            AStarData startNode = FindInDic(start.row, start.col, battleData);
            AStarData endNode = FindInDic(end.row, end.col, battleData);
            AStarData currNode = endNode;
            while (currNode != startNode)
            {
                pathNode.Add(currNode);
                currNode = currNode.preNode;
            }
        }
        //输出路径
        public List<Vector3> Path()
        {
            List<Vector3> path = new List<Vector3>();
            if (path.Count > 0) path.Clear();
            foreach (var item in pathNode) path.Add(new Vector3(item.col * 3, 0.2f, item.row * 3));
            path.Reverse();
            return path;
        }
        //查找邻居
        public List<AStarData> GetNeighbour(AStarData curr, BattleData battleData)
        {
            List<AStarData> neighbour = new List<AStarData>();
            for (int i = -1; i <= 1; i++)
            {
                if (i == 0) continue;
                //行数 对应的是高
                int checkX = curr.row + i;
                //列数 对应的是宽
                int checkY = curr.col + i;

                //不限制范围 会在字典中查不到
                if (checkX >= 0 && checkX < battleData.mapWidth)
                {
                    for (int k = 0; k < battleData.normalGridUnits.Count; k++)
                    {
                        if (battleData.normalGridUnits[k].row == FindInDic(checkX, curr.col, battleData).row
                            && battleData.normalGridUnits[k].col == FindInDic(checkX, curr.col, battleData).col)
                        {
                            neighbour.Add(FindInDic(checkX, curr.col, battleData));
                        }
                    }
                }
                if (checkY >= 0 && checkY < battleData.mapHeight)
                {
                    for (int k = 0; k < battleData.normalGridUnits.Count; k++)
                    {
                        if (battleData.normalGridUnits[k].row == FindInDic(curr.row, checkY, battleData).row
                            && battleData.normalGridUnits[k].col == FindInDic(curr.row, checkY, battleData).col)
                        {
                            neighbour.Add(FindInDic(curr.row, checkY, battleData));
                        }
                    }
                }
            }
            return neighbour;
        }
        
        //在字典中查找数据
        public AStarData FindInDic(int row, int col, BattleData battleMapData)
        {
            Point point = new Point
            {
                row = row,
                col = col
            };
            //foreach (Point item in battleMapData.nodeDic.Keys)
            //{
            //    Debug.Log(item);
            //}
            return battleMapData.nodeDic[point];
        }
        //判断是否是相同位置
        public bool CheckPosition(int row1, int row2, int col1, int col2)
        {
            if (row1 == row2 && col1 == col2)
            {
                return true;
            }
            return false;
        }
        public int GetH(AStarData curr, AStarData end)
        {
            int x = Mathf.Abs(curr.row - end.row);
            int y = Mathf.Abs(curr.col - end.col);
            return (x + y) * 1;
        }
        public int GetG(AStarData pre, AStarData curr)
        {
            int x = Mathf.Abs(curr.row - pre.row);
            int y = Mathf.Abs(curr.col - pre.col);
            return (x + y) * 1;
        }
        /*计算范围===============================================*/
        List<AStarData> rangeList = new List<AStarData>();
        public void Range(GridUnit thisGrid, BattleData battleData, int rangeNum)
        {
            List<AStarData> openSet = new List<AStarData>();
            List<AStarData> closeSet = new List<AStarData>();
     
            AStarData startNode = FindInDic(thisGrid.data.row, thisGrid.data.col, battleData);

            startNode.G = 0;
            openSet.Add(startNode);
            bool isOver = false;
            while (!isOver)
            {
                AStarData currNode = openSet[0];
                for (int i = 0; i < openSet.Count; i++)
                {
                    if (currNode.G > openSet[i].G)
                    {
                        currNode = openSet[i];
                    }
                }

                openSet.Remove(currNode);
                closeSet.Add(currNode);

                foreach (AStarData node in GetNeighbour(currNode, battleData))
                {
                    if (closeSet.Contains(node)) continue;

                    //代表当前邻居应该有的代价 主要是为第一次判断做铺垫
                    int newMoveCost = currNode.G + GetG(currNode, node);
                    //如果没有搜查过 或者 G值大
                    if (newMoveCost < node.G || !openSet.Contains(node))
                    {
                        node.G = newMoveCost;
                        node.preNode = currNode;
                        if (!openSet.Contains(node))
                        {
                            openSet.Add(node);
                        }
                    }
                }

                

                

                for (int i = 0; i < closeSet.Count; i++)
                {
                    if (closeSet[i].G < rangeNum && openSet.Count == 0)
                    {
                        closeSet.Remove(closeSet[i]);
                        rangeList = closeSet;
                        isOver = true;
                    }
                    else if (closeSet[i].G == rangeNum)
                    {
                        closeSet.Remove(closeSet[i]);
                        if (closeSet.Count == 0)
                        {
                            isOver = true;
                            break;
                        }
                        rangeList = closeSet;
                        isOver = true;
                    }
                }
            }
            if (isOver)
            {
                Debug.Log("范围查找结束");
            }
        }
        //返回范围的路径
        public List<Vector3> RangePath()
        {
            List<Vector3> path = new List<Vector3>();
            foreach (var item in rangeList) path.Add(new Vector3(item.col * 3, 0.5f, item.row * 3) );
            return path;
        }
    }
}
