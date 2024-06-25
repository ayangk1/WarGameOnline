using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;
using Photon.Realtime;

namespace Graduation_Design_Turn_Based_Game
{
    public class Battle : MonoBehaviourPunCallbacks
    {
        public static Battle Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }
        //���������Ѿ������ĸ���
        public List<GridUnit> gridUnitsList;
        public BattleData mapData;
        //����ĸ�����
        [SerializeField] public Transform root;
        //����ķ�Ƭ
        [SerializeField] public Transform tile;
        private const byte GRIDUNIT_CHANGE_EVENT = 31;

        private new void OnEnable()
        {
            mapData = new BattleData();
            PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
        }
        private void Start()
        {
            mapData = new BattleData();
        }
        public void LoadBattle(BattleData data)
        {
            mapData = data;
            PreBattle();

        }
        void PreBattle()
        {
            for (int i = 0; i < mapData.mapWidth; i++)
            {
                for (int j = 0; j < mapData.mapHeight; j++)
                {
                    CreatePhotonGrid(i, j);
                }
            }
        }
        //��������
        private void CreatePhotonGrid(int i, int j)
        {
            //ʵ��������
            var gridUnit = PhotonNetwork.Instantiate(tile.name, Vector3.zero, Quaternion.identity).GetComponent<GridUnit>();
            GridUnitData gud = mapData.mapPos[i, j];
            gridUnit.data = gud;
            gridUnit.name = i + "��" + j + "��";
            gridUnit.transform.position = gud.worldPos;
            gridUnit.transform.SetParent(root);
            gridUnit.RefreshColor();
            InitTag(gridUnit);
            gridUnitsList.Add(gridUnit);

            //���������
            GridType gridType = gud.gridType;
            int row = gud.row;
            int col = gud.col;
            Vector3 worldPos = gud.worldPos;
            Vector2 gridPos = gud.gridPos;
            int mapWidth = mapData.mapWidth;
            int mapHeight = mapData.mapHeight;
            //�������� �������� �������� �� �� ��ͼ�� ��ͼ��
            object[] datas = new object[] { gridType, worldPos, gridPos, row, col, mapWidth, mapHeight };

            PhotonNetwork.RaiseEvent(GRIDUNIT_CHANGE_EVENT, datas, RaiseEventOptions.Default, SendOptions.SendUnreliable);
        }
        //��ȡ����
        private void NetworkingClient_EventReceived(EventData obj)
        {
            //ֻ���ܺ�����һ����
            if (obj.Code == GRIDUNIT_CHANGE_EVENT)
            {
                object[] datas = (object[])obj.CustomData;
                Point point = new Point
                {
                    row = (int)datas[3],
                    col = (int)datas[4],
                };
                AStarData node = new AStarData((int)datas[3], (int)datas[4]);
                //�ֵ䲻������ظ��ļ�ֵ
                if (!mapData.nodeDic.ContainsKey(point))
                    mapData.nodeDic.Add(point, node);
                mapData.mapWidth = (int)datas[5];
                mapData.mapHeight = (int)datas[6];
            }
        }
        //�������
        public void ClearData()
        {
            mapData = null;
        }

        ////�����Ⱦ��ɫ��tag
        //public void ClearRenderType(params object[] obj)
        //{
        //    Debug.Log(obj.Length);
        //    foreach (var item in gridUnitsList)
        //    {
        //        if (obj.Length > 0)
        //        {
        //            foreach (var data in obj)
        //            {
        //                if (item.RenderType == (GridRenderType)data)
        //                    continue;

        //                item.RenderType = GridRenderType.none;
        //                InitTag(item);
        //            }
        //        }
        //        else if (obj.Length == 0)
        //        {
        //            item.RenderType = GridRenderType.none;
        //            InitTag(item);
        //        }
        //    }
        //}
        //�����Ⱦ��ɫ��tag
        public void ClearRenderType()
        {
            foreach (var item in gridUnitsList)
            {
                item.RenderType = GridRenderType.none;
                InitTag(item);
            }
        }
        //��ʼ��tag
        public void InitTag(GridUnit gridUnit)
        {
            switch (gridUnit.data.gridType)
            {
                case GridType.normal:
                    gridUnit.gameObject.tag = "tile";
                    break;
                case GridType.obstacle:
                    gridUnit.gameObject.tag = "obstacle";
                    break;
                case GridType.born:
                    gridUnit.gameObject.tag = "born";
                    break;
                case GridType.item:
                    break;
                default:
                    break;
            }
        }
        //������ɫ
        public void PaintColor(List<Vector3> pos, GridRenderType renderType, int limit)
        {
            List<GridUnit> gridUnits = new List<GridUnit>();
            for (int i = 0; i < pos.Count - limit; i++)
            {
                gridUnits.Add(GetGridUnit(pos[i]));
            }
            foreach (var item in gridUnits)
            {
                item.RenderType = renderType;
            }
        }
        //��õ�λ����
        public GridUnit GetGridUnit(Vector3 clicked)
        {
            clicked = root.transform.InverseTransformPoint(clicked);
            float dis = 10000;
            GridUnit gu = null;
            for (int i = 0; i < gridUnitsList.Count; i++)
            {
                if (Mathf.Abs(gridUnitsList[i].data.worldPos.x - clicked.x) < dis
                    && Mathf.Abs(gridUnitsList[i].data.worldPos.z - clicked.z) < dis)
                {
                    gu = gridUnitsList[i];
                    dis = Vector3.Distance(gridUnitsList[i].data.worldPos, clicked);
                }
            }
            return gu;
        }
    }
}

