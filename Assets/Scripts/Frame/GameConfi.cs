using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��Ϸ����
/// </summary>
[CreateAssetMenu(fileName = "GameConfi", menuName = "GameConfi")]
public class GameConfi : ScriptableObject
{
    [Tooltip("������Ч")]
    public GameObject AttackEffect;

    [Header("Ԥ����")]
    [Tooltip("BUFFͼ��Ԥ����")]
    public GameObject BuffState;
    [Tooltip("����Ԥ����")]
    public GameObject RoomPrefab;

    [Header("�츳��ͼ")]
    [Tooltip("����")]
    public Sprite FastWalk;
    [Tooltip("��Ѫ")]
    public Sprite UnhealthAttack;
    [Tooltip("����")]
    public Sprite ShieldPlus;
    [Tooltip("����")]
    public Sprite Resurgence;

    [Header("buff��ͼ")]
    [Tooltip("����Ѫ��")]
    public Sprite AddHealth;
    [Tooltip("���ӹ�����")]
    public Sprite AddAttack;
    [Tooltip("���ӹ�������")]
    public Sprite AddAttackFrequency;
    [Tooltip("����һ���غ�")]
    public Sprite AddRound;
    [Tooltip("���ӱ���")]
    public Sprite AddCrit;
    [Tooltip("����10����")]
    public Sprite AddShieldValue;

}
