using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:d8fc66ca-9df9-4ad4-8a0c-565893c37c2c
	public partial class UICommunicationPanel
	{
		public const string Name = "UICommunicationPanel";
		
		[SerializeField]
		public RectTransform Content_Obj;
		[SerializeField]
		public RectTransform Page_Little;
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_LeftContent;
		[SerializeField]
		public UnityEngine.UI.Button Btn_AudioLeft;
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_RightContent;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Exit;
		
		private UICommunicationPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Content_Obj = null;
			Page_Little = null;
			Text_LeftContent = null;
			Btn_AudioLeft = null;
			Text_RightContent = null;
			Btn_Exit = null;
			
			mData = null;
		}
		
		public UICommunicationPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UICommunicationPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UICommunicationPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
