using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:14c3ebbb-7855-4c05-a485-993e115c0767
	public partial class UILogicPanel
	{
		public const string Name = "UILogicPanel";
		
		
		private UILogicPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UILogicPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UILogicPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UILogicPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
