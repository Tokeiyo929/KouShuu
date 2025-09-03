using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:7b8a4f25-239c-44b9-a6fd-da1cffd6e3db
	public partial class UIFinalScorePanel
	{
		public const string Name = "UIFinalScorePanel";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_StudentClass;
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_StudentName;
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_StudentID;
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_EnterTime;
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_TotalTime;
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_TotalScore;
		[SerializeField]
		public UnityEngine.UI.Image Viewport;
		[SerializeField]
		public RectTransform Content;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Return;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Restart;
		[SerializeField]
		public UnityEngine.UI.Button Btn_ExportReport;
		
		private UIFinalScorePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Text_StudentClass = null;
			Text_StudentName = null;
			Text_StudentID = null;
			Text_EnterTime = null;
			Text_TotalTime = null;
			Text_TotalScore = null;
			Viewport = null;
			Content = null;
			Btn_Return = null;
			Btn_Restart = null;
			Btn_ExportReport = null;
			
			mData = null;
		}
		
		public UIFinalScorePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIFinalScorePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIFinalScorePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
