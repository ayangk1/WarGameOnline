using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Battle2CardManager : MonoBehaviour
{
    public static Battle2CardManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public GameObject _CardPanel;
    public bool isDispose;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _CardPanel.transform.childCount; i++)
        {
            Button button = _CardPanel.transform.GetChild(i).GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate ()
            {
                GameObject[] objects = GameObject.FindGameObjectsWithTag("canDispose");
                isDispose = true;
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDispose)
        {

        }
    }
}
