using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graduation_Design_Turn_Based_Game
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;
        private void Awake()
        {
            Instance = this;
        }
        List<Vector3> path;
        Vector3 nextPos;
        int posIndex;

        private void Start()
        {
            path = new List<Vector3>();
            posIndex = 0;
        }
        public void AddPath(List<Vector3> newPath)
        {
            path = newPath;
            nextPos = path[posIndex];
        }
        public void MoveStart()
        {
            PlayerManager.Instance.player.transform.LookAt(nextPos);
            Hashtable hash = new Hashtable();
            hash.Add("easetype", iTween.EaseType.linear);
            hash.Add("speed", 5);
            hash.Add("position", nextPos);
            hash.Add("oncomplete", "MoveOver");
            hash.Add("oncompletetarget", gameObject);
            iTween.MoveTo(PlayerManager.Instance.player, hash);
        }
        public void MoveOver()
        {
            if (posIndex + 1 == path.Count)
            {
                PlayerManager.Instance.player.transform.LookAt(PlayerManager.Instance.enemy.transform);
                posIndex = 0;
                PlayerManager.Instance.ResetData();
                path.Clear();
                return;
            }
               
            posIndex++;
            nextPos = path[posIndex];
            MoveStart();
        }
    }
}
