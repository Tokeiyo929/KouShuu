using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:5934f1ee-fb3c-474a-93f3-e2e1714f8e27
	public partial class UIContractPanel
	{
		public const string Name = "UIContractPanel";
		
		[SerializeField]
		public RectTransform Content_Obj;
		[SerializeField]
		public RectTransform Page_Little;
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_LeftContent;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Audio;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Exit;
		
		private UIContractPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Content_Obj = null;
			Page_Little = null;
			Text_LeftContent = null;
			Btn_Audio = null;
			Btn_Exit = null;
			
			mData = null;
		}
		
		public UIContractPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIContractPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIContractPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
