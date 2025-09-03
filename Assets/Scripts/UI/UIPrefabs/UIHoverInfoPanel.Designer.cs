using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:d47bde84-98d5-41ee-ab05-fd5089ed8ea4
	public partial class UIHoverInfoPanel
	{
		public const string Name = "UIHoverInfoPanel";
		
		[SerializeField]
		public UnityEngine.UI.Image Img_BG;
		[SerializeField]
		public TMPro.TextMeshProUGUI Text_Tips;
		
		private UIHoverInfoPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Img_BG = null;
			Text_Tips = null;
			
			mData = null;
		}
		
		public UIHoverInfoPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIHoverInfoPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIHoverInfoPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
