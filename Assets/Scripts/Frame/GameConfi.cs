using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏配置
/// </summary>
[CreateAssetMenu(fileName = "GameConfi", menuName = "GameConfi")]
public class GameConfi : ScriptableObject
{
    [Tooltip("攻击特效")]
    public GameObject AttackEffect;

    [Header("预制体")]
    [Tooltip("BUFF图标预制体")]
    public GameObject BuffState;
    [Tooltip("房间预制体")]
    public GameObject RoomPrefab;

    [Header("天赋贴图")]
    [Tooltip("行走")]
    public Sprite FastWalk;
    [Tooltip("嗜血")]
    public Sprite UnhealthAttack;
    [Tooltip("护盾")]
    public Sprite ShieldPlus;
    [Tooltip("复活")]
    public Sprite Resurgence;

    [Header("buff贴图")]
    [Tooltip("增加血量")]
    public Sprite AddHealth;
    [Tooltip("增加攻击力")]
    public Sprite AddAttack;
    [Tooltip("增加攻击次数")]
    public Sprite AddAttackFrequency;
    [Tooltip("增加一个回合")]
    public Sprite AddRound;
    [Tooltip("增加暴击")]
    public Sprite AddCrit;
    [Tooltip("增加10护盾")]
    public Sprite AddShieldValue;

}
