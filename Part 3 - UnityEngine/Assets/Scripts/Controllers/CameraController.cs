using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [FormerlySerializedAs("_mode")] [SerializeField]
    Define.CameraMode mode = Define.CameraMode.QuarterView;

    [FormerlySerializedAs("_delta")] [SerializeField]
    Vector3 delta = new Vector3(0.0f, 6.0f, -5.0f);

    [FormerlySerializedAs("_player")] [SerializeField]
    GameObject player = null;

    public void SetPlayer(GameObject player) { this.player = player; }

    void Start()
    {
        
    }

    void LateUpdate()
    {
        if (mode != Define.CameraMode.QuarterView) return;
        
        if (player.IsValid() == false)
        {
            return;
        }

        if (Physics.Raycast(player.transform.position, delta, out var hit, delta.magnitude, 1 << (int)Define.Layer.Block))
        {
            float dist = (hit.point - player.transform.position).magnitude * 0.8f;
            transform.position = player.transform.position + delta.normalized * dist;
        }
        else
        {
            transform.position = player.transform.position + delta;
            transform.LookAt(player.transform);
        }
    }

    public void SetQuarterView(Vector3 delta)
    {
        mode = Define.CameraMode.QuarterView;
        this.delta = delta;
    }
}
