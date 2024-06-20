using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

	protected virtual void Awake()
	{
		Object obj = FindObjectOfType(typeof(EventSystem));
		if (!obj)
		{
			Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
		}
	}

	protected virtual void Init()
    {
	    
    }

    public abstract void Clear();
}
