using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectManager
{
    // 추후 객체는 ID를 가지고 있을 것이므로 Dictionary를 사용하게 될 것.
    // 다만 지금은 ID가 없으므로 임시로 List<>에 저장
    // private Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
    private List<GameObject> _objects = new List<GameObject>();

    public int Count => _objects.Count;
    
    public void Add(GameObject item)
    {
        _objects.Add(item);
    }

    public void Clear()
    {
        _objects.Clear();
    }
    
    public void Remove(GameObject item)
    {
        _objects.Remove(item);
    }

    public GameObject Find(Vector3Int position)
    {
        // Controller를 소유하고 있는 객체들은 position 정보를 들고 있음.
        // 하지만 protected로 되어있기 때문에, 이에 대한 Getter가 필요함
        foreach (GameObject item in _objects)
        {
            Controller controller = item.GetComponent<Controller>();
            if (!controller) continue;

            if (controller.OwnerCell == position) return item;
        }

        return null;
    }

    // GameObject를 인자로 넘겨주면 결과를 bool로 반환하는 델리게이트가 인자
    public GameObject Find(Func<GameObject, bool> condition)
    {
        // 넘겨준 condition에 부합하는 첫 번째 오브젝트
        foreach (GameObject item in _objects)
        {
            if (condition.Invoke(item)) return item;
        }

        return null;
    }
}
