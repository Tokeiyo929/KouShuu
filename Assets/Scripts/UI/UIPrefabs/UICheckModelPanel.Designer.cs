using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:4639ffce-4f44-497e-9e28-e9b34dd90f08
	public partial class UICheckModelPanel
	{
		public const string Name = "UICheckModelPanel";
		
		
		private UICheckModelPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UICheckModelPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UICheckModelPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UICheckModelPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
