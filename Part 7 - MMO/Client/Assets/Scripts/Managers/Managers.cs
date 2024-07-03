using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance; // 유일성이 보장된다
    private static Managers Instance { get { Init(); return _instance; } } // 유일한 매니저를 갖고온다

    #region Contents

    private MapManager _map = new();
    private ObjectManager _obj = new();
    private NetworkManager _network = new();

    public static MapManager Map => Instance._map;
    public static ObjectManager Object => Instance._obj;
    public static NetworkManager Network => Instance._network;
	#endregion

	#region Core

    private DataManager _data = new();
    private PoolManager _pool = new();
    private ResourceManager _resource = new();
    private SceneManagerEx _scene = new();
    private SoundManager _sound = new();
    private UIManager _ui = new();

    public static DataManager Data => Instance._data;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    public static SoundManager Sound => Instance._sound;
    public static UIManager UI => Instance._ui;

    #endregion

	void Start()
    {
        Init();
	}

    void Update()
    {
        _network.Update();  // NetworkManager는 Init과 Update가 반드시 이루어져야 함
    }

    private static void Init()
    {
        if (_instance != null) return;
        
        GameObject managers = GameObject.Find("@Managers");
        if (managers == null)
        {
            managers = new GameObject { name = "@Managers" };
            managers.AddComponent<Managers>();
        }

        DontDestroyOnLoad(managers);
        _instance = managers.GetComponent<Managers>();

        _instance._network.Init();  // NetworkManager는 Init과 Update가 반드시 이루어져야 함
        _instance._data.Init();
        _instance._pool.Init();
        _instance._sound.Init();
    }

    public static void Clear()
    {
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
    }
}
