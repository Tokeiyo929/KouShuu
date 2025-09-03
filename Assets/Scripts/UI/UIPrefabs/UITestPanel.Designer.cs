using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:abd7fcdd-9d21-45e8-b71e-53e62a20f75d
	public partial class UITestPanel
	{
		public const string Name = "UITestPanel";
		
		
		private UITestPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UITestPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UITestPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITestPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
