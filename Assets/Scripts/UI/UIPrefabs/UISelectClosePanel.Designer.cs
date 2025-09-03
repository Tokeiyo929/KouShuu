using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:21058253-5bcb-4d86-81f0-46c832f73c4d
	public partial class UISelectClosePanel
	{
		public const string Name = "UISelectClosePanel";
		
		[SerializeField]
		public UnityEngine.UI.Button Btn_NextStep;
		[SerializeField]
		public UnityEngine.UI.Button Btn_LastStep;
		
		private UISelectClosePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Btn_NextStep = null;
			Btn_LastStep = null;
			
			mData = null;
		}
		
		public UISelectClosePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UISelectClosePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UISelectClosePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
