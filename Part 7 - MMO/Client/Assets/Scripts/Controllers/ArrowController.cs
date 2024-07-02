using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : Controller
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        
        switch (prevMoveDir)
        {
            case EMoveDir.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case EMoveDir.Down:
                transform.rotation = Quaternion.Euler(0, 0, -180);
                break;
            case EMoveDir.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case EMoveDir.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }

        State = EState.Move;
        MovementSpeed = 15f;
    }

    protected override void SetAnimation()
    {
        
    }

    protected override void MoveToDestination()
    {
        Vector3Int destination = OwnerCell;
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

        if (Managers.Map.CanGo(destination))
        {
            GameObject target = Managers.Object.Find(destination);
            if (!target)
            {
                OwnerCell = destination;
            }
            else
            {
                Controller controller = target.GetComponent<Controller>();
                if (controller)
                {
                    controller.OnDamaged();
                }
                
                // Arrow Object Remove
                Managers.Resource.Destroy(gameObject);
            }
        }
        else
        {
            Managers.Resource.Destroy(gameObject);
        }
    }
}
