using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TalentPanel : MonoBehaviourPunCallbacks
{
    public GameObject talents;
    public Text prompt;
    int overNum;
    ExitGames.Client.Photon.Hashtable costomProperties;
    new void OnEnable()
    {
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    talents.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        prompt.text = "����:" + "��������һ��";
                        costomProperties = new ExitGames.Client.Photon.Hashtable() { { "Talent", "Run" } };
                        PhotonNetwork.SetPlayerCustomProperties(costomProperties);
                    });
                    break;
                case 1:
                    talents.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        prompt.text = "��Ѫ:" + "Ѫ��ÿ����5%����������2��";
                        costomProperties = new ExitGames.Client.Photon.Hashtable() { { "Talent", "Health" } };
                        PhotonNetwork.SetPlayerCustomProperties(costomProperties);
                    });
                    break;
                case 2:
                    talents.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        prompt.text = "����:" + "ÿ�λ�û���ʱ������һ��";
                        costomProperties = new ExitGames.Client.Photon.Hashtable() { { "Talent", "Shield" } };
                        PhotonNetwork.SetPlayerCustomProperties(costomProperties);
                    });
                    break;
                case 3:
                    talents.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        prompt.text = "����:" + "��������󣬸���";
                        costomProperties = new ExitGames.Client.Photon.Hashtable() { { "Talent", "Life" } };
                        PhotonNetwork.SetPlayerCustomProperties(costomProperties);
                    });
                    break;
                default:
                    break;
            }
        }
    }
    public void ConfirmButton()
    {
        if ((string)PhotonNetwork.LocalPlayer.CustomProperties["Talent"] != "None")
        {
            costomProperties = new ExitGames.Client.Photon.Hashtable() { { "IsReady",true } };
            PhotonNetwork.SetPlayerCustomProperties(costomProperties);
            photonView.RPC("SelectOverNum", RpcTarget.All);
        }
        else
        {
            prompt.text = "��ѡ��һ���츳";
        }
        Debug.Log("ȷ�ϳɹ�");
    }
    [PunRPC]//ѡ��ɹ�������
    public void SelectOverNum()
    {
        overNum++;
    }
    private void Update()
    {
        if (overNum == 2)
        {
            PhotonNetwork.LoadLevel(1);
            overNum = 0;
        }
    }
}
