using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:6713a3b7-ce04-43f1-a3ea-3983c2ed0925
	public partial class UIMarketEnvironmentPanel
	{
		public const string Name = "UIMarketEnvironmentPanel";
		
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_option_1;
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_option_2;
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_option_3;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Submit_1;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next_1;
		[SerializeField]
		public UnityEngine.UI.Image Img_Correct;
		[SerializeField]
		public UnityEngine.UI.Image Img_Error;
		
		private UIMarketEnvironmentPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Tog_option_1 = null;
			Tog_option_2 = null;
			Tog_option_3 = null;
			Btn_Submit_1 = null;
			Btn_Next_1 = null;
			Img_Correct = null;
			Img_Error = null;
			
			mData = null;
		}
		
		public UIMarketEnvironmentPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIMarketEnvironmentPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIMarketEnvironmentPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
