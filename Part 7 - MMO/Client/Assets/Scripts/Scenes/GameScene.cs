using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();
        
        SceneType = Define.Scene.Game;
        Managers.Map.Load(1);
        
        // GameScene에 필요한 것들을 모두 로드해야 함
        // 물론 추후 서버에서 처리하도록 수정할 예정
        
        GameObject player = Managers.Resource.Instantiate("Pawn/Player");
        player.name = "Player";
        Managers.Object.Add(player);

        for (int i = 0; i < 5; ++i)
        {
            GameObject monster = Managers.Resource.Instantiate("Pawn/Monster");
            monster.name = $"Monster_{i:000}";
            
            // 랜덤 위치 스폰
            Vector3Int position = new Vector3Int
            {
                x = Random.Range(-20, 20),
                y = Random.Range(-10, 10)
            };

            monster.GetComponent<AIController>().Position = position;
            Managers.Object.Add(monster);
        }
    }

    protected override void Init()
    {
        base.Init();
        
        //Managers.UI.ShowSceneUI<UI_Inven>();
        //Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        //gameObject.GetOrAddComponent<CursorController>();

        //GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        //Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player);

        ////Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");
        //GameObject go = new GameObject { name = "SpawningPool" };
        //SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        //pool.SetKeepMonsterCount(2);
    }

    public override void Clear()
    {
        
    }
}
