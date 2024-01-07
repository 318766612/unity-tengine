﻿using System;
using System.IO;
using TEngine.Runtime;
using UnityEditor;
using UnityEngine;

namespace TEngine.Editor
{
    /// <summary>
    /// 打包流程插入（Builder流程插入处理器）
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class TEngineBuilderInjectorAttribute : System.Attribute
    {
        private int _moment = -1;
        public int Moment
        {
            get => _moment;
            set => _moment = value;
        }

        /// <summary>
        /// 插入Builder流程
        /// </summary>
        /// <param name="monments"></param>
        public TEngineBuilderInjectorAttribute(params BuilderInjectorMoment[] monments)
        {
            if (monments == null || monments.Length == 0)
                return;

            Moment = 0;
            foreach (var builderInjectorMoment in monments)
            {
                Moment += 1 << (int)builderInjectorMoment;

            }
        }

        public bool IsInMoment(BuilderInjectorMoment moment)
        {
            return ((1 << (int)moment) & Moment) > 0;
        }
    }

    public enum BuilderInjectorMoment
    {
        /// <summary>
        /// 收集AB包之前
        /// </summary>
        BeforeCollect_AssetBundle,
        /// <summary>
        /// 打AB包之前
        /// </summary>
        BeforeBuild_AssetBundle,
        /// <summary>
        /// 打AB包之后
        /// </summary>
        AfterBuild_AssetBundle,
        /// <summary>
        /// 打APK之前
        /// </summary>
        BeforeBuild_Apk,
        /// <summary>
        /// 打APK之后
        /// </summary>
        AfterBuild_Apk,
        /// <summary>
        /// 打APK之后
        /// </summary>
        AfterBuild_FirstZip,
        /// <summary>
        /// 打APK之前
        /// </summary>
        BeforeBuild_FirstZip
    }

    public static class CusInjectorDemoEditor
    {
	    [TEngineBuilderInjector(BuilderInjectorMoment.BeforeCollect_AssetBundle)]
	    public static void CopyVersion()
	    {
		    var innerVersionFile = UnityEngine.Application.streamingAssetsPath + "/TEngine/version.json";

		    string path = FileSystem.ResourceRoot + "/" + GameConfig.CONFIG;
		    if (System.IO.File.Exists(innerVersionFile))
		    {
			    FileUtil.DeleteFileOrDirectory(innerVersionFile);
			    FileUtil.CopyFileOrDirectory(path, innerVersionFile);
		    }
		    else
		    {
			    FileUtil.CopyFileOrDirectory(path, innerVersionFile);
		    }

		    Log.Debug("复制版本信息成功");
	    }

        [TEngineBuilderInjector(BuilderInjectorMoment.AfterBuild_AssetBundle)]
        public static void GenMd5List()
        {
            UnityEngine.Debug.Log($"productName: {PlayerSettings.productName}");
            UnityEngine.Debug.Log($"version:{GameConfig.Instance.GameBundleVersion}");
            string versionStr = GameConfig.Instance.GameBundleVersion.Replace(".", "");
            long versionLong = long.Parse(versionStr);
            UnityEngine.Debug.Log($"versionStr:{versionStr}");
            UnityEngine.Debug.LogError("BuilderInjectorMoment.AfterBuild_AssetBundle");
            TEngineCore.Editor.TEngineEditorUtil.GenMd5List();
        }
    }
}
