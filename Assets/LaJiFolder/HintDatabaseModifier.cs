using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HintDatabaseModifier : MonoBehaviour
{
    public HintDatabase hintDatabase;
    public string jsonFilePath = "Assets/Resources/hints.json";

    private void Start()
    {
        if (hintDatabase == null)
        {
            Debug.LogError("HintDatabase not assigned!");
            return;
        }

        LoadHintsFromJson(jsonFilePath);

        // 保存修改
        SaveHintDatabase();
    }

    private void LoadHintsFromJson(string path)
    {
        // 你现有的加载逻辑保持不变
        if (!File.Exists(path))
        {
            Debug.LogError("JSON file not found at path: " + path);
            return;
        }

        string jsonContent = File.ReadAllText(path);
        var hintEntries = JsonConvert.DeserializeObject<List<HintEntryJson>>(jsonContent);

        if (hintEntries == null || hintEntries.Count == 0)
        {
            Debug.LogWarning("No entries found in the JSON file.");
            return;
        }

        foreach (var entry in hintEntries)
        {
            AddOrModifyEntry(entry);
        }

        hintDatabase.RefreshDictionary();
    }

    private void AddOrModifyEntry(HintEntryJson jsonEntry)
    {
        // 你现有的添加/修改逻辑保持不变
        if (string.IsNullOrEmpty(jsonEntry.词汇))
        {
            Debug.LogWarning("Key cannot be empty.");
            return;
        }

        var existingEntry = hintDatabase.entries.Find(entry => entry.key == jsonEntry.词汇);

        if (existingEntry != null)
        {
            existingEntry.hintText_CN = FormatHintText(jsonEntry, true);
            existingEntry.hintText_EN = FormatHintText(jsonEntry, false);
            Debug.Log($"Updated entry with key: {jsonEntry.词汇}");
        }
        else
        {
            HintDatabase.HintEntry newEntry = new HintDatabase.HintEntry
            {
                key = jsonEntry.词汇,
                hintText_CN = FormatHintText(jsonEntry, true),
                hintText_EN = FormatHintText(jsonEntry, false)
            };
            hintDatabase.entries.Add(newEntry);
            Debug.Log($"Added new entry with key: {jsonEntry.词汇}");
        }
    }

    private void SaveHintDatabase()
    {
#if UNITY_EDITOR
        // 标记为已修改
        EditorUtility.SetDirty(hintDatabase);
        // 保存资源
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("HintDatabase saved successfully!");
#else
        Debug.LogWarning("Saving ScriptableObject is only available in Editor mode.");
#endif
    }

    private string FormatHintText(HintEntryJson entry, bool isChinese)
    {
        string result = "";
        result += "拼音：" + entry.拼音 + "\n";
        result += "英文翻译：" + entry.英文翻译 + "\n";
        result += "中文注释：" + entry.中文注释 + "\n";
        result += "英文注释：" + entry.英文注释;
        return result;
    }

    [System.Serializable]
    public class HintEntryJson
    {
        public string 词汇;
        public string 拼音;
        public string 英文翻译;
        public string 中文注释;
        public string 英文注释;
    }
}