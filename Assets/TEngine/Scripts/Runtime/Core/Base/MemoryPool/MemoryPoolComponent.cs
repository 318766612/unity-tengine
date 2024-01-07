﻿using UnityEngine;

namespace TEngine.Runtime
{
    /// <summary>
    /// 内存强制检查类型。
    /// </summary>
    public enum MemoryStrictCheckType : byte
    {
        /// <summary>
        /// 总是启用。
        /// </summary>
        AlwaysEnable = 0,

        /// <summary>
        /// 仅在开发模式时启用。
        /// </summary>
        OnlyEnableWhenDevelopment,

        /// <summary>
        /// 仅在编辑器中启用。
        /// </summary>
        OnlyEnableInEditor,

        /// <summary>
        /// 总是禁用。
        /// </summary>
        AlwaysDisable,
    }

    /// <summary>
    /// 基础组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("TEngine/MemoryPool")]
    public sealed class MemoryPoolComponent : UnitySingleton<MemoryPoolComponent>
    {
        [SerializeField]
        private MemoryStrictCheckType m_EnableStrictCheck = MemoryStrictCheckType.AlwaysEnable;

        /// <summary>
        /// 获取或设置是否开启强制检查。
        /// </summary>
        public bool EnableStrictCheck
        {
            get
            {
                return MemoryPool.EnableStrictCheck;
            }
            set
            {
                MemoryPool.EnableStrictCheck = value;
                if (value)
                {
                    Log.Info("Strict checking is enabled for the Memory Pool. It will drastically affect the performance.");
                }
            }
        }

        private void Start()
        {
            switch (m_EnableStrictCheck)
            {
                case MemoryStrictCheckType.AlwaysEnable:
                    EnableStrictCheck = true;
                    break;

                case MemoryStrictCheckType.OnlyEnableWhenDevelopment:
                    EnableStrictCheck = Debug.isDebugBuild;
                    break;

                case MemoryStrictCheckType.OnlyEnableInEditor:
                    EnableStrictCheck = Application.isEditor;
                    break;

                default:
                    EnableStrictCheck = false;
                    break;
            }
        }
    }
}