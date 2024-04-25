using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
	private const int Mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

	Texture2D _attackIcon;
	Texture2D _handIcon;

	enum CursorType
	{
		None,
		Attack,
		Hand,
	}

	CursorType _cursorType = CursorType.None;

	void Start()
    {
		_attackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack");
		_handIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");
	}

    void Update()
    {
		if (Input.GetMouseButton(0))
			return;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (!Physics.Raycast(ray, out var hit, 100.0f, Mask)) return;
		
		if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
		{
			if (_cursorType == CursorType.Attack) return;
			
			Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 5, 0), CursorMode.Auto);
			_cursorType = CursorType.Attack;
		}
		else
		{
			if (_cursorType == CursorType.Hand) return;
			
			Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3, 0), CursorMode.Auto);
			_cursorType = CursorType.Hand;
		}
    }
}
