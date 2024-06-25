using UnityEngine;
using ExitGames.Client.Photon;
using System.Linq;

namespace Graduation_Design_Turn_Based_Game
{
    public class PhotonEngine : MonoBehaviour,IPhotonPeerListener
    {
        private static PhotonEngine instance;
        PhotonPeer peer;
        public void DebugReturn(DebugLevel level, string message)
        {

        }

        public void OnEvent(EventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            int operationCode = operationResponse.OperationCode;
            switch (operationCode)
            {
                case 0:
                    break;
                default:
                    break;
            }
            
        }
        

        public void OnStatusChanged(StatusCode statusCode)
        {

        }
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        // Start is called before the first frame update
        void Start()
        {
     //       peer = new PhotonPeer(this,ConnectionProtocol.Udp);
   //         peer.Connect("ns.photonengine.cn:0", "9d64279b-df8f-404b-badb-b28007fe20b5");
    
        }

        // Update is called once per frame
        void Update()
        {
        //    peer.Service();
            if (Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log(1573);
                ParameterDictionary parameters = new ParameterDictionary();
                OperationRequest operationRequest = new OperationRequest();
              
                SendOptions options = new SendOptions();
                options.Reliability = true;
                peer.SendOperation(0, parameters, options);
                
            }
        }
        private void OnDestroy()
        {
            if (peer != null && peer.PeerState == PeerStateValue.Connected)
            {
                peer.Disconnect();
            }
        }
    }
}