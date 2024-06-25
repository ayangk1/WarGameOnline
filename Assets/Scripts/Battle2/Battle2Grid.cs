using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graduation_Design_Turn_Based_Game
{
    public class Battle2Grid : MonoBehaviour
    {
        public Battle2GridData data;
        public bool canDispose;
        public bool CanDispose
        {
            get { return canDispose; }
            set 
            {
                if (value == true)
                {
                    canDispose = value;
                    transform.tag = "canDispose";
                }
                else
                {
                    canDispose = value;
                    transform.tag = "notTouch";
                }
            }
        }
        
        private void OnEnable()
        {
            CanDispose = true;
            if (transform.position == new Vector3(12.5f,0,7.5f))
            {
                MeshRenderer renderer = GetComponent<MeshRenderer>();
                renderer.material.color = Color.green;
            }
        }
        public void Dispose()
        {
            canDispose = false;
        }
    }
}