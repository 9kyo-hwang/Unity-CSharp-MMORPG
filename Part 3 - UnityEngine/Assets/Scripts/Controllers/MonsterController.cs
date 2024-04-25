using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class MonsterController : BaseController
{
	Stat _stat;

	[FormerlySerializedAs("_scanRange")] [SerializeField]
	float scanRange = 10;

	[FormerlySerializedAs("_attackRange")] [SerializeField]
	float attackRange = 2;

    public override void Init()
    {
		WorldObjectType = Define.WorldObject.Monster;
		_stat = gameObject.GetComponent<Stat>();

		if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
			Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
	}

	protected override void UpdateIdle()
	{
		GameObject player = Managers.Game.GetPlayer();
		if (player == null)
			return;

		float distance = (player.transform.position - transform.position).magnitude;
		if (!(distance <= scanRange)) return;
		
		lockTarget = player;
		State = Define.State.Moving;
	}

	protected override void UpdateMoving()
	{
		// 플레이어가 내 사정거리보다 가까우면 공격
		if (lockTarget != null)
		{
			_destPos = lockTarget.transform.position;
			float distance = (_destPos - transform.position).magnitude;
			if (distance <= attackRange)
			{
				NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
				nma.SetDestination(transform.position);
				State = Define.State.Skill;
				return;
			}
		}

		// 이동
		Vector3 dir = _destPos - transform.position;
		if (dir.magnitude < 0.1f)
		{
			State = Define.State.Idle;
		}
		else
		{
			NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
			nma.SetDestination(_destPos);
			nma.speed = _stat.MoveSpeed;

			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
		}
	}

	protected override void UpdateSkill()
	{
		if (lockTarget == null) return;
		
		Vector3 dir = lockTarget.transform.position - transform.position;
		Quaternion quat = Quaternion.LookRotation(dir);
		transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
	}

	void OnHitEvent()
	{
		if (lockTarget == null)
		{
			State = Define.State.Idle;
			return;
		}

		// 체력
		Stat targetStat = lockTarget.GetComponent<Stat>();
		targetStat.OnAttacked(_stat);

		if (targetStat.Hp <= 0)
		{
			State = Define.State.Idle;
			return;
		}

		float distance = (lockTarget.transform.position - transform.position).magnitude;
		State = distance <= attackRange ? Define.State.Skill : Define.State.Moving;
	}
}
