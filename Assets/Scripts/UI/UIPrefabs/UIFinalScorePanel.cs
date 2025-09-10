using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Windows;

namespace QFramework.Example
{
	public class UIFinalScorePanelData : UIPanelData
	{
	}
	public partial class UIFinalScorePanel : UIPanel
	{
		private WebPDFGenerator pdfGenerator;
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIFinalScorePanelData ?? new UIFinalScorePanelData();
			// please add init code here

			// 初始化PDF生成器
			pdfGenerator = gameObject.AddComponent<WebPDFGenerator>();
			
			OnClickButton();
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
			SetStudentInfo();
			SetConversationScore();
			GetScore();
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

		private void SetStudentInfo()
		{
			Text_StudentClass.text = "班级：" + Global.StudentClass;
			Text_StudentName.text = "姓名：" + Global.StudentName;
			Text_StudentID.text = "学号：" + Global.StudentId;

			Text_EnterTime.text = "考核时间：" + Global.StudentStartTime.ToString("yyyy-MM-dd HH:mm:ss");
			
			// 计算考核用时
			TimeSpan duration = DateTime.Now - Global.StudentStartTime;
			Text_TotalTime.text = "考核用时：" + duration.ToString(@"mm\:ss");
			
			//Text_TotalScore.text = "得分：" + Global.StudentScore.ToString() ;
		}

		private void SetConversationScore()
		{
			List<int> errorTimes = DialogueManager.Instance.ErrorTimes;

			float totalScore=0;
			for(int i=0;i<errorTimes.Count;i++)
			{
				if(i==0||i==1)
				{
					totalScore=2f;
				}
				else
					totalScore=3f;

				if(i!=2)
				{
					if(errorTimes[i]==0)
					{
						Global.ScoreList[i+6]=totalScore;
					}
					else if(errorTimes[i]<2)
					{
						Global.ScoreList[i+6]=totalScore/2;
					}
					else
					{
						Global.ScoreList[i+6]=0f;
					}
				}
				else
				{
					if(errorTimes[i]==0)
					{
						Global.ScoreList[i+6]=totalScore;
					}
					else
					{
						Global.ScoreList[i+6]=0;
					}
				}
			}
		}

		private void GetScore()
		{
			float score=0;
			for(int i=0;i<Content.transform.childCount;i++)
			{
				score+=Global.ScoreList[i];
				Content.transform.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text =  Global.ScoreList[i].ToString();
			}

			Text_TotalScore.text = "得分：\n" + score.ToString("F1") ;
		}

		private void OnClickButton()
		{
			Btn_Return.onClick.AddListener(OnClickReturn);
			Btn_Restart.onClick.AddListener(OnClickRestart);
			Btn_ExportReport.onClick.AddListener(OnClickExport);
		}

		private void OnClickReturn()
		{
			UIKit.CloseAllPanel();
			UIKit.OpenPanel<UICoverPanel>(UILevel.Common, null, null, "UIPrefabs/UICoverPanel");
			Global.ScoreList.Clear();
            if (Global.ScoreList.Count == 0)
            {
                for (int i = 0; i < 18; i++)
                {
                    Global.ScoreList.Add(0f);
                }
            }
        }

		private void OnClickRestart()
		{
			UIKit.CloseAllPanel();
			UIKit.OpenPanel<UITitlePanel>(UILevel.PopUI, null, null, "UIPrefabs/UITitlePanel");
			UIKit.OpenPanel<UIDialoguePanel>(UILevel.PopUI, null, null, "UIPrefabs/UIDialoguePanel");
			UIKit.OpenPanel<UISelectPanel>(UILevel.Common, null, null, "UIPrefabs/UISelectPanel");
            Global.ScoreList.Clear();
            if (Global.ScoreList.Count == 0)
            {
                for (int i = 0; i < 18; i++)
                {
                    Global.ScoreList.Add(0f);
                }
            }
        }

		private void OnClickExport()
		{
			StartCoroutine(ExportScoreReport());
		}
		
		private IEnumerator ExportScoreReport()
		{
			Debug.Log("开始导出成绩报告...");
			
			                // 在WebGL平台，确保jsPDF库已加载并设置
                #if UNITY_WEBGL && !UNITY_EDITOR
                    Debug.Log("WebGL平台：确保jsPDF库已加载...");
                    pdfGenerator.EnsureJsPDFLoaded();
                    
                    // 等待更长时间让jsPDF库加载和设置完成
                    yield return new WaitForSeconds(2.0f);
                    Debug.Log("jsPDF库设置等待完成");
                #endif
			
			// 验证基础数据
			if (Global.StudentClass == null || Global.StudentName == null || Global.StudentId == null)
			{
				Debug.LogError("学生基础信息不完整，无法导出");
				yield break;
			}
			
			if (Content == null)
			{
				Debug.LogError("Content对象为空，无法导出");
				yield break;
			}
			
			// 准备学生信息
			var studentInfo = new Dictionary<string, string>
			{
				["class"] = Global.StudentClass ?? "未知班级",
				["name"] = Global.StudentName ?? "未知姓名",
				["id"] = Global.StudentId ?? "未知学号",
				["enterTime"] = Global.StudentStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
				["totalTime"] = (DateTime.Now - Global.StudentStartTime).ToString(@"mm\:ss"),
				["endTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
				["totalScore"] = Text_TotalScore != null ? Text_TotalScore.text.Replace("得分：\n", "") : "0"
			};
			
			Debug.Log($"学生信息准备完成: {Global.StudentClass}, {Global.StudentName}, {Global.StudentId}");
			
			// 验证Content对象结构
			if (!pdfGenerator.ValidateContentStructure(Content.transform))
			{
				Debug.LogError("Content对象结构验证失败，无法继续导出");
				yield break;
			}
			
			// 从Content读取成绩数据
			var scoreItems = pdfGenerator.ReadScoreData(Content.transform);
			Debug.Log($"从Content读取到 {scoreItems.Count} 个成绩项目");
			
			// 验证分数数据
			if (Global.ScoreList == null || Global.ScoreList.Count == 0)
			{
				Debug.LogError("Global.ScoreList为空，无法生成PDF");
				yield break;
			}
			
			Debug.Log($"分数列表包含 {Global.ScoreList.Count} 个分数");
			for (int i = 0; i < Global.ScoreList.Count; i++)
			{
				Debug.Log($"第{i+1}题分数: {Global.ScoreList[i]}");
			}
			
			// 生成PDF
			try
			{
				pdfGenerator.CreatePDF(Global.ScoreList, studentInfo, scoreItems);
				Debug.Log("成绩报告导出完成");
			}
			catch (System.Exception e)
			{
				Debug.LogError("生成PDF时发生错误: " + e.Message);
				Debug.LogError("错误堆栈: " + e.StackTrace);
			}
			
			yield return null;
		}
		
	}
}
