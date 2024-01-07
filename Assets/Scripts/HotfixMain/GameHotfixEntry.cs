using TEngine.Runtime;
using TEngine.Runtime.UIModule;


public class GameHotfixEntry
{
    public static void Start()
    {
        Log.Debug("HotFix.GameHotfixEntry");

        Log.Debug("=======看到此条日志代表你成功运行了示例项目的热更新代码=======");
        MonoUtility.AddUpdateListener(Update);
        //TResources.Load<GameObject>("Test/Cube.prefab");

        UISys.Mgr.ShowWindow<V_ViewType>();
        UISys.Mgr.ShowWindow<V_Content>();
    }

    private static void Update()
    {

    }
}


