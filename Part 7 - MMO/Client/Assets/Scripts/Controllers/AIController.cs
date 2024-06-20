using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class AIController : Controller
{
    protected override void Awake()
    {
        base.Awake();
        
        State = EState.Idle;
        CurMoveDir = EMoveDir.None;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }
    
    // TODO: 추후 키보드 입력이 아닌 AI에 의해 방향을 결정하도록 코드 변경
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
}
