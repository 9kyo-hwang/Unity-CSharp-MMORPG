using System;
using UnityEngine;
using UnityEngine.Serialization;
using static Define;

public class Controller : MonoBehaviour
{
    [SerializeField] protected float movementSpeed = 5.0f;
    protected Animator Animator;
    protected SpriteRenderer Sprite;
    public Vector3Int Position { get; set; } = Vector3Int.zero;
    
    [SerializeField] protected EState state = EState.Idle;
    public virtual EState State
    {
        get => state;
        set
        {
            if (state == value) return;
            state = value;
            SetAnimation();
        }
    }
    
    [SerializeField] protected EMoveDir curMoveDir = EMoveDir.Down;
    [SerializeField] protected EMoveDir prevMoveDir = EMoveDir.Down;
    public EMoveDir CurMoveDir
    {
        get => curMoveDir;
        set
        {
            if (curMoveDir == value) return;

            curMoveDir = value;
            if (value != EMoveDir.None) prevMoveDir = value;
            SetAnimation();
        }
    }

    public Vector3Int GetFrontPosition()
    {
        Vector3Int position = Position;
        switch(prevMoveDir)
        {
            case EMoveDir.Up:
                position += Vector3Int.up;
                break;
            case EMoveDir.Down:
                position += Vector3Int.down;
                break;
            case EMoveDir.Left:
                position += Vector3Int.left;
                break;
            case EMoveDir.Right:
                position += Vector3Int.right;
                break;
        }

        return position;
    }

    private readonly Vector3 _cellOffset = new Vector3(0.5f, 0, 0);

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();
        Sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        // 초기 위치 세팅
        transform.position = Managers.Map.ActiveMap.CellToWorld(Position) + _cellOffset;
    }
    
    protected virtual void Update()
    {
        // 조건이 복잡해짐에 따라, State 별로 Update에서 수행해야 할 함수를 구분해서 적용
        switch (State)
        {
            case EState.Idle:
                OnIdle();
                break;
            case EState.Move:
                OnMove();
                break;
            case EState.Skill:
                break;
            case EState.Dead:
                break;
        }
    }

    protected virtual void LateUpdate()
    {

    }

    protected virtual void SetAnimation()
    {
        // MoveDir에서 IDLE 애니메이션을 처리할 땐, 애니메이션을 Play한 후 실제 방향을 변경함
        // 여기서는 그 방식이 힘들어, 이전 방향 정보를 저장하는 변수 추가
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
                        Animator.Play("ATTACK_BACK");
                        Sprite.flipX = false;
                        break;
                    case EMoveDir.Down:
                        Animator.Play("ATTACK_FRONT");
                        Sprite.flipX = false;
                        break;
                    case EMoveDir.Left:
                        Animator.Play("ATTACK_SIDE");
                        Sprite.flipX = true;
                        break;
                    case EMoveDir.Right:  // None에 대해서는 Right와 동일한 처리
                        Animator.Play("ATTACK_SIDE");
                        Sprite.flipX = false;
                        break;
                }
                break;
            case EState.Dead:
                break;
        }
    }

    // 이동 가능할 때 실제 좌표 조정
    protected virtual void OnIdle()
    {

    }

    // 캐릭터가 서서히 움직이는 로직 처리
    protected virtual void OnMove()
    {
        // 클라이언트에서 캐릭터가 서서히 움직이는 모습 구현
        Vector3 destination = Managers.Map.ActiveMap.CellToWorld(Position) + _cellOffset;
        Vector3 moveDir = destination - transform.position;
        
        // 도착 여부 체크
        // 벡터 크기 == 거리. 즉 거의 다 도달해서 남은 거리가 매우 작다면
        if (moveDir.magnitude < movementSpeed * Time.deltaTime)
        {
            transform.position = destination;
            MoveToDestination();
        }
        else
        {
            transform.position += moveDir.normalized * movementSpeed * Time.deltaTime;
            State = EState.Move;
        }
    }

    protected virtual void MoveToDestination()
    {
        // 여기서 이동 방향이 None이라는 것은 실제로 상태가 Idle로 바껴야 함을 의미함
        if (curMoveDir == EMoveDir.None)
        {
            State = EState.Idle;
            return;
        }
        
        // 여기는 Move 상태로 유지시켜 애니메이션을 출력해야 함
        Vector3Int destination = Position;
        switch (curMoveDir)
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
        
        // OnMove()에서 호출하기 때문에, 여기에 도달했다는 것은 Move 상태가 유지됨을 보장받음
        if (Managers.Map.CanGo(destination) && !Managers.Object.Find(destination))
        {
            Position = destination;
        }
    }

    protected virtual void OnSkill()
    {
        
    }

    protected virtual void OnDead()
    {
        
    }

    public virtual void OnDamaged()
    {
        
    }
}
