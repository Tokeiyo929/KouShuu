using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:9a85a3c3-8a15-4c4e-8832-ecc5a7f41d50
	public partial class UIInvitedPanel
	{
		public const string Name = "UIInvitedPanel";
		
		[SerializeField]
		public UnityEngine.UI.Image Page_Invitation;
		[SerializeField]
		public RectTransform InputFileds;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Submit_1;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next_1;
		[SerializeField]
		public UnityEngine.UI.Image Page_Car;
		[SerializeField]
		public UnityEngine.UI.Image Img_Left;
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_location1;
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_location2;
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_location3;
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_location4;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Submit_2;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next_2;
		[SerializeField]
		public UnityEngine.UI.Image Img_Correct;
		[SerializeField]
		public UnityEngine.UI.Image Img_Error;
		
		private UIInvitedPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Page_Invitation = null;
			InputFileds = null;
			Btn_Submit_1 = null;
			Btn_Next_1 = null;
			Page_Car = null;
			Img_Left = null;
			Tog_location1 = null;
			Tog_location2 = null;
			Tog_location3 = null;
			Tog_location4 = null;
			Btn_Submit_2 = null;
			Btn_Next_2 = null;
			Img_Correct = null;
			Img_Error = null;
			
			mData = null;
		}
		
		public UIInvitedPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIInvitedPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIInvitedPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
