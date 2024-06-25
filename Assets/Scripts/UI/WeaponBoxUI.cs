using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Graduation_Design_Turn_Based_Game
{
    public class WeaponBoxUI : MonoBehaviour
    {
        private void OnEnable()
        {
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate () {
                    switch (i)
                    {
                        // 0-Wand 1-Bow 2-Sword
                        case 0:
                            Weapon(WeaponState.Wand);
                            break;
                        case 1:
                            Weapon(WeaponState.Bow);
                            break;
                        case 2:

                            Weapon(WeaponState.Sword);
                            WeaponManager.Instance.Weapon = WeaponState.Sword;
                            transform.gameObject.SetActive(false);
                            break;
                        default:
                            break;
                    }
                });
            }
        }
        public void Weapon(WeaponState weapon)
        {
            transform.gameObject.SetActive(false);
        }

    }
}