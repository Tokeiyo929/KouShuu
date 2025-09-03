using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:bb827251-42be-4190-a978-600d665b00d1
	public partial class UIHistoryPanel
	{
		public const string Name = "UIHistoryPanel";
		
		[SerializeField]
		public RectTransform Content_Obj;
		[SerializeField]
		public RectTransform Page_Little;
		[SerializeField]
		public RectTransform AllPage;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Last;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Exit;
		
		private UIHistoryPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Content_Obj = null;
			Page_Little = null;
			AllPage = null;
			Btn_Last = null;
			Btn_Next = null;
			Btn_Exit = null;
			
			mData = null;
		}
		
		public UIHistoryPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIHistoryPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIHistoryPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
