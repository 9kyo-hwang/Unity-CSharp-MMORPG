using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : Controller
{
    private Coroutine _coroutineAttack;
    
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }
    
    protected override void Update()
    {
        switch (State)
        {
            case EState.Idle:
                SetMoveDir();
                OnIdle();
                break;
            case EState.Move:
                SetMoveDir();
                break;
            case EState.Skill:
                break;
            case EState.Dead:
                break;
        }
        
        base.Update();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        
        // 2D라서 기본적으로 Z축은 -10 설정
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // 키보드 입력에 따른 방향 정의
    void SetMoveDir()
    {
        if (Input.GetKey(KeyCode.W))
        {
            CurMoveDir = EMoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            CurMoveDir = EMoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            CurMoveDir = EMoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            CurMoveDir = EMoveDir.Right;
        }
        else
        {
            CurMoveDir = EMoveDir.None;
        }
    }

    protected override void OnIdle()
    {
        base.OnIdle();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            State = EState.Skill;
            _coroutineAttack = StartCoroutine(nameof(AttackRoutine));
        }
    }

    private IEnumerator AttackRoutine()
    {
        // 피격 판정
        GameObject target = Managers.Object.Find(GetFrontPosition());
        if (target)
        {
            Debug.Log(target.name);
        }
        
        // 쿨타임
        yield return new WaitForSeconds(0.5f);
        State = EState.Idle;
        _coroutineAttack = null;
    }
}
