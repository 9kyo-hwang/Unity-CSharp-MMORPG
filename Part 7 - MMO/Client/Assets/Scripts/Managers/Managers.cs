using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
	private static Managers _instance; // 유일성이 보장된다
	private static Managers Instance { get { Init(); return _instance; } } // 유일한 매니저를 갖고온다

	#region Contents

	private MapManager _map = new MapManager();
	private ObjectManager _object = new ObjectManager();
	public static MapManager Map => Instance._map;
	public static ObjectManager Object => Instance._object;
	
	#endregion

	#region Core

	private DataManager _data = new DataManager();
	private PoolManager _pool = new PoolManager();
	private ResourceManager _resource = new ResourceManager();
	private SceneManagerEx _scene = new SceneManagerEx();
	private SoundManager _sound = new SoundManager();
	private UIManager _ui = new UIManager();

    public static DataManager Data => Instance._data;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    public static SoundManager Sound => Instance._sound;
    public static UIManager UI => Instance._ui;
    
    #endregion

    void Awake()
    {
	    Init();
    }

	void Start()
    {
        
	}

    void Update()
    {

    }

    private static void Init()
    {
	    if (_instance) return;
	    
	    GameObject go = GameObject.Find("@Managers");
	    if (!go)
	    {
		    go = new GameObject { name = "@Managers" };
		    go.AddComponent<Managers>();
	    }
	    DontDestroyOnLoad(go);
	    
	    _instance = go.GetComponent<Managers>();
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
