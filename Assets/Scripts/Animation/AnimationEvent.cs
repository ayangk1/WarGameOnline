using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graduation_Design_Turn_Based_Game
{
    public class AnimationEvent : MonoBehaviour
    {
        //��ʾ��Ϣ����ʾ����
        public void PromptClose()
        {
            UIManager.Instance.Prompt.text = "";
            UIManager.Instance.Prompt = UIManager.Instance.Prompt;
        }
    }
}