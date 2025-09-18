using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:8980306c-ead1-403f-b1a0-7ec444e8e4b1
	public partial class UIMeetingPanel
	{
		public const string Name = "UIMeetingPanel";
		
		[SerializeField]
		public RectTransform Tips_Handshake;
		[SerializeField]
		public RectTransform Tips_Greetings;
		[SerializeField]
		public RectTransform Tips_Introduction;
		[SerializeField]
		public UnityEngine.UI.Image Page_Greetings;
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_Handshake;
		[SerializeField]
		public UnityEngine.UI.Toggle Tog_Embrace;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Check;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Next;
		[SerializeField]
		public UnityEngine.UI.Image Img_Correct;
		[SerializeField]
		public UnityEngine.UI.Image Img_Error;
		[SerializeField]
		public UnityEngine.UI.Button Btn_NextPage;
		
		private UIMeetingPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Tips_Handshake = null;
			Tips_Greetings = null;
			Tips_Introduction = null;
			Page_Greetings = null;
			Tog_Handshake = null;
			Tog_Embrace = null;
			Btn_Check = null;
			Btn_Next = null;
			Img_Correct = null;
			Img_Error = null;
			Btn_NextPage = null;
			
			mData = null;
		}
		
		public UIMeetingPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIMeetingPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIMeetingPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
