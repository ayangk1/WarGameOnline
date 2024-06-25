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
                        prompt.text = "行走:" + "行走增加一格";
                        costomProperties = new ExitGames.Client.Photon.Hashtable() { { "Talent", "Run" } };
                        PhotonNetwork.SetPlayerCustomProperties(costomProperties);
                    });
                    break;
                case 1:
                    talents.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        prompt.text = "嗜血:" + "血量每减掉5%，攻击增加2点";
                        costomProperties = new ExitGames.Client.Photon.Hashtable() { { "Talent", "Health" } };
                        PhotonNetwork.SetPlayerCustomProperties(costomProperties);
                    });
                    break;
                case 2:
                    talents.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        prompt.text = "护盾:" + "每次获得护盾时，增加一倍";
                        costomProperties = new ExitGames.Client.Photon.Hashtable() { { "Talent", "Shield" } };
                        PhotonNetwork.SetPlayerCustomProperties(costomProperties);
                    });
                    break;
                case 3:
                    talents.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        prompt.text = "复活:" + "玩家死亡后，复活";
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
            prompt.text = "请选择一个天赋";
        }
        Debug.Log("确认成功");
    }
    [PunRPC]//选择成功的人数
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
