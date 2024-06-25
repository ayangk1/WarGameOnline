using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Graduation_Design_Turn_Based_Game
{
    public class Battle2PlayerManager : MonoBehaviourPunCallbacks
    {
        public static Battle2PlayerManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            PlayerTouch();
        }
        //玩家交互
        public void PlayerTouch()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //点击玩家
                if (hit.transform.tag == "canDispose" && Input.GetMouseButtonDown(0))
                {
                    if (Battle2CardManager.Instance.isDispose)
                    {
                        PhotonNetwork.Instantiate("Battle2Golem", hit.transform.position, Quaternion.identity);
                        hit.transform.GetComponent<Battle2Grid>().Dispose();
                        Battle2CardManager.Instance.isDispose = false;
                    }
                }
            }
        }
    }
}