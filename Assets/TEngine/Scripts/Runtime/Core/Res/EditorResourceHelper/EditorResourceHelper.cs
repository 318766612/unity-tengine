using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TEngine.Runtime
{
#if UNITY_EDITOR
    public class EditorResourceHelper : ResourceHelperBase
    {
        public override GameObject Load(string path)
        {
            Debug.Log("加载资源路径：" + path);
            UnityEngine.Object target = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            UnityEngine.Object temp = Instantiate(target);
            return temp as GameObject;
        }

        public override GameObject Load(string path, Transform parent)
        {
            var obj = Load(path);
            if (obj != null && parent != null)
            {
                obj.transform.SetParent(parent);
            }
            return obj;
        }

        public override T Load<T>(string path)
        {
            //Debug.Log("加载资源路径 T：" + path);
            T target = AssetDatabase.LoadAssetAtPath<T>(path);
            T temp = Instantiate(target);
            return temp;
        }

        public override void LoadAsync(string path, Action<GameObject> callBack)
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            callBack?.Invoke(GameObject.Instantiate(obj));
        }


        public override void LoadAsync<T>(string path, Action<T> callBack, bool withSubAsset = false)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<T>(path);
            callBack?.Invoke(obj as T);
        }
    }
#endif
}