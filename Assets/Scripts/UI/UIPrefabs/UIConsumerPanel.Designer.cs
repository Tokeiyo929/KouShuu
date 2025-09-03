using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:d56fcdce-c73f-4372-894e-298047cbd4a3
	public partial class UIConsumerPanel
	{
		public const string Name = "UIConsumerPanel";
		
		[SerializeField]
		public UnityEngine.UI.Image Page_Consumer;
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_Consumer_1;
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_Consumer_2;
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_Consumer_3;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Submit;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next;
		[SerializeField]
		public UnityEngine.UI.Image Img_Correct;
		[SerializeField]
		public UnityEngine.UI.Image Img_Error;
		
		private UIConsumerPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Page_Consumer = null;
			Tog_Consumer_1 = null;
			Tog_Consumer_2 = null;
			Tog_Consumer_3 = null;
			Btn_Submit = null;
			Btn_Next = null;
			Img_Correct = null;
			Img_Error = null;
			
			mData = null;
		}
		
		public UIConsumerPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIConsumerPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIConsumerPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
