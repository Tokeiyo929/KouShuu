using System.IO;
using UnityEditor;
using UnityEngine;
using ExcelDataReader;
using QFramework.Example;

public class ExcelToDialogueJournalSO : EditorWindow
{
    private string excelFolderPath = "Assets/StreamingAssets/Excel/DialogueJournal";
    private string outputFolderPath = "Assets/Resources/ScriptableObjects/DialogueJournal";

    [MenuItem("Tools/Excel To Dialogue Journal SO")]
    public static void ShowWindow()
    {
        GetWindow<ExcelToDialogueJournalSO>("Excel To Dialogue Journal SO");
    }

    private void OnGUI()
    {
        GUILayout.Label("Excel转DialogueJournal ScriptableObject工具", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        GUILayout.Label("Excel文件夹路径:");
        excelFolderPath = EditorGUILayout.TextField(excelFolderPath);
        
        GUILayout.Label("输出SO文件夹路径:");
        outputFolderPath = EditorGUILayout.TextField(outputFolderPath);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("选择Excel文件夹"))
        {
            string path = EditorUtility.OpenFolderPanel("选择Excel文件夹", excelFolderPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                excelFolderPath = path;
            }
        }
        
        if (GUILayout.Button("选择输出文件夹"))
        {
            string path = EditorUtility.OpenFolderPanel("选择输出文件夹", outputFolderPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                outputFolderPath = path;
            }
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("生成所有SO文件"))
        {
            GenerateAllSOFiles();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox("使用说明:\n1. 确保Excel文件夹路径正确\n2. 设置输出SO文件夹路径\n3. 点击'生成所有SO文件'按钮\n4. 生成的SO文件将保存在输出文件夹中\n5. 所有行都会被生成，包括选项和结局行", MessageType.Info);
    }

    private void GenerateAllSOFiles()
    {
        if (!Directory.Exists(excelFolderPath))
        {
            EditorUtility.DisplayDialog("错误", "Excel文件夹不存在！", "确定");
            return;
        }

        // 确保输出文件夹存在
        if (!Directory.Exists(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
        }

        string[] excelFiles = Directory.GetFiles(excelFolderPath, "*.xlsx");
        
        if (excelFiles.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "未找到Excel文件！", "确定");
            return;
        }

        int successCount = 0;
        int totalCount = excelFiles.Length;

        foreach (string excelFile in excelFiles)
        {
            try
            {
                if (GenerateSOFromExcel(excelFile))
                {
                    successCount++;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"处理文件 {excelFile} 时出错: {e.Message}");
            }
        }

        EditorUtility.DisplayDialog("完成", $"成功生成 {successCount}/{totalCount} 个SO文件！", "确定");
        
        // 强制刷新资源
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
        
        // 如果DialogueJournalManager已经存在，强制刷新其数据
        if (Application.isPlaying)
        {
            var manager = FindObjectOfType<QFramework.Example.DialogueJournalManager>();
            if (manager != null)
            {
                manager.ForceRefreshDialogueData();
                Debug.Log("已自动刷新DialogueJournalManager的数据");
            }
        }
    }

    private bool GenerateSOFromExcel(string excelFilePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(excelFilePath);
        
        // 创建SO文件
        DialogueJournalData soData = ScriptableObject.CreateInstance<DialogueJournalData>();
        soData.conversationName = fileName;
        soData.dialogueEntries.Clear();

        try
        {
            using (var stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                // 跳过表头
                if (!reader.Read()) return false;

                int rowIndex = 0;
                while (reader.Read())
                {
                    string characterName_CN = SafeGetString(reader, 0);
                    string characterName_EN = SafeGetString(reader, 1);
                    string dialogue_CN = SafeGetString(reader, 2);
                    string dialogue_EN = SafeGetString(reader, 3);

                    // 不再跳过选项和结局行，全部生成
                    var entry = new DialogueJournalData.DialogueEntry
                    {
                        characterName_CN = characterName_CN,
                        characterName_EN = characterName_EN,
                        dialogue_CN = dialogue_CN,
                        dialogue_EN = dialogue_EN,
                        color = Color.black
                    };

                    soData.dialogueEntries.Add(entry);
                    rowIndex++;
                }
            }

            // 保存SO文件
            string outputPath = Path.Combine(outputFolderPath, fileName + ".asset");
            AssetDatabase.CreateAsset(soData, outputPath);
            
            Debug.Log($"成功生成SO文件: {outputPath}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"生成SO文件失败 {fileName}: {e.Message}");
            return false;
        }
    }

    private static string SafeGetString(IExcelDataReader reader, int index)
    {
        if (index < 0 || index >= reader.FieldCount) return string.Empty;
        try
        {
            return reader.GetValue(index)?.ToString() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
} 