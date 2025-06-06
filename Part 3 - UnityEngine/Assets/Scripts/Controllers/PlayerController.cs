﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : BaseController
{
	private const int Mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

	PlayerStat _stat;
	bool _stopSkill = false;

	public override void Init()
    {
		WorldObjectType = Define.WorldObject.Player;
		_stat = gameObject.GetComponent<PlayerStat>();
		Managers.Input.MouseAction -= OnMouseEvent;
		Managers.Input.MouseAction += OnMouseEvent;

		if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
			Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
	}

	protected override void UpdateMoving()
	{
		// 몬스터가 내 사정거리보다 가까우면 공격
		if (lockTarget != null)
		{
			_destPos = lockTarget.transform.position;
			float distance = (_destPos - transform.position).magnitude;
			if (distance <= 1)
			{
				State = Define.State.Skill;
				return;
			}
		}

		// 이동
		Vector3 dir = _destPos - transform.position;
		dir.y = 0;

		if (dir.magnitude < 0.1f)
		{
			State = Define.State.Idle;
			return;
		}

		Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.green);
		if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))
		{
			if (Input.GetMouseButton(0) == false)
				State = Define.State.Idle;
			return;
		}

		float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
		transform.position += dir.normalized * moveDist;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
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
		if (lockTarget != null)
		{
			Stat targetStat = lockTarget.GetComponent<Stat>();
			targetStat.OnAttacked(_stat);
		}

		State = _stopSkill ? Define.State.Idle : Define.State.Skill;
	}

	void OnMouseEvent(Define.MouseEvent evt)
	{
		switch (State)
		{
			case Define.State.Idle:
				OnMouseEvent_IdleRun(evt);
				break;
			case Define.State.Moving:
				OnMouseEvent_IdleRun(evt);
				break;
			case Define.State.Skill:
				{
					if (evt == Define.MouseEvent.PointerUp)
						_stopSkill = true;
				}
				break;
		}
	}

	void OnMouseEvent_IdleRun(Define.MouseEvent evt)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		bool raycastHit = Physics.Raycast(ray, out var hit, 100.0f, Mask);
		//Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

		switch (evt)
		{
			case Define.MouseEvent.PointerDown:
				{
					if (raycastHit)
					{
						_destPos = hit.point;
						State = Define.State.Moving;
						_stopSkill = false;

						if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
							lockTarget = hit.collider.gameObject;
						else
							lockTarget = null;
					}
				}
				break;
			case Define.MouseEvent.Press:
				{
					if (lockTarget == null && raycastHit)
						_destPos = hit.point;
				}
				break;
			case Define.MouseEvent.PointerUp:
				_stopSkill = true;
				break;
		}
	}
}
