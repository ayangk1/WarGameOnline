using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviourPun
{
    private void OnEnable()
    {
        //≥ı ºªØtag
        if (photonView.IsMine)
        {
            transform.name = "playerWeapon";
        }
        else
        {
            transform.name = "enemyWeapon";
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
