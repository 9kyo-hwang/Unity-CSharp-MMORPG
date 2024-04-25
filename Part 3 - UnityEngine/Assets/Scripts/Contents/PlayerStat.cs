using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerStat : Stat
{
    [FormerlySerializedAs("_exp")] [SerializeField]
	protected int exp;
    [FormerlySerializedAs("_gold")] [SerializeField]
	protected int gold;

	public int Exp 
	{ 
		get => exp;
		set 
		{ 
			exp = value;

			int level = 1;
			while (true)
			{
				Data.Stat stat;
				if (Managers.Data.StatDict.TryGetValue(level + 1, out stat) == false)
					break;
				if (exp < stat.totalExp)
					break;
				level++;
			}

			if (level == Level) return;
			
			Debug.Log("Level Up!");
			Level = level;
			SetStat(Level);
		}
	}

	public int Gold
	{
		get => gold;
		set => gold = value;
	}

	private void Start()
	{
		level = 1;
		exp = 0;
		defense = 5;
		moveSpeed = 5.0f;
		gold = 0;

		SetStat(level);
	}

	public void SetStat(int level)
	{
		Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
		Data.Stat stat = dict[level];
		hp = stat.maxHp;
		maxHp = stat.maxHp;
		attack = stat.attack;
	}

	protected override void OnDead(Stat attacker)
	{
		Debug.Log("Player Dead");
	}
}
