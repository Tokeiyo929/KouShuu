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

        // ѡ�� ScriptableObject
        EditorGUI.BeginChangeCheck();
        hintDatabase = (HintDatabase)EditorGUILayout.ObjectField("Hint Database", hintDatabase, typeof(HintDatabase), false);
        if (EditorGUI.EndChangeCheck())
        {
            UpdateKeysList();
        }

        if (hintDatabase == null)
        {
            EditorGUILayout.HelpBox("����ѡ��һ�� Hint Database", MessageType.Info);
            return;
        }

        // ��ʾ���п��õ�key
        EditorGUILayout.Space();
        GUILayout.Label($"���ùؼ��� ({allKeys.Count}��):", EditorStyles.boldLabel);

        if (allKeys.Count == 0)
        {
            EditorGUILayout.HelpBox("Hint Database ��û���ҵ��κιؼ���", MessageType.Warning);
        }
        else
        {
            // ��ʾǰ20��key�������б����
            int displayCount = Mathf.Min(allKeys.Count, 20);
            string keysPreview = string.Join(", ", allKeys.GetRange(0, displayCount));
            if (allKeys.Count > 20)
            {
                keysPreview += $"... (���� {allKeys.Count - 20} ��)";
            }
            EditorGUILayout.HelpBox(keysPreview, MessageType.Info);
        }

        // �����ı�����
        EditorGUILayout.Space();
        GUILayout.Label("�����ı�:", EditorStyles.boldLabel);
        inputScroll = EditorGUILayout.BeginScrollView(inputScroll, GUILayout.Height(100));
        inputText = EditorGUILayout.TextArea(inputText, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        // ����ť
        EditorGUILayout.Space();
        if (GUILayout.Button("�����ı�", GUILayout.Height(30)))
        {
            ProcessText();
        }

        // ����ı�����
        EditorGUILayout.Space();
        GUILayout.Label("����ı�:", EditorStyles.boldLabel);
        outputScroll = EditorGUILayout.BeginScrollView(outputScroll, GUILayout.Height(100));
        EditorGUILayout.TextArea(outputText, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        // ���ư�ť
        if (!string.IsNullOrEmpty(outputText))
        {
            if (GUILayout.Button("���Ƶ�������"))
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

            // �����ȴӳ��������򣬱���̹ؼ���ƥ�䳤�ؼ��ʵ�һ����
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
            EditorUtility.DisplayDialog("��ʾ", "û�п��õĹؼ���", "ȷ��");
            return;
        }

        StringBuilder result = new StringBuilder(inputText);
        HashSet<string> processedKeys = new HashSet<string>();

        // �������йؼ��ʣ����ı��в��Ҳ��滻
        foreach (string key in allKeys)
        {
            if (string.IsNullOrEmpty(key)) continue;

            string searchText = result.ToString();
            int startIndex = 0;

            while (startIndex < searchText.Length)
            {
                int foundIndex = searchText.IndexOf(key, startIndex, System.StringComparison.Ordinal);
                if (foundIndex == -1) break;

                // ���ؼ��ʱ߽磨ȷ�����������ʵ�һ���֣�
                bool isWordBoundary = IsWordBoundary(searchText, foundIndex, key.Length);

                if (isWordBoundary)
                {
                    // �����滻�ı�
                    string replacement = $"<link={key}><color=blue><u>{key}</u></color></link>";

                    // �滻�ı�
                    result.Remove(foundIndex, key.Length);
                    result.Insert(foundIndex, replacement);

                    // ���������ı�������
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

        // ��ʾ������
        if (processedKeys.Count > 0)
        {
            Debug.Log($"������ɣ��ҵ��� {processedKeys.Count} ���ؼ���: {string.Join(", ", processedKeys)}");
        }
        else
        {
            Debug.Log("������ɣ�δ�ҵ��κιؼ���");
        }
    }

    private bool IsWordBoundary(string text, int index, int length)
    {
        // �����߽߱�
        if (index > 0)
        {
            char leftChar = text[index - 1];
            if (char.IsLetterOrDigit(leftChar) || leftChar == '_')
                return false;
        }

        // ����ұ߽߱�
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
        Debug.Log("����ı��Ѹ��Ƶ�������");
    }

    // �ڴ��ڴ�ʱ����key�б�
    private void OnEnable()
    {
        UpdateKeysList();
    }
}