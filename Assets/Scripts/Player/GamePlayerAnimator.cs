using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Graduation_Design_Turn_Based_Game
{
    public class GamePlayerAnimator : MonoBehaviourPun
    {

        private Animator animator;
        private void OnEnable()
        {
            animator = GetComponent<Animator>();
        }
        void Update()
        {

        }
        public void SynchronizeAnimationState(string animName)
        {
            photonView.RPC("SynAnimation", RpcTarget.All, animName);
        }
        [PunRPC]
        private void SynAnimation(string animName)
        {
            animator.Play(animName);
        }
    }
}
