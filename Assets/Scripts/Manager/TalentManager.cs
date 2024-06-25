using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Graduation_Design_Turn_Based_Game
{
    public enum TalentState
    {
        None,//空
        Run,//快速行走 行走增加一格
        Health,//低血攻击加倍 每掉10%血量 增加2攻击
        Shield,//获得护盾增加 每次得到护盾加成的时 获得两倍护盾
        Life,//复活一次
    }
    public class TalentManager : MonoBehaviourPunCallbacks
    {
        public static TalentManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }
        public TalentState _Talent;
        public string TalentName;
        private void Start()
        {
            switch ((string)PhotonNetwork.LocalPlayer.CustomProperties["Talent"])
            {
                case "None":
                    _Talent = TalentState.None;
                    break;
                case "Run":
                    _Talent = TalentState.Run;
                    TalentName = "行走";
                    break;
                case "Health":
                    _Talent = TalentState.Health;
                    TalentName = "嗜血";
                    break;
                case "Shield":
                    _Talent = TalentState.Shield;
                    TalentName = "护盾";
                    break;
                case "Life":
                    _Talent = TalentState.Life;
                    TalentName = "复活";
                    break;
                default:
                    break;
            }
        }
    }
}