using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class WebPDFGenerator : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void GeneratePDF(string jsonData);
    
    [DllImport("__Internal")]
    private static extern void SetJSPDF(object jsPDFObject);

    /// <summary>
    /// 设置jsPDF库对象（供HTML页面调用）
    /// </summary>
    /// <param name="jsPDFObject">jsPDF库对象</param>
    public void SetJSPDFLibrary(object jsPDFObject)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                SetJSPDF(jsPDFObject);
                Debug.Log("jsPDF库对象已设置到.jslib环境");
            }
            catch (System.Exception e)
            {
                Debug.LogError("设置jsPDF库对象失败: " + e.Message);
            }
        #else
            Debug.Log("SetJSPDFLibrary: 仅在WebGL平台可用");
        #endif
    }
    
    /// <summary>
    /// 确保jsPDF库已加载并设置（供Unity调用）
    /// </summary>
    public void EnsureJsPDFLoaded()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                // 直接调用HTML页面的jsPDF加载函数
                Application.ExternalEval(@"
                    console.log('Unity调用：开始加载jsPDF库...');
                    
                    // 检查jsPDF库是否已加载
                    if(typeof window.jsPDF !== 'undefined') {
                        console.log('jsPDF库已加载，直接设置到Unity环境');
                        if(typeof window.unityInstance !== 'undefined') {
                            window.unityInstance.SendMessage('WebPDFGenerator', 'SetJSPDFLibrary', window.jsPDF);
                            console.log('jsPDF库已成功设置到Unity环境');
                        }
                    } else if(typeof window.jspdf !== 'undefined' && window.jspdf.jsPDF) {
                        console.log('jspdf对象存在，设置jsPDF库');
                        window.jsPDF = window.jspdf.jsPDF;
                        if(typeof window.unityInstance !== 'undefined') {
                            window.unityInstance.SendMessage('WebPDFGenerator', 'SetJSPDFLibrary', window.jsPDF);
                            console.log('jsPDF库已成功设置到Unity环境');
                        }
                                         } else {
                         console.log('jsPDF库未加载，开始加载...');
                         
                         // 首先尝试加载本地jsPDF库
                         var script = document.createElement('script');
                         script.src = 'jspdf.umd.min.js';
                         script.onload = function() {
                             console.log('本地jsPDF库加载成功');
                             if(window.jspdf && window.jspdf.jsPDF) {
                                 window.jsPDF = window.jspdf.jsPDF;
                                 console.log('jsPDF库设置成功');
                                 
                                 // 设置到Unity环境
                                 if(typeof window.unityInstance !== 'undefined') {
                                     window.unityInstance.SendMessage('WebPDFGenerator', 'SetJSPDFLibrary', window.jsPDF);
                                     console.log('jsPDF库已成功设置到Unity环境');
                                 }
                             }
                         };
                         script.onerror = function() {
                             console.error('本地jsPDF库加载失败');
                             console.error('请确保jspdf.umd.min.js文件存在于构建目录中');
                         };
                         document.head.appendChild(script);
                     }
                ");
                Debug.Log("已调用HTML页面的jsPDF加载函数");
            }
            catch (System.Exception e)
            {
                Debug.LogError("调用HTML页面函数失败: " + e.Message);
            }
        #else
            Debug.Log("EnsureJsPDFLoaded: 仅在WebGL平台可用");
        #endif
    }
    
    /// <summary>
    /// 生成成绩单PDF（Web端）
    /// </summary>
    /// <param name="scoreList">分数列表</param>
    /// <param name="studentInfo">学生信息</param>
    /// <param name="scoreItems">成绩项目详细信息</param>
    public void CreatePDF(List<float> scoreList, Dictionary<string, string> studentInfo, List<ScoreItem> scoreItems = null)
    {
        try
        {
            // 验证数据
            if (scoreList == null || scoreList.Count == 0)
            {
                Debug.LogError("分数列表为空，无法生成PDF");
                return;
            }
            
            if (studentInfo == null)
            {
                Debug.LogError("学生信息为空，无法生成PDF");
                return;
            }
            
            // 准备数据
            var pdfData = new PDFData
            {
                studentClass = studentInfo.ContainsKey("class") ? studentInfo["class"] : "未知班级",
                studentName = studentInfo.ContainsKey("name") ? studentInfo["name"] : "未知姓名",
                studentId = studentInfo.ContainsKey("id") ? studentInfo["id"] : "未知学号",
                enterTime = studentInfo.ContainsKey("enterTime") ? studentInfo["enterTime"] : "未知时间",
                totalTime = studentInfo.ContainsKey("totalTime") ? studentInfo["totalTime"] : "0:00",
                endTime = studentInfo.ContainsKey("endTime") ? studentInfo["endTime"] : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                totalScore = studentInfo.ContainsKey("totalScore") ? studentInfo["totalScore"] : "0",
                scores = scoreList,
                scoreItems = scoreItems
            };

            // 转换为JSON - 使用更稳定的序列化方式
            string jsonData;
            try
            {
                // 尝试使用JsonUtility
                jsonData = JsonUtility.ToJson(pdfData);
                
                // 检查JSON是否包含无效字符
                if (jsonData.Contains("") || jsonData.Contains("null"))
                {
                    Debug.LogWarning("JsonUtility产生无效字符，尝试手动构建JSON");
                    // 手动构建JSON以避免编码问题
                    jsonData = BuildJSONManually(pdfData);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("JsonUtility序列化失败，使用手动构建: " + e.Message);
                jsonData = BuildJSONManually(pdfData);
            }
            
            Debug.Log("准备生成PDF，数据: " + jsonData);
            
            // 调用JavaScript函数生成PDF
            #if UNITY_WEBGL && !UNITY_EDITOR
                GeneratePDF(jsonData);
                Debug.Log("已调用JavaScript PDF生成函数");
            #else
                Debug.Log("PDF数据已准备就绪（仅Web端可用）: " + jsonData);
                Debug.Log("在编辑器中，您可以查看控制台输出来验证数据格式");
            #endif
        }
        catch (System.Exception e)
        {
            Debug.LogError("生成PDF时发生错误: " + e.Message);
        }
    }

    /// <summary>
    /// 从Content对象读取成绩数据
    /// </summary>
    /// <param name="content">Content对象</param>
    /// <returns>成绩数据列表</returns>
    public List<ScoreItem> ReadScoreData(Transform content)
    {
        List<ScoreItem> scoreItems = new List<ScoreItem>();
        
        if (content == null) return scoreItems;
        
        for (int i = 0; i < content.childCount; i++)
        {
            Transform child = content.GetChild(i);
            if (child.childCount >= 4)
            {
                ScoreItem item = new ScoreItem
                {
                    index = i + 1,
                    step = GetTextFromChild(child, 1), // 步骤
                    expectedScore = GetTextFromChild(child, 2), // 应得分数
                    actualScore = GetTextFromChild(child, 3)   // 实际分数
                };
                scoreItems.Add(item);
            }
        }
        
        return scoreItems;
    }

    /// <summary>
    /// 从子物体获取文本内容
    /// </summary>
    /// <param name="parent">父物体</param>
    /// <param name="childIndex">子物体索引</param>
    /// <returns>文本内容</returns>
    private string GetTextFromChild(Transform parent, int childIndex)
    {
        if (parent.childCount > childIndex)
        {
            Transform child = parent.GetChild(childIndex);
            var textComponent = child.GetComponent<TMPro.TextMeshProUGUI>();
            if (textComponent != null)
            {
                return textComponent.text;
            }
        }
        return "";
    }
    
    /// <summary>
    /// 验证Content对象结构是否正确
    /// </summary>
    /// <param name="content">Content对象</param>
    /// <returns>结构是否有效</returns>
    public bool ValidateContentStructure(Transform content)
    {
        if (content == null)
        {
            Debug.LogError("Content对象为空");
            return false;
        }
        
        int childCount = content.childCount;
        if (childCount != 18)
        {
            Debug.LogWarning($"Content对象包含 {childCount} 个子物体，期望18个");
        }
        
        for (int i = 0; i < childCount; i++)
        {
            Transform child = content.GetChild(i);
            if (child.childCount < 4)
            {
                Debug.LogError($"第{i+1}个子物体只包含 {child.childCount} 个子物体，期望至少4个");
                return false;
            }
            
            // 检查是否有TextMeshPro组件
            for (int j = 0; j < 4; j++)
            {
                Transform subChild = child.GetChild(j);
                var textComponent = subChild.GetComponent<TMPro.TextMeshProUGUI>();
                if (textComponent == null)
                {
                    Debug.LogWarning($"第{i+1}个子物体的第{j+1}个子物体缺少TextMeshPro组件");
                }
            }
        }
        
        Debug.Log("Content对象结构验证通过");
        return true;
    }
    
    /// <summary>
    /// 手动构建JSON字符串，避免编码问题
    /// </summary>
    private string BuildJSONManually(PDFData pdfData)
    {
        try
        {
            var json = new System.Text.StringBuilder();
            json.Append("{");
            
            // 学生信息
            json.AppendFormat("\"studentClass\":\"{0}\",", EscapeString(pdfData.studentClass));
            json.AppendFormat("\"studentName\":\"{0}\",", EscapeString(pdfData.studentName));
            json.AppendFormat("\"studentId\":\"{0}\",", EscapeString(pdfData.studentId));
            json.AppendFormat("\"enterTime\":\"{0}\",", EscapeString(pdfData.enterTime));
            json.AppendFormat("\"totalTime\":\"{0}\",", EscapeString(pdfData.totalTime));
            json.AppendFormat("\"endTime\":\"{0}\",", EscapeString(pdfData.endTime));
            json.AppendFormat("\"totalScore\":\"{0}\",", EscapeString(pdfData.totalScore));
            
            // 分数列表
            json.Append("\"scores\":[");
            if (pdfData.scores != null)
            {
                for (int i = 0; i < pdfData.scores.Count; i++)
                {
                    if (i > 0) json.Append(",");
                    json.Append(pdfData.scores[i].ToString("F2"));
                }
            }
            json.Append("],");
            
            // 成绩项目
            json.Append("\"scoreItems\":[");
            if (pdfData.scoreItems != null)
            {
                for (int i = 0; i < pdfData.scoreItems.Count; i++)
                {
                    if (i > 0) json.Append(",");
                    var item = pdfData.scoreItems[i];
                    json.Append("{");
                    json.AppendFormat("\"index\":{0},", item.index);
                    json.AppendFormat("\"step\":\"{0}\",", EscapeString(item.step));
                    json.AppendFormat("\"expectedScore\":\"{0}\",", EscapeString(item.expectedScore));
                    json.AppendFormat("\"actualScore\":\"{0}\"", EscapeString(item.actualScore));
                    json.Append("}");
                }
            }
            json.Append("]");
            
            json.Append("}");
            return json.ToString();
        }
        catch (System.Exception e)
        {
            Debug.LogError("手动构建JSON失败: " + e.Message);
            return "{}";
        }
    }
    
    /// <summary>
    /// 转义JSON字符串中的特殊字符
    /// </summary>
    private string EscapeString(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        
        return input.Replace("\\", "\\\\")
                   .Replace("\"", "\\\"")
                   .Replace("\n", "\\n")
                   .Replace("\r", "\\r")
                   .Replace("\t", "\\t");
    }
}

[System.Serializable]
public class PDFData
{
    public string studentClass;
    public string studentName;
    public string studentId;
    public string enterTime;
    public string totalTime;
    public string endTime;
    public string totalScore;
    public List<float> scores;
    public List<ScoreItem> scoreItems;
}

[System.Serializable]
public class ScoreItem
{
    public int index;
    public string step;
    public string expectedScore;
    public string actualScore;
}
