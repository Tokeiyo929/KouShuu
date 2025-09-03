using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:58ca23e3-fbb0-400b-8df5-d12171a0446e
	public partial class UITitlePanel
	{
		public const string Name = "UITitlePanel";
		
		[SerializeField]
		public UnityEngine.UI.Button Btn_ChangText;
		[SerializeField]
		public UnityEngine.UI.Button Btn_ChangAudio;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Back;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Step1;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Step2;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Step3;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Step4;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Step5;
		
		private UITitlePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Btn_ChangText = null;
			Btn_ChangAudio = null;
			Btn_Back = null;
			Btn_Step1 = null;
			Btn_Step2 = null;
			Btn_Step3 = null;
			Btn_Step4 = null;
			Btn_Step5 = null;
			
			mData = null;
		}
		
		public UITitlePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UITitlePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITitlePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
