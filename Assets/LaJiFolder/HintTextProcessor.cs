using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

public class HintTextProcessor : EditorWindow
{
    private HintDatabase hintDatabase;
    private string inputText = "";
    private string outputText = "";
    private Vector2 inputScroll;
    private Vector2 outputScroll;
    private List<string> allKeys = new List<string>();

    [MenuItem("Tools/Hint Text Processor")]
    public static void ShowWindow()
    {
        GetWindow<HintTextProcessor>("Hint Text Processor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Hint Text Processor", EditorStyles.boldLabel);

        // 选择 ScriptableObject
        EditorGUI.BeginChangeCheck();
        hintDatabase = (HintDatabase)EditorGUILayout.ObjectField("Hint Database", hintDatabase, typeof(HintDatabase), false);
        if (EditorGUI.EndChangeCheck())
        {
            UpdateKeysList();
        }

        if (hintDatabase == null)
        {
            EditorGUILayout.HelpBox("请先选择一个 Hint Database", MessageType.Info);
            return;
        }

        // 显示所有可用的key
        EditorGUILayout.Space();
        GUILayout.Label($"可用关键词 ({allKeys.Count}个):", EditorStyles.boldLabel);

        if (allKeys.Count == 0)
        {
            EditorGUILayout.HelpBox("Hint Database 中没有找到任何关键词", MessageType.Warning);
        }
        else
        {
            // 显示前20个key，避免列表过长
            int displayCount = Mathf.Min(allKeys.Count, 20);
            string keysPreview = string.Join(", ", allKeys.GetRange(0, displayCount));
            if (allKeys.Count > 20)
            {
                keysPreview += $"... (还有 {allKeys.Count - 20} 个)";
            }
            EditorGUILayout.HelpBox(keysPreview, MessageType.Info);
        }

        // 输入文本区域
        EditorGUILayout.Space();
        GUILayout.Label("输入文本:", EditorStyles.boldLabel);
        inputScroll = EditorGUILayout.BeginScrollView(inputScroll, GUILayout.Height(100));
        inputText = EditorGUILayout.TextArea(inputText, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        // 处理按钮
        EditorGUILayout.Space();
        if (GUILayout.Button("处理文本", GUILayout.Height(30)))
        {
            ProcessText();
        }

        // 输出文本区域
        EditorGUILayout.Space();
        GUILayout.Label("输出文本:", EditorStyles.boldLabel);
        outputScroll = EditorGUILayout.BeginScrollView(outputScroll, GUILayout.Height(100));
        EditorGUILayout.TextArea(outputText, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        // 复制按钮
        if (!string.IsNullOrEmpty(outputText))
        {
            if (GUILayout.Button("复制到剪贴板"))
            {
                CopyToClipboard();
            }
        }
    }

    private void UpdateKeysList()
    {
        allKeys.Clear();
        if (hintDatabase != null)
        {
            string[] keys = hintDatabase.GetAllKeys();
            allKeys.AddRange(keys);

            // 按长度从长到短排序，避免短关键词匹配长关键词的一部分
            allKeys.Sort((a, b) => b.Length.CompareTo(a.Length));
        }
    }

    private void ProcessText()
    {
        if (string.IsNullOrEmpty(inputText))
        {
            outputText = "";
            return;
        }

        if (allKeys.Count == 0)
        {
            outputText = inputText;
            EditorUtility.DisplayDialog("提示", "没有可用的关键词", "确定");
            return;
        }

        StringBuilder result = new StringBuilder(inputText);
        HashSet<string> processedKeys = new HashSet<string>();

        // 遍历所有关键词，在文本中查找并替换
        foreach (string key in allKeys)
        {
            if (string.IsNullOrEmpty(key)) continue;

            string searchText = result.ToString();
            int startIndex = 0;

            while (startIndex < searchText.Length)
            {
                int foundIndex = searchText.IndexOf(key, startIndex, System.StringComparison.Ordinal);
                if (foundIndex == -1) break;

                // 检查关键词边界（确保不是其他词的一部分）
                bool isWordBoundary = IsWordBoundary(searchText, foundIndex, key.Length);

                if (isWordBoundary)
                {
                    // 构建替换文本
                    string replacement = $"<link={key}><color=blue><u>{key}</u></color></link>";

                    // 替换文本
                    result.Remove(foundIndex, key.Length);
                    result.Insert(foundIndex, replacement);

                    // 更新搜索文本和索引
                    searchText = result.ToString();
                    startIndex = foundIndex + replacement.Length;

                    processedKeys.Add(key);
                }
                else
                {
                    startIndex = foundIndex + key.Length;
                }
            }
        }

        outputText = result.ToString();

        // 显示处理结果
        if (processedKeys.Count > 0)
        {
            Debug.Log($"处理完成！找到了 {processedKeys.Count} 个关键词: {string.Join(", ", processedKeys)}");
        }
        else
        {
            Debug.Log("处理完成！未找到任何关键词");
        }
    }

    private bool IsWordBoundary(string text, int index, int length)
    {
        // 检查左边边界
        if (index > 0)
        {
            char leftChar = text[index - 1];
            if (char.IsLetterOrDigit(leftChar) || leftChar == '_')
                return false;
        }

        // 检查右边边界
        if (index + length < text.Length)
        {
            char rightChar = text[index + length];
            if (char.IsLetterOrDigit(rightChar) || rightChar == '_')
                return false;
        }

        return true;
    }

    private void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = outputText;
        Debug.Log("输出文本已复制到剪贴板");
    }

    // 在窗口打开时更新key列表
    private void OnEnable()
    {
        UpdateKeysList();
    }
}