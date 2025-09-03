using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:199d5187-cd37-4653-b339-7de9bec595f4
	public partial class UILoginPanel
	{
		public const string Name = "UILoginPanel";
		
		[SerializeField]
		public TMPro.TMP_InputField InputField_Name;
		[SerializeField]
		public TMPro.TMP_InputField InputField_Class;
		[SerializeField]
		public TMPro.TMP_InputField InputField_ID;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Check;
		
		private UILoginPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			InputField_Name = null;
			InputField_Class = null;
			InputField_ID = null;
			Btn_Check = null;
			
			mData = null;
		}
		
		public UILoginPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UILoginPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UILoginPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
