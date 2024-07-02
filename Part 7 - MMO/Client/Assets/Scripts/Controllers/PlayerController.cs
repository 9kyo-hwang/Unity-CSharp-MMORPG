using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : Controller
{
    private Coroutine _skillRoutine;
    private bool _isArrowAttack = false;
    
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

    protected override void SetAnimation()
    {
        switch (State)
        {
            case EState.Idle:
                switch (prevMoveDir)
                {
                    case EMoveDir.Up:
                        Animator.Play("IDLE_BACK");
                        Sprite.flipX = false;
                        break;
                    case EMoveDir.Down:
                        Animator.Play("IDLE_FRONT");
                        Sprite.flipX = false;
                        break;
                    case EMoveDir.Left:
                        Animator.Play("IDLE_SIDE");
                        Sprite.flipX = true;
                        break;
                    case EMoveDir.Right:
                    case EMoveDir.None:  // None에 대해서는 Right와 동일한 처리
                        Animator.Play("IDLE_SIDE");
                        Sprite.flipX = false;
                        break;
                }
                break;
            case EState.Move:
                switch (curMoveDir)
                {
                    case EMoveDir.Up:
                        Animator.Play("WALK_BACK");
                        Sprite.flipX = false;
                        break;
                    case EMoveDir.Down:
                        Animator.Play("WALK_FRONT");
                        Sprite.flipX = false;
                        break;
                    case EMoveDir.Left:
                        Animator.Play("WALK_SIDE");
                        Sprite.flipX = true;
                        break;
                    case EMoveDir.Right:
                        Animator.Play("WALK_SIDE");
                        Sprite.flipX = false;
                        break;
                }
                break;
            case EState.Skill:
                switch (prevMoveDir)
                {
                    case EMoveDir.Up:
                        Animator.Play(_isArrowAttack ? "ATTACK_WEAPON_BACK" : "ATTACK_BACK");
                        Sprite.flipX = false;
                        break;
                    case EMoveDir.Down:
                        Animator.Play(_isArrowAttack ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
                        Sprite.flipX = false;
                        break;
                    case EMoveDir.Left:
                        Animator.Play(_isArrowAttack ? "ATTACK_WEAPON_SIDE" : "ATTACK_SIDE");
                        Sprite.flipX = true;
                        break;
                    case EMoveDir.Right:  // None에 대해서는 Right와 동일한 처리
                        Animator.Play(_isArrowAttack ? "ATTACK_WEAPON_SIDE" : "ATTACK_SIDE");
                        Sprite.flipX = false;
                        break;
                }
                break;
            case EState.Dead:
                break;
        }
    }

    protected override void OnIdle()
    {
        // 이동 상태로 갈 지 확인
        if (curMoveDir != EMoveDir.None)
        {
            State = EState.Move;
            return;
        }
        
        // 스킬 상태로 갈 지 확인
        if (Input.GetKeyDown(KeyCode.Space))
        {
            State = EState.Skill;
            //_skillRoutine = StartCoroutine(nameof(AttackRoutine));
            _skillRoutine = StartCoroutine(nameof(ShootArrowRoutine));
        }
    }

    private IEnumerator AttackRoutine()
    {
        // 피격 판정
        GameObject target = Managers.Object.Find(GetFrontCell());
        if (target)
        {
            Controller controller = target.GetComponent<Controller>();
            if (controller)
            {
                controller.OnDamaged();
            }
        }
        
        // 쿨타임
        _isArrowAttack = false;
        yield return new WaitForSeconds(0.5f);
        State = EState.Idle;
        _skillRoutine = null;
    }

    private IEnumerator ShootArrowRoutine()
    {
        GameObject arrow = Managers.Resource.Instantiate("Pawn/Arrow");
        ArrowController controller = arrow.GetComponent<ArrowController>();
        controller.CurMoveDir = prevMoveDir;
        controller.OwnerCell = OwnerCell;
        
        // 쿨타임
        _isArrowAttack = true;
        yield return new WaitForSeconds(0.5f);
        State = EState.Idle;
        _skillRoutine = null;
    }

    public override void OnDamaged()
    {
        Debug.Log("Player Hit!");
    }
}
