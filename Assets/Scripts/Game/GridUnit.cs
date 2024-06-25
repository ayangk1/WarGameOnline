using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

namespace Graduation_Design_Turn_Based_Game
{
    public enum GridRenderType
    {
        none,//��
        path,//����·��
        start,//������ʼλ��
        end,//��������λ��
        search,//���Ѳ�λ��
        range,//��Χ
        enemyTile,//����λ��
        canAttackEnemyTile,//���Թ����ĵ���λ��
        playerTile,//�Լ�λ��
        boxTile,//���ӵ���λ��
        skill,//���ܵ�λ��
        golemTile,//ʯͷ��
        canTouch,//�ɽ������
    }
    public class GridUnit : MonoBehaviourPunCallbacks
    {
        
        public GridUnitData data;
        public MeshRenderer meshColor;
        private GridRenderType gridRenderType;
        private const byte GRIDUNIT_CHANGE_EVENT = 31;
        public GridRenderType RenderType
        {
            set
            {
                gridRenderType = value;
                RefreshColor();
            }
            get { return gridRenderType; }
        }
        private new void OnEnable()
        {
            meshColor = GetComponent<MeshRenderer>();
            PhotonNetwork.AddCallbackTarget(this);
            PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
        }
        public override void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
            PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
        }
        private void NetworkingClient_EventReceived(EventData obj)
        {
            if (obj.Code == GRIDUNIT_CHANGE_EVENT)
            {
                if (PhotonNetwork.IsMasterClient || data != null) return;
                if (Battle.Instance.mapData.normalGridUnits == null)
                    Battle.Instance.mapData.normalGridUnits = new List<GridUnitData>();
                if (Battle.Instance.mapData.bornGridUnits == null)
                    Battle.Instance.mapData.bornGridUnits = new List<GridUnitData>();
                if (Battle.Instance.mapData.obstacleGridUnits == null)
                    Battle.Instance.mapData.obstacleGridUnits = new List<GridUnitData>();

                object[] datas = (object[])obj.CustomData;
                data = new GridUnitData();
                data.gridType = (GridType)datas[0];
                data.worldPos = (Vector3)datas[1];
                data.gridPos = (Vector2)datas[2];
                data.row = (int)datas[3];
                data.col = (int)datas[4];
                gameObject.transform.SetParent(Battle.Instance.root);
                Battle.Instance.gridUnitsList.Add(gameObject.GetComponent<GridUnit>());
                if (data.gridType == GridType.normal)
                    Battle.Instance.mapData.normalGridUnits.Add(data);
                if (data.gridType == GridType.born)
                    Battle.Instance.mapData.bornGridUnits.Add(data);
                if (data.gridType == GridType.obstacle)
                    Battle.Instance.mapData.obstacleGridUnits.Add(data);

                //�����ͻ��˵�tile��ֵλ�õĻ����ܿ������������ɵ�λ�ã�����ȡ�����껹��0,0,0 ������Ҫ��ֵ
                gameObject.transform.position = data.worldPos; 
                gameObject.GetComponent<GridUnit>().name = data.col + "��" + data.row + "��";
                RefreshColor();
                Battle.Instance.InitTag(gameObject.GetComponent<GridUnit>());
            }
        }

        public void RefreshColor()
        {
            switch (data.gridType)
            {
                case GridType.none:
                    break;
                case GridType.normal:
                    meshColor.material.color = Color.white;
                    gameObject.tag = "tile";
                    break;
                case GridType.obstacle:
                    meshColor.material.color = Color.black;
                    gameObject.tag = "obstacle";
                    break;
                case GridType.born:
                    meshColor.material.color = Color.blue;
                    gameObject.tag = "born";
                    break;
                case GridType.item:
                    meshColor.material.color = Color.green;
                    break;
                default:
                    break;
            }
            switch (RenderType)
            {
                case GridRenderType.none:

                    break;
                case GridRenderType.path:
                    meshColor.material.color = Color.yellow;
                    break;
                case GridRenderType.start:
                    meshColor.material.color = Color.green;
                    break;
                case GridRenderType.end:
                    meshColor.material.color = Color.blue;
                    break;
                case GridRenderType.search:
                    meshColor.material.color = Color.yellow;
                    break;
                case GridRenderType.range:
                    meshColor.material.color = Color.yellow;
                    gameObject.tag = "range";
                    break;
                case GridRenderType.enemyTile:
                    meshColor.material.color = Color.red;
                    break;
                case GridRenderType.canAttackEnemyTile:
                    meshColor.material.color = new Color(1,0.75f,0.8f,1);
                    gameObject.tag = "canAttackEnemyTile";
                    break;
                case GridRenderType.playerTile:
                    meshColor.material.color = Color.green;
                    gameObject.tag = "playerTile";
                    break;
                case GridRenderType.boxTile:
                    meshColor.material.color = Color.green;
                    gameObject.tag = "boxTile";
                    break;
                case GridRenderType.skill:
                    meshColor.material.color = Color.green;
                    gameObject.tag = "skill";
                    break;
                case GridRenderType.golemTile:
                    meshColor.material.color = Color.blue;
                    gameObject.tag = "golemTile";
                    break;
                case GridRenderType.canTouch:
                    meshColor.material.color = new Color(1, 0.75f, 0.8f, 1);
                    gameObject.tag = "canTouch";
                    break;
                default:
                    break;
            }
        }

        public void SynchronizeState(GridRenderType render, GridType gridType)
        {
            photonView.RPC("SynData", RpcTarget.All, render, gridType);
        }
        [PunRPC]
        private void SynData(GridRenderType render,GridType gridType)
        {
            if (gridType != GridType.none)
            {
                data.gridType = gridType;
                RenderType = render;
            }
            else
            {
                RenderType = render;
            }
            
            
        }
    }
}

