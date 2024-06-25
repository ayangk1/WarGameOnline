using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Graduation_Design_Turn_Based_Game
{
    public enum TalentState
    {
        None,//��
        Run,//�������� ��������һ��
        Health,//��Ѫ�����ӱ� ÿ��10%Ѫ�� ����2����
        Shield,//��û������� ÿ�εõ����ܼӳɵ�ʱ �����������
        Life,//����һ��
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
                    TalentName = "����";
                    break;
                case "Health":
                    _Talent = TalentState.Health;
                    TalentName = "��Ѫ";
                    break;
                case "Shield":
                    _Talent = TalentState.Shield;
                    TalentName = "����";
                    break;
                case "Life":
                    _Talent = TalentState.Life;
                    TalentName = "����";
                    break;
                default:
                    break;
            }
        }
    }
}