using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:9e7ce337-fb63-4b10-a3f1-0fcd6c6e37ec
	public partial class UITeaMeetingPanel
	{
		public const string Name = "UITeaMeetingPanel";
		
		[SerializeField]
		public RectTransform Tip_SelectError;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next_1;
		[SerializeField]
		public UnityEngine.UI.Button Btn_NextModule;
		
		private UITeaMeetingPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Tip_SelectError = null;
			Btn_Next_1 = null;
			Btn_NextModule = null;
			
			mData = null;
		}
		
		public UITeaMeetingPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UITeaMeetingPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITeaMeetingPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
