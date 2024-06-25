using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graduation_Design_Turn_Based_Game
{
    public class TileManager : MonoBehaviour
    {
        public static TileManager Instance;
        private void Awake()
        {
            Instance = this;
            isFindEnemyTile = true;
            isFindPlayerTile = true;
        }
        public bool isFindEnemyTile;
        public bool isFindPlayerTile;
        void Update()
        {
            //if (PlayerManager.Instance.player == null || PlayerManager.Instance.enemy == null) return;
            
            ////如果要给玩家的版块上色 并且还没有上色
            //if (isFindPlayerTile && Battle.Instance.GetGridUnit(PlayerManager.Instance.player.transform.position).RenderType != GridRenderType.playerTile)
            //    Battle.Instance.GetGridUnit(PlayerManager.Instance.player.transform.position).RenderType = GridRenderType.playerTile;
            ////如果要给敌人的版块上色 并且还没有上色
            //if (isFindEnemyTile && GetDisance(PlayerManager.Instance.player.transform.position, PlayerManager.Instance.enemy.transform.position)
            //    > PlayerManager.Instance.player.GetComponent<GamePlayer>().AttackRange &&
            //    Battle.Instance.GetGridUnit(PlayerManager.Instance.enemy.transform.position).RenderType != GridRenderType.enemyTile
            //    )
            //        Battle.Instance.GetGridUnit(PlayerManager.Instance.enemy.transform.position).RenderType = GridRenderType.enemyTile;
        }
        /*范围===================================================*/
        public void Range(Vector3 pos, float step, GridRenderType renderType,bool isPlayer)
        {
            Battle.Instance.ClearRenderType();
            AStar aStar = new AStar();
            aStar.Range(Battle.Instance.GetGridUnit(pos), Battle.Instance.mapData, (int)step + 1);

            //上色
            Battle.Instance.PaintColor(aStar.RangePath(), renderType, 0);
            if (isPlayer)
            {
                Battle.Instance.GetGridUnit(pos).RenderType = GridRenderType.playerTile;
                if (GetDisance(PlayerManager.Instance.player.transform.position, PlayerManager.Instance.enemy.transform.position) 
                    <= PlayerManager.Instance.player.GetComponent<GamePlayer>().AttackRange)
                    Battle.Instance.GetGridUnit(PlayerManager.Instance.enemy.transform.position).RenderType = GridRenderType.canAttackEnemyTile;
                if (GameObject.FindGameObjectWithTag("enemy").GetComponent<GamePlayer>().golem != null)
                {
                    foreach (var item in GameObject.FindGameObjectsWithTag("golem"))
                    {
                        if (item.GetComponent<Golem>().master == PlayerManager.Instance.enemy)
                        {
                            if (GetDisance(PlayerManager.Instance.player.transform.position, item.transform.position)
                                <= PlayerManager.Instance.player.GetComponent<GamePlayer>().AttackRange)
                                Battle.Instance.GetGridUnit(item.transform.position).RenderType = GridRenderType.canAttackEnemyTile;
                        }
                    }
                }
            }
            else
            {
                Battle.Instance.GetGridUnit(pos).RenderType = GridRenderType.golemTile;
                if (GetDisance(GolemManager.Instance.golem.transform.position, PlayerManager.Instance.enemy.transform.position)
                    <= GolemManager.Instance.golem.GetComponent<Golem>().attackRange)
                    Battle.Instance.GetGridUnit(PlayerManager.Instance.enemy.transform.position).RenderType = GridRenderType.canAttackEnemyTile;
            }
        }
        //改变方片标签
        public void AlterTileTag()
        {
            if (GetDisance(PlayerManager.Instance.player.transform.position,
                Battle.Instance.GetGridUnit(GameObject.FindGameObjectWithTag("boxTile").transform.position).transform.position) <= 1 &&
                GameObject.FindGameObjectWithTag("boxTile") != null)
            {
                Battle.Instance.GetGridUnit(GameObject.FindGameObjectWithTag("boxTile").transform.position).RenderType = GridRenderType.canTouch;
            }
            else if(
                GameObject.FindGameObjectWithTag("boxTile") == null && GetDisance(PlayerManager.Instance.player.transform.position,
                Battle.Instance.GetGridUnit(GameObject.FindGameObjectWithTag("canTouch").transform.position).transform.position) <= 1 )
            {
                return;
            }
            else if(
                GameObject.FindGameObjectWithTag("boxTile") == null && GetDisance(PlayerManager.Instance.player.transform.position,
                Battle.Instance.GetGridUnit(GameObject.FindGameObjectWithTag("canTouch").transform.position).transform.position) > 1)
            {
                Battle.Instance.GetGridUnit(GameObject.FindGameObjectWithTag("canTouch").transform.position).RenderType = GridRenderType.boxTile;
            }
        }
        //返回玩家和敌人的距离
        public int GetDisance(Vector3 pos1,Vector3 pos2)
        {
            AStar aStar = new AStar();
            aStar.Navigation(Battle.Instance.GetGridUnit(pos1),Battle.Instance.GetGridUnit(pos2), Battle.Instance.mapData);
            return aStar.Path().Count - 2;
        }
        //放置物品的版块
        public void DisposeItemTile(Item item)
        {
            for (int i = 0; i < 1; i++)
            {
                int tryTime = 999;
                int tryindex;
                GridUnitData target = null;
                while (tryTime > 0 && target == null)
                {
                    tryindex = Random.Range(0,Battle.Instance.mapData.normalGridUnits.Count);
                    target = Battle.Instance.mapData.normalGridUnits[tryindex];
                    foreach (GridUnitData obj in Battle.Instance.mapData.bornGridUnits)
                    {
                        //控制不在出生地旁边
                        if (Mathf.Abs(Battle.Instance.mapData.normalGridUnits[tryindex].gridPos.x - obj.gridPos.x) < 2 &&
                           Mathf.Abs(Battle.Instance.mapData.normalGridUnits[tryindex].gridPos.y - obj.gridPos.y) < 2)
                        {
                            target = null;
                            break;
                        }
                    }
                    tryTime--;
                }
                if (target != null)
                {
                    foreach (GridUnit obj in Battle.Instance.gridUnitsList)
                    {
                        if (obj.data == target)
                        {
                            switch (item)
                            {
                                case Item.Box:
                                    obj.SynchronizeState(GridRenderType.boxTile, GridType.item);

                                    break;
                                case Item.Golem:
                                    obj.SynchronizeState(GridRenderType.golemTile, GridType.none);
                                    
                                    break;
                            }
                            
                        }
                    }
                    
                }
            }
        }
    }
}
