using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Graduation_Design_Turn_Based_Game
{
    
    public class BuffIcon : MonoBehaviour
    {
        Image image;
        bool isInit;

        void Update()
        {
            if (!isInit)
            {
                Image image = GetComponent<Image>();
                switch (transform.gameObject.name)
                {
                    case "AddCrit":
                        image.sprite = DataManager.Instance.GameConfi.AddCrit;
                        break;
                    case "AddAttackFrequency":
                        image.sprite = DataManager.Instance.GameConfi.AddAttackFrequency;
                        break;
                    default:
                        break;
                }
                isInit = true;
            }
        }
    }
}
