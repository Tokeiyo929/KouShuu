using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:23709e66-a2e3-4789-9a69-08a55ce575f4
	public partial class UITipPanel_FirstMeeting
	{
		public const string Name = "UITipPanel_FirstMeeting";
		
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next_2;
		
		private UITipPanel_FirstMeetingData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Btn_Next = null;
			Btn_Next_2 = null;
			
			mData = null;
		}
		
		public UITipPanel_FirstMeetingData Data
		{
			get
			{
				return mData;
			}
		}
		
		UITipPanel_FirstMeetingData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UITipPanel_FirstMeetingData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
