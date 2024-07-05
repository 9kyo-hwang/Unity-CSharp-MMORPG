using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ObjectManager
{
	public LocalPlayerController LocalPlayer { get; set; }
	private Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

    public void Add(PlayerInfo info, bool isLocalPlayer = false)
    {
        if (isLocalPlayer)
        {
            GameObject player = Managers.Resource.Instantiate("Creature/LocalPlayer");
            player.name = info.Name;
            _objects.Add(info.PlayerId, player);

            LocalPlayer = player.GetComponent<LocalPlayerController>();
            LocalPlayer.Id = info.PlayerId;
            LocalPlayer.CellPos = new Vector3Int(info.PosX, info.PosY, 0);
        }
        else
        {
            GameObject player = Managers.Resource.Instantiate("Creature/Player");
            player.name = info.Name;
            _objects.Add(info.PlayerId, player);

            PlayerController another = player.GetComponent<PlayerController>();
            another.Id = info.PlayerId;
            another.CellPos = new Vector3Int(info.PosX, info.PosY, 0);
        }
    }

	public void Add(int id, GameObject obj)
	{
		_objects.Add(id, obj);
	}

    public void Remove()
    {
        if (LocalPlayer == null) return;

        Remove(LocalPlayer.Id);
        LocalPlayer = null;
    }

	public void Remove(int id)
	{
		_objects.Remove(id);
	}

	public GameObject Find(Vector3Int cellPos)
    {
        return (from obj in _objects.Values
            let cc = obj.GetComponent<CreatureController>()
            where cc != null
            where cc.CellPos == cellPos
            select obj).FirstOrDefault();
    }

	public GameObject Find(Func<GameObject, bool> condition)
    {
        return _objects.Values.FirstOrDefault(condition.Invoke);
    }

	public void Clear()
	{
		_objects.Clear();
	}
}
