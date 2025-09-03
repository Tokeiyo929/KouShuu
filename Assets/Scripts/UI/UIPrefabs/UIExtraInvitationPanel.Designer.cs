using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:80b300d7-c805-4eb1-bf47-8a0a70a581f9
	public partial class UIExtraInvitationPanel
	{
		public const string Name = "UIExtraInvitationPanel";
		
		[SerializeField]
		public UnityEngine.UI.Image Page_Invitation;
		[SerializeField]
		public RectTransform InputFileds;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Submit_1;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next_1;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Skip_1;
		
		private UIExtraInvitationPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Page_Invitation = null;
			InputFileds = null;
			Btn_Submit_1 = null;
			Btn_Next_1 = null;
			Btn_Skip_1 = null;
			
			mData = null;
		}
		
		public UIExtraInvitationPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIExtraInvitationPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIExtraInvitationPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
