using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Components;

public class AddComponentForLocalization
{
    [MenuItem("Tools/输出预制体本地化信息")]
    static void OutputLocalizationInfo()
    {
        ExecuteLocalizationOutput();
    }
    
    [MenuItem("Tools/输出预制体本地化信息", true)]
    static bool ValidateOutputLocalizationInfo()
    {
        return Selection.activeGameObject != null;
    }
    
    static void ExecuteLocalizationOutput()
    {
        // 获取当前选中的GameObject
        GameObject selectedObject = Selection.activeGameObject;
        
        if (selectedObject == null)
        {
            Debug.LogWarning("请先选择一个预制体或GameObject");
            return;
        }
        
        // 在选中的GameObject及其子物体中查找所有LocalizeStringEvent组件
        LocalizeStringEvent[] localizeComponents = selectedObject.GetComponentsInChildren<LocalizeStringEvent>(true);
        
        if (localizeComponents.Length == 0)
        {
            Debug.Log($"在 '{selectedObject.name}' 中未找到LocalizeStringEvent组件");
            return;
        }
        
        Debug.Log($"在 '{selectedObject.name}' 中找到 {localizeComponents.Length} 个LocalizeStringEvent组件，正在添加LocalizationText组件...");
        
        int successCount = 0;
        int failureCount = 0;
        
        for (int i = 0; i < localizeComponents.Length; i++)
        {
            LocalizeStringEvent component = localizeComponents[i];
            GameObject targetObject = component.gameObject;
            string objectPath = GetGameObjectPath(targetObject, selectedObject);
            
            // 获取本地化字符串引用
            var stringReference = component.StringReference;
            
            if (stringReference != null && stringReference.TableReference != null)
            {
                // 获取不同语言的文本
                var chineseText = GetLocalizedText(stringReference, "zh-Hans");
                var englishText = GetLocalizedText(stringReference, "en");
                
                // 检查是否已经有LocalizationText组件
                LocalizationText localizationText = targetObject.GetComponent<LocalizationText>();
                
                if (localizationText == null)
                {
                    // 添加LocalizationText组件
                    localizationText = targetObject.AddComponent<LocalizationText>();
                    Debug.Log($"✓ 为 '{objectPath}' 添加了LocalizationText组件");
                }
                else
                {
                    Debug.Log($"○ '{objectPath}' 已存在LocalizationText组件，正在更新...");
                }
                
                // 使用反射设置私有字段
                SetPrivateField(localizationText, "chineseText", chineseText);
                SetPrivateField(localizationText, "englishText", englishText);
                
                Debug.Log($"  中文文本: {chineseText}");
                Debug.Log($"  英文文本: {englishText}");
                
                // 删除LocalizeStringEvent组件
                UnityEngine.Object.DestroyImmediate(component);
                Debug.Log($"  已删除LocalizeStringEvent组件");
                
                // 标记对象为已修改
                UnityEditor.EditorUtility.SetDirty(targetObject);
                
                successCount++;
            }
            else
            {
                Debug.LogWarning($"✗ '{objectPath}' 的LocalizeStringEvent组件StringReference为空或无效，跳过处理");
                failureCount++;
            }
        }
        
        Debug.Log($"\n=== 处理完成 ===");
        Debug.Log($"成功处理: {successCount} 个组件");
        if (failureCount > 0)
        {
            Debug.Log($"失败/跳过: {failureCount} 个组件");
        }
    }
    
    // 辅助方法：获取GameObject在层级中的路径
    static string GetGameObjectPath(GameObject obj, GameObject root)
    {
        if (obj == root)
            return obj.name;
            
        string path = obj.name;
        Transform parent = obj.transform.parent;
        
        while (parent != null && parent.gameObject != root)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        if (parent != null)
            path = root.name + "/" + path;
            
        return path;
    }
    
    // 辅助方法：获取指定语言的本地化文本
    static string GetLocalizedText(UnityEngine.Localization.LocalizedString stringReference, string localeCode)
    {
        try
        {
            // 查找指定的Locale
            var targetLocale = FindLocaleByCode(localeCode);
            if (targetLocale == null)
            {
                return $"[错误: 未找到语言代码 '{localeCode}']";
            }

            // 获取字符串表
            var stringTable = UnityEngine.Localization.Settings.LocalizationSettings.StringDatabase.GetTable(stringReference.TableReference, targetLocale);
            if (stringTable == null)
            {
                return $"[错误: 未找到表格 '{stringReference.TableReference}' 对应语言 '{localeCode}']";
            }

            // 获取表格条目
            UnityEngine.Localization.Tables.StringTableEntry entry = null;
            
            // 尝试不同的方式获取条目
            if (!string.IsNullOrEmpty(stringReference.TableEntryReference.Key))
            {
                entry = stringTable.GetEntry(stringReference.TableEntryReference.Key);
            }
            else if (stringReference.TableEntryReference.KeyId != 0)
            {
                entry = stringTable.GetEntry(stringReference.TableEntryReference.KeyId);
            }
            
            if (entry == null)
            {
                return $"[错误: 未找到条目 Key:'{stringReference.TableEntryReference.Key}' KeyId:{stringReference.TableEntryReference.KeyId}]";
            }

            // 获取本地化字符串
            return entry.GetLocalizedString();
        }
        catch (System.Exception e)
        {
            return $"[错误: {e.Message}]";
        }
    }

    // 辅助方法：根据语言代码查找Locale
    static UnityEngine.Localization.Locale FindLocaleByCode(string localeCode)
    {
        var availableLocales = UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in availableLocales)
        {
            if (locale.Identifier.Code == localeCode)
            {
                return locale;
            }
        }
        return null;
    }
    
    // 辅助方法：使用反射设置私有字段
    static void SetPrivateField(object target, string fieldName, object value)
    {
        var type = target.GetType();
        var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            field.SetValue(target, value);
        }
        else
        {
            Debug.LogWarning($"无法找到字段 '{fieldName}' 在类型 '{type.Name}' 中");
        }
    }
}
