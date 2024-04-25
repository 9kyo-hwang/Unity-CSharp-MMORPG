using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Stat : MonoBehaviour
{
    [FormerlySerializedAs("_level")] [SerializeField]
    protected int level;
    [FormerlySerializedAs("_hp")] [SerializeField]
    protected int hp;
    [FormerlySerializedAs("_maxHp")] [SerializeField]
    protected int maxHp;
    [FormerlySerializedAs("_attack")] [SerializeField]
    protected int attack;
    [FormerlySerializedAs("_defense")] [SerializeField]
    protected int defense;
    [FormerlySerializedAs("_moveSpeed")] [SerializeField]
    protected float moveSpeed;

    public int Level
    {
        get => level;
        set => level = value;
    }
    public int Hp
    {
        get => hp;
        set => hp = value;
    }
    public int MaxHp
    {
        get => maxHp;
        set => maxHp = value;
    }
    public int Attack
    {
        get => attack;
        set => attack = value;
    }
    public int Defense
    {
        get => defense;
        set => defense = value;
    }
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    private void Start()
    {
        level = 1;
        hp = 100;
        maxHp = 100;
        attack = 10;
        defense = 5;
        moveSpeed = 5.0f;
    }

    public virtual void OnAttacked(Stat attacker)
    {
		int damage = Mathf.Max(0, attacker.Attack - Defense);
		Hp -= damage;
        if (Hp > 0) return;
        
        Hp = 0;
        OnDead(attacker);
    }

    protected virtual void OnDead(Stat attacker)
    {
		PlayerStat playerStat = attacker as PlayerStat;
		if (playerStat != null)
		{
            playerStat.Exp += 15;
		}

        Managers.Game.Despawn(gameObject);
    }
}
