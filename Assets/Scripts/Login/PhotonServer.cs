using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class PhotonServer : IPhotonSocket
{
    public PhotonServer(PeerBase peerBase) : base(peerBase)
    {
    }

    public override bool Disconnect()
    {
        throw new System.NotImplementedException();
    }

    public override PhotonSocketError Receive(out byte[] data)
    {
        throw new System.NotImplementedException();
    }

    public override PhotonSocketError Send(byte[] data, int length)
    {
        throw new System.NotImplementedException();
    }
}
