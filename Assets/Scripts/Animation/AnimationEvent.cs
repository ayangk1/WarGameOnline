using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graduation_Design_Turn_Based_Game
{
    public class AnimationEvent : MonoBehaviour
    {
        //提示信息的显示动画
        public void PromptClose()
        {
            UIManager.Instance.Prompt.text = "";
            UIManager.Instance.Prompt = UIManager.Instance.Prompt;
        }
    }
}