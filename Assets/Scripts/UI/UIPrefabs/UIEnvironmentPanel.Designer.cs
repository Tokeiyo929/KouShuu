using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:f08bd8e8-eed4-4d39-b52c-379209ee88dc
	public partial class UIEnvironmentPanel
	{
		public const string Name = "UIEnvironmentPanel";
		
		
		private UIEnvironmentPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UIEnvironmentPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIEnvironmentPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIEnvironmentPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
