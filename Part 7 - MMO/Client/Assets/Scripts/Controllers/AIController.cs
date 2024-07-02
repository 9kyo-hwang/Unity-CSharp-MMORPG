using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static Define;

public class AIController : Controller
{
    private Coroutine _patrolRoutine;
    private Coroutine _searchRoutine;
    private Coroutine _skillRoutine;
    
    [SerializeField] private Vector3Int _dstCell;
    [SerializeField] private GameObject _targetObject;
    [SerializeField] private float _searchRange = 10.0f;
    [SerializeField] private float _skillRange = 1.0f;
    [SerializeField] private bool _isArcher;

    public override EState State 
    { 
        get => state;
        set
        {
            base.State = value;
            if (_patrolRoutine != null)
            {
                StopCoroutine(_patrolRoutine);
                _patrolRoutine = null;
            }

            if (_searchRoutine != null)
            {
                StopCoroutine(_searchRoutine);
                _searchRoutine = null;
            }
        } 
    }

    protected override void Awake()
    {
        base.Awake();
        
        State = EState.Idle;
        CurMoveDir = EMoveDir.None;

        MovementSpeed = 3.0f;
        if (Random.Range(0, 2) == 0)
        {
            _isArcher = true;
            _skillRange = 10.0f;
        }
        else
        {
            _isArcher = false;
            _skillRange = 1.0f;
        }
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

    protected override void SetAnimation()
    {
        base.SetAnimation();
    }

    protected override void OnIdle()
    {
        base.OnIdle();

        _patrolRoutine ??= StartCoroutine(nameof(PatrolRoutine));
        _searchRoutine ??= StartCoroutine(nameof(SearchRoutine));
    }

    protected override void MoveToDestination()
    {
        // TODO: A*를 이용한 길찾기
        Vector3Int dstCell = _dstCell;
        if (_targetObject)
        {
            dstCell = _targetObject.GetComponent<Controller>().OwnerCell;

            // 거리 추출
            Vector3Int dir2Dst = dstCell - OwnerCell;
            if (dir2Dst.magnitude <= _skillRange && (dir2Dst.x == 0 || dir2Dst.y == 0))
            {
                CurMoveDir = GetMoveDirFrom(dir2Dst);
                State = EState.Skill;
                _skillRoutine = _isArcher
                    ? StartCoroutine(nameof(ShootArrowRoutine))
                    : StartCoroutine(nameof(AttackRoutine));
                return;
            }
        }

        List<Vector3Int> path = Managers.Map.FindPath(OwnerCell, dstCell, true);
        if (path.Count < 2 || (_targetObject && path.Count > 20))  // 길을 못찾았거나 캐릭터가 찰나에 범위를 벗어난 경우
        {
            _targetObject = null;
            State = EState.Idle;
            return;
        }

        Vector3Int nextCell = path[1];  // 0: 현재 위치
        Vector3Int dir2Next = nextCell - OwnerCell;
        CurMoveDir = GetMoveDirFrom(dir2Next);
        
        if (Managers.Map.CanGo(nextCell) && !Managers.Object.Find(nextCell))
        {
            OwnerCell = nextCell;
        }
        else  // 갈 수 없는 위치라면 즉시 Idle로 change
        {
            State = EState.Idle;
        }
    }

    public override void OnDamaged()
    {
        base.OnDamaged();
        
        // Effect Play
        GameObject effect = Managers.Resource.Instantiate("Effect/DeathEffect");
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        Destroy(effect, 0.5f);  // 0.5초 후 Destroy
                
        // Target Object Remove
        Managers.Object.Remove(gameObject);
        Managers.Resource.Destroy(gameObject);
    }

    private IEnumerator PatrolRoutine()
    {
        int cooldown = Random.Range(1, 4);
        yield return new WaitForSeconds(cooldown);

        // 주변 위치로 갈 수 있는지 10번 try
        for (int i = 0; i < 10; ++i)
        {
            int x = Random.Range(-5, 6);
            int y = Random.Range(-5, 6);
            Vector3Int dst = OwnerCell + new Vector3Int(x, y, 0);

            if (Managers.Map.CanGo(dst) && !Managers.Object.Find(dst))
            {
                // AI는 한 칸씩 움직이지 않고 목적지까지 한 번에 이동함. 따라서 이를 저장할 변수 필요
                _dstCell = dst;
                State = EState.Move;
                yield break;  // Coroutine을 완전히 종료하기 위해
            }
        }

        State = EState.Idle;
    }

    private IEnumerator SearchRoutine()
    {
        // 매 초마다 Search 알고리즘을 수행하는 것이 타당
        while (true)
        {
            yield return new WaitForSeconds(1f);
            
            // Search Target Check
            if (_targetObject)
            {
                continue;
            }
            
            // ObjectManager에 탐색 조건을 매개변수로 넘겨주는 새로운 시그니처의 Find 함수 추가
            _targetObject = Managers.Object.Find(target =>
            {
                PlayerController controller = target.GetComponent<PlayerController>();
                if (!controller)
                {
                    return false;
                }

                // 탐색 범위 내플레이어인 지 확인
                Vector3Int dir = controller.OwnerCell - OwnerCell;
                if (dir.magnitude > _searchRange)
                {
                    return false;
                }

                return true;
            });
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
        yield return new WaitForSeconds(0.5f);
        State = EState.Move;
        _skillRoutine = null;
    }

    private IEnumerator ShootArrowRoutine()
    {
        GameObject arrow = Managers.Resource.Instantiate("Pawn/Arrow");
        ArrowController controller = arrow.GetComponent<ArrowController>();
        controller.CurMoveDir = prevMoveDir;
        controller.OwnerCell = OwnerCell;

        // 쿨타임
        yield return new WaitForSeconds(0.5f);
        State = EState.Move;
        _skillRoutine = null;
    }
}
