using UnityEngine;
using System.Collections.Generic;
using QFramework.Example;

[CreateAssetMenu(
    fileName = "HintDatabase", 
    menuName = "Data/Hint Database", 
    order = 1)]
public class HintDatabase : ScriptableObject
{
    [System.Serializable]
    public class HintEntry
    {
        [Tooltip("唯一的关键词，用来索引提示文本")]
        public string key;

        [Tooltip("与关键词对应的提示文本")]
        [TextArea(2,5)]
        public string hintText_CN;
        [TextArea(2,5)]
        public string hintText_EN;
    }

    [Header("在这里添加所有的（关键词＋提示）对")]
    public List<HintEntry> entries = new List<HintEntry>();

    // 延迟构建的字典，供运行时快速查找
    private Dictionary<string, string> _hintDict_CN;
    private Dictionary<string, string> _hintDict_EN;
    
    /// <summary>
    /// 初始化字典缓存
    /// </summary>
    private void InitializeDictionary()
    {
        if (_hintDict_CN != null && _hintDict_EN != null) return;
        
        _hintDict_CN = new Dictionary<string, string>();
        _hintDict_EN = new Dictionary<string, string>();
        Debug.Log($"开始构建HintDatabase字典，条目数量：{entries.Count}");
        
        foreach (var entry in entries)
        {
            if (string.IsNullOrEmpty(entry.key))
            {
                Debug.LogWarning("发现空的key，跳过该条目");
                continue;
            }
            
            if (!_hintDict_CN.ContainsKey(entry.key))
            {
                _hintDict_CN.Add(entry.key, entry.hintText_CN);
                _hintDict_EN.Add(entry.key, entry.hintText_EN);
            }
            else
            {
                Debug.LogWarning($"HintDatabase 中有重复的 key：{entry.key}，跳过重复项");
            }
        }
        
        Debug.Log($"HintDatabase字典构建完成，实际加载 {_hintDict_CN.Count} 个条目");
    }

    /// <summary>
    /// 根据 key 拿到提示文本，找不到返回空串
    /// </summary>
    public string GetHint(string key, GlobalEnums.Language language)
    {
        // 确保字典已初始化
        InitializeDictionary();
        
        if (language == GlobalEnums.Language.English)
        {
            if (_hintDict_EN.TryGetValue(key, out string hintText))
            {
                Debug.Log($"已找到提示文本：{hintText}");
                return hintText;
            }
            else
            {
                Debug.Log($"[{key}]未找到提示文本");
                return "???";
            }
        }
        else if (language == GlobalEnums.Language.Chinese)
        {
            if (_hintDict_CN.TryGetValue(key, out string hintText))
            {
                Debug.Log($"已找到提示文本：{hintText}");
                return hintText;
            }
            else
            {
                Debug.Log($"[{key}]未找到提示文本");
                return "???";
            }
        }
        else
        {
            Debug.Log($"[{key}]未找到提示文本");
            return "???";
        }
    }
    
    /// <summary>
    /// 强制重新构建字典缓存（当entries在运行时被修改后调用）
    /// </summary>
    public void RefreshDictionary()
    {
        _hintDict_CN = null;
        _hintDict_EN = null;
        InitializeDictionary();
        Debug.Log("HintDatabase字典已强制刷新");
    }
    
    /// <summary>
    /// 检查是否包含指定的key
    /// </summary>
    public bool ContainsKey(string key)
    {
        InitializeDictionary();
        return _hintDict_CN.ContainsKey(key) || _hintDict_EN.ContainsKey(key);
    }
    
    /// <summary>
    /// 获取所有可用的keys
    /// </summary>
    public string[] GetAllKeys()
    {
        InitializeDictionary();
        string[] keys = new string[_hintDict_CN.Keys.Count];
        _hintDict_CN.Keys.CopyTo(keys, 0);
        _hintDict_EN.Keys.CopyTo(keys, 0);
        return keys;
    }
    
    /// <summary>
    /// 获取字典中的条目数量
    /// </summary>
    public int GetDictionaryCount()
    {
        InitializeDictionary();
        return _hintDict_CN.Count;
    }
}
