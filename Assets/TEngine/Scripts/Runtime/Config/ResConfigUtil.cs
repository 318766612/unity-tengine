﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEngine.Runtime
{
    /// <summary>
    /// 过滤配置
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <param name="val"></param>
    /// <returns></returns>
    public delegate bool FilterResBin<TType>(TType val);

    /// <summary>
    /// 计算拼接Key
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="val"></param>
    /// <returns></returns>
    public delegate TKey ConvertDictionaryKey<TKey, TValue>(TValue val);

    public class ResConfigUtil
    {
        private static StringBuilder _mStrBuilder = new StringBuilder();
        private static readonly string MSplit = "_";

        #region 读取接口

        public static List<T> ReadConfigListRes<T>(string fileName = "")
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = typeof(T).Name;
            }

            string resPath = string.Format("Config/{0}.json", fileName);
            TextAsset jsonStr = TResources.Load<TextAsset>(resPath);
            if (jsonStr == null)
            {
                TLogger.LogError("Config {0} Read Json Error", fileName);
                return null;
            }

            List<T> list = new List<T>();
            var jsonData = JsonHelper.Instance.Deserialize<List<T>>(jsonStr.text);
            list = jsonData;
            return list;
        }

        public static List<T> ReadResBinDict<K, T>(Dictionary<K, T> dic, ConvertDictionaryKey<K, T> convKey,
            string fileName = "")
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = typeof(T).Name;
            }

            string resPath = string.Format("Config/{0}.json", fileName);
            TextAsset jsonStr = TResources.Load<TextAsset>(resPath);
            if (jsonStr == null)
            {
                TLogger.LogError("Config {0} Read Json Error", fileName);
                return null;
            }

            var jsonData = JsonHelper.Instance.Deserialize<List<T>>(jsonStr.text);

            var etr = jsonData.GetEnumerator();

            if (dic == null)
            {
                dic = new Dictionary<K, T>();
            }
            else
            {
                dic.Clear();
            }

            while (etr.MoveNext())
            {
                var key = convKey(etr.Current);
                {
                    if (dic.ContainsKey(key))
                    {
                        TLogger.LogError("Config {0} Load Error, Repeat config {1}", typeof(T).ToString(),
                            key.ToString());
                    }

                    dic.Add(key, etr.Current);
                }
            }

            etr.Dispose();

            return jsonData;
        }

        public static List<T> ReadResBinDict<K, T>(Dictionary<K, List<T>> dict, ConvertDictionaryKey<K, T> convKey,
            string fileName = "")
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = typeof(T).Name;
            }

            string resPath = string.Format("Config/{0}.json", fileName);
            TextAsset jsonStr = TResources.Load<TextAsset>(resPath);
            if (jsonStr == null)
            {
                TLogger.LogError("Config {0} Read Json Error", fileName);
                return null;
            }

            var jsonData = JsonHelper.Instance.Deserialize<List<T>>(jsonStr.text);

            var etr = jsonData.GetEnumerator();
            if (dict == null)
            {
                dict = new Dictionary<K, List<T>>();
            }
            else
            {
                dict.Clear();
            }

            while (etr.MoveNext())
            {
                var data = etr.Current;
                var key = convKey(data);
                List<T> listItem;
                if (!dict.TryGetValue(key, out listItem))
                {
                    listItem = new List<T>();
                    dict.Add(key, listItem);
                }

                listItem.Add(data);
            }

            etr.Dispose();

            return jsonData;
        }

        #endregion

        public static UInt64 Make64Key(uint key1, uint key2)
        {
            return (((UInt64)key1) << 32) | key2;
        }

        public static string MakeStringKey(uint key1, uint key2, uint key3)
        {
            _mStrBuilder.Length = 0;
            _mStrBuilder.Append(key1);
            _mStrBuilder.Append(MSplit);
            _mStrBuilder.Append(key2);
            _mStrBuilder.Append(MSplit);
            _mStrBuilder.Append(key3);
            return _mStrBuilder.ToString();
        }

        public static string MakeStringKey(string key1, uint key2)
        {
            _mStrBuilder.Length = 0;
            _mStrBuilder.Append(key1);
            _mStrBuilder.Append(MSplit);
            _mStrBuilder.Append(key2);
            return _mStrBuilder.ToString();
        }

        public static string MakeStringKey(string key1, string key2)
        {
            _mStrBuilder.Length = 0;
            _mStrBuilder.Append(key1);
            _mStrBuilder.Append(MSplit);
            _mStrBuilder.Append(key2);
            return _mStrBuilder.ToString();
        }
    }
}
/*
 *
===》 example 《===
public class BufferMgr : Singleton<BufferMgr>
{
    private Dictionary<string, BuffConfig> m_dictBaseConfig = new Dictionary<string, BuffConfig>();

    public BufferMgr()
    {
        m_dictBaseConfig = ResConfigUtil.ReadConfigRes<BuffConfig>();
    }

    public Dictionary<string, BuffConfig> GetBuffConfig()
    {
        return m_dictBaseConfig;
    }
}
 *
 */