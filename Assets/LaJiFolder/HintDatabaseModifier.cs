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

        // �����޸�
        SaveHintDatabase();
    }

    private void LoadHintsFromJson(string path)
    {
        // �����еļ����߼����ֲ���
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
        // �����е����/�޸��߼����ֲ���
        if (string.IsNullOrEmpty(jsonEntry.�ʻ�))
        {
            Debug.LogWarning("Key cannot be empty.");
            return;
        }

        var existingEntry = hintDatabase.entries.Find(entry => entry.key == jsonEntry.�ʻ�);

        if (existingEntry != null)
        {
            existingEntry.hintText_CN = FormatHintText(jsonEntry, true);
            existingEntry.hintText_EN = FormatHintText(jsonEntry, false);
            Debug.Log($"Updated entry with key: {jsonEntry.�ʻ�}");
        }
        else
        {
            HintDatabase.HintEntry newEntry = new HintDatabase.HintEntry
            {
                key = jsonEntry.�ʻ�,
                hintText_CN = FormatHintText(jsonEntry, true),
                hintText_EN = FormatHintText(jsonEntry, false)
            };
            hintDatabase.entries.Add(newEntry);
            Debug.Log($"Added new entry with key: {jsonEntry.�ʻ�}");
        }
    }

    private void SaveHintDatabase()
    {
#if UNITY_EDITOR
        // ���Ϊ���޸�
        EditorUtility.SetDirty(hintDatabase);
        // ������Դ
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
        result += "ƴ����" + entry.ƴ�� + "\n";
        result += "Ӣ�ķ��룺" + entry.Ӣ�ķ��� + "\n";
        result += "����ע�ͣ�" + entry.����ע�� + "\n";
        result += "Ӣ��ע�ͣ�" + entry.Ӣ��ע��;
        return result;
    }

    [System.Serializable]
    public class HintEntryJson
    {
        public string �ʻ�;
        public string ƴ��;
        public string Ӣ�ķ���;
        public string ����ע��;
        public string Ӣ��ע��;
    }
}