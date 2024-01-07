using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HybridCLR;
using UnityEngine;

namespace TEngine.Runtime
{
    /// <summary>
    /// 流程加载器 - 代码初始化
    /// </summary>
    public class ProcedureCodeInit : ProcedureBase
    {

        public static List<string> AOTMetaAssemblyNames { get; } = new List<string>()
        {
            "mscorlib",
            "System",
            "System.Core"
        };

        public static List<string> HotAssemblyNames { get; } = new List<string>()
        {
            "HotfixCommon",
            "HotfixMain"
        };

        private static Dictionary<string, byte[]> _AssetDatas = new Dictionary<string, byte[]>();

        public static byte[] GetAssetData(string dllName)
        {
            return _AssetDatas[dllName];
        }

        /// <summary>
        /// 是否需要加载热更新DLL
        /// </summary>
        public bool NeedLoadDll => ResourceComponent.Instance.ResourceMode == ResourceMode.Updatable || ResourceComponent.Instance.ResourceMode == ResourceMode.UpdatableWhilePlaying;

        private IFsm<IProcedureManager> m_procedureOwner;

        protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);

            m_procedureOwner = procedureOwner;

            if (!NeedLoadDll)
            {
                ChangeState<ProcedureStartGame>(procedureOwner);
                return;
            }

            LoadMetadataForAOTAssemblies();
            LoadMetadataForHotAssemblies();

        }
        /// <summary>
        /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
        /// </summary>
        private static void LoadMetadataForAOTAssemblies()
        {
            // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
            // 我们在BuildProcessors里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

            // 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            // 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            foreach (var aotDllName in AOTMetaAssemblyNames)
            {
                string dllPath = GetPath(aotDllName);
                byte[] dllBytes;

#if UNITY_EDITOR
                dllBytes = System.IO.File.ReadAllBytes(dllPath);
#else
                TextAsset aotDll = TResources.Load<TextAsset>(dllPath);
                dllBytes = aotDll.bytes;
#endif
                if (dllBytes == null)
                {
                    Log.Error($"Load aot {aotDllName} dll failed.");
                    continue;
                }
                else
                {
                    _AssetDatas[aotDllName + ".dll.bytes"] = dllBytes;
                }

                // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
                Log.Info($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
            }
        }

        private void LoadMetadataForHotAssemblies()
        {
            foreach (var hotDllName in HotAssemblyNames)
            {
                Debug.Log(hotDllName);
                Assembly hotfixAssembly;
#if UNITY_EDITOR
                hotfixAssembly = System.AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == hotDllName);
#else
                string dllPath = GetPath(hotDllName);
                TextAsset hotDll = TResources.Load<TextAsset>(dllPath);
                if (hotDll == null)
                {
                    Log.Error($"Load hotfix {hotDllName} dll failed. ");
                    continue;
                }
                hotfixAssembly = System.Reflection.Assembly.Load(hotDll.bytes);
#endif
                if (hotfixAssembly == null)
                {
                    Log.Error($"Load hotfix {hotDllName} dll failed. ");
                    continue;
                }
                else
                {
                    Log.Info($"Load hotfix {hotDllName} dll success. ");
                    if (hotDllName == "HotfixMain")
                        StartHotfixEntry(hotfixAssembly);
                }
            }

        }

        //获取dll路径
        static string GetPath(string dllName)
        {
            string path;
#if UNITY_EDITOR
            path = string.Format("{0}/HotfixRes/dll/{1}.dll.bytes", Application.dataPath, dllName);
#else
            path = string.Format("{0}dll/{1}.dll.bytes", Constant.Setting.AssetRootPath, dllName);
#endif
            return path;
        }

        private void StartHotfixEntry(Assembly hotfixAssembly)
        {
            var hotfixEntry = hotfixAssembly.GetType("GameHotfixEntry");
            var start = hotfixEntry.GetMethod("Start");
            start?.Invoke(null, null);
            ChangeState<ProcedureStartGame>(m_procedureOwner);
        }
    }
}
