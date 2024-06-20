using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5.0f;
    private Animator _animator;
    
    private EMoveDir _moveDir = EMoveDir.None;
    private bool _isMoving = false;
    private Vector3Int _position = Vector3Int.zero;

    public EMoveDir MoveDir
    {
        get => _moveDir;
        set
        {
            if (_moveDir == value) return;

            switch (value)
            {
                case EMoveDir.None:  // 입력이 멈춘 경우, "갱신되기 전" 이동 방향을 가지고 IDLE 애니메이션 재생
                    switch (_moveDir)
                    {
                        case EMoveDir.Up:
                            _animator.Play("IDLE_BACK");
                            transform.localScale = new Vector3(1, 1, 1);
                            break;
                        case EMoveDir.Down:
                            _animator.Play("IDLE_FRONT");
                            transform.localScale = new Vector3(1, 1, 1);
                            break;
                        case EMoveDir.Left:
                            _animator.Play("IDLE_SIDE");
                            transform.localScale = new Vector3(-1, 1, 1);
                            break;
                        case EMoveDir.Right:
                        case EMoveDir.None:  // None에 대해서는 Right와 동일한 처리
                            _animator.Play("IDLE_SIDE");
                            transform.localScale = new Vector3(1, 1, 1);
                            break;
                    }
                    break;
                case EMoveDir.Up:
                    _animator.Play("WALK_BACK");
                    transform.localScale = new Vector3(1, 1, 1);
                    break;
                case EMoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    transform.localScale = new Vector3(1, 1, 1);
                    break;
                case EMoveDir.Left:
                    _animator.Play("WALK_SIDE");
                    transform.localScale = new Vector3(-1, 1, 1);
                    break;
                case EMoveDir.Right:
                    _animator.Play("WALK_SIDE");
                    transform.localScale = new Vector3(1, 1, 1);
                    break;
            }

            _moveDir = value;
        }
    }

    private readonly Vector3 _cellOffset = new Vector3(0.5f, 0, 0);

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        
        // 초기 위치 세팅
        transform.position = Managers.Map.ActiveMap.CellToWorld(_position) + _cellOffset;
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        SetMoveDir();
        SetPosition();
        Move();
    }

    private void LateUpdate()
    {
        // 2D라서 기본적으로 Z축은 -10 설정
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // 키보드 입력에 따른 방향 정의
    void SetMoveDir()
    {
        if (Input.GetKey(KeyCode.W))
        {
            MoveDir = EMoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            MoveDir = EMoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            MoveDir = EMoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveDir = EMoveDir.Right;
        }
        else
        {
            MoveDir = EMoveDir.None;
        }
    }

    // 이동 가능할 때 실제 좌표 조정
    void SetPosition()
    {
        // 움직이는 도중에는 애니메이션만 출력함
        if (_isMoving || _moveDir == EMoveDir.None)
        {
            return;
        }
        
        // 실제 플레이어 좌표는 한 번만 옮김

        Vector3Int destination = _position;
        switch (_moveDir)
        {
            case EMoveDir.Up:
                destination += Vector3Int.up;
                break;
            case EMoveDir.Down:
                destination += Vector3Int.down;
                break;
            case EMoveDir.Left:
                destination += Vector3Int.left;
                break;
            case EMoveDir.Right:
                destination += Vector3Int.right;
                break;
        }

        if (!Managers.Map.CanGo(destination))
        {
            return;
        }

        _position = destination;
        _isMoving = true;
    }

    // 캐릭터가 서서히 움직이는 로직 처리
    void Move()
    {
        if (!_isMoving)
        {
            return;
        }
        
        // 클라이언트에서 캐릭터가 서서히 움직이는 모습 구현
        Vector3 destination = Managers.Map.ActiveMap.CellToWorld(_position) + _cellOffset;
        Vector3 moveDir = destination - transform.position;
        
        // 도착 여부 체크
        // 벡터 크기 == 거리. 즉 거의 다 도달해서 남은 거리가 매우 작다면
        if (moveDir.magnitude < movementSpeed * Time.deltaTime)
        {
            transform.position = destination;
            _isMoving = false;
        }
        else
        {
            transform.position += moveDir.normalized * movementSpeed * Time.deltaTime;
            _isMoving = true;
        }
    }
}
