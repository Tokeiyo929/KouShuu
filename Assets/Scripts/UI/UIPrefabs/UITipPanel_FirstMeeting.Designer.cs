using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:49feac39-d40d-48b2-b377-899dfe5e407f
	public partial class UITipPanel_FirstMeeting
	{
		public const string Name = "UITipPanel_FirstMeeting";
		
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next;
		
		private UITipPanel_FirstMeetingData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Btn_Next = null;
			
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
