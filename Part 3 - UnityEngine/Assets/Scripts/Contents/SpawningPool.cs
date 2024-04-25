using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class SpawningPool : MonoBehaviour
{
    [FormerlySerializedAs("_monsterCount")] [SerializeField]
    int monsterCount = 0;
    int _reserveCount = 0;

    [FormerlySerializedAs("_keepMonsterCount")] [SerializeField]
    int keepMonsterCount = 0;

    [FormerlySerializedAs("_spawnPos")] [SerializeField]
    Vector3 spawnPos;
    [FormerlySerializedAs("_spawnRadius")] [SerializeField]
    float spawnRadius = 15.0f;
    [FormerlySerializedAs("_spawnTime")] [SerializeField]
    float spawnTime = 5.0f;

    public void AddMonsterCount(int value) { monsterCount += value; }
    public void SetKeepMonsterCount(int count) { keepMonsterCount = count; }

    void Start()
    {
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    void Update()
    {
        while (_reserveCount + monsterCount < keepMonsterCount)
        {
            StartCoroutine(nameof(ReserveSpawn));
        }
    }

    IEnumerator ReserveSpawn()
    {
        _reserveCount++;
        yield return new WaitForSeconds(Random.Range(0, spawnTime));
        GameObject obj = Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");
        NavMeshAgent nma = obj.GetOrAddComponent<NavMeshAgent>();

        Vector3 randPos;
        while (true)
        {
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, spawnRadius);
			randDir.y = 0;
			randPos = spawnPos + randDir;

            // 갈 수 있나
            NavMeshPath path = new NavMeshPath();
            if (nma.CalculatePath(randPos, path))
                break;
		}

        obj.transform.position = randPos;
        _reserveCount--;
    }
}
