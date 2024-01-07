/*
 * created : 2023-01-11      author : holin
 * function description : Complementary AOT type
 */
using Newtonsoft.Json.Linq;
using TEngine.Runtime.Entity;
using TEngine.Runtime;
using UnityEngine;
using System;

public class RefTypes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MyAOTRefs();
    }

    //Complementary AOT type
    public void MyAOTRefs()
    {
        Debug.Log("Complementary AOT type");
        //new JObject().Value<int>();
        //GameEventMgr.Instance.Init

        //GameEvent.AddEventListener(0, OnCameraInit);
        GameEvent.AddEventListener<int>(0, T_Int);
        //GameEvent.AddEventListener<float>(1, CameraFieldOfView);
        //GameEvent.AddEventListener<IEntity, float, object>(3, OnShowEntitySuccess);

        //GameEvent.Send(0);
        //GameEvent.Send<int>(0, 0);
        //GameEvent.Send<float>(0, 0);

        // 加载 EmptyAnimatorController
        //var emptyController = Resources.Load<RuntimeAnimatorController>("Anim/Empty");
        // 获取 Animator 组件
        //var animator = gameObject.AddComponent<Animator>();
        // 将 EmptyAnimatorController 指定为运行时控制器
        //animator.runtimeAnimatorController = emptyController;


    }

    private void T_Int(int obj)
    {

    }
}
